using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Reflection;
using System.Text;
using Yaml2DocsApp.Data;
using Yaml2DocsApp.Properties;
using Yaml2DocsApp.Settings;
using YamlDotNet.Serialization;

namespace Yaml2DocsApp.Services
{
    /// <summary>
    /// YAML to Markdown処理のServiceクラスのサンプルインターフェース
    /// </summary>
    public interface IYml2MdService : IServiceBase
    {
        /// <summary>
        /// YAMLファイルをデシリアライズします。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>YAMLデータ</returns>
        T DeserializeYaml<T>(string path) where T : YmlDataBase;

        /// <summary>
        /// Markdownファイル出力します。
        /// </summary>
        /// <param name="data">YAMLデータ</param>
        /// <param name="path">テンプレートファイルパス</param>
        /// <param name="export">出力ファイルパス</param>
        /// <returns>処理結果</returns>
        bool ExportMd<T>(T data, string path, string export) where T : YmlDataBase;
    }

    /// <summary>
    /// YAML to Markdown処理のServiceクラスのサンプルクラス
    /// </summary>
    public class Yml2MdService : ServiceBase, IYml2MdService
    {
        /// <summary>
        /// YAML to Markdown処理の設定情報
        /// </summary>
        private readonly Yml2MdSrvSettings settings;

        /// <summary>
        /// 処理ID
        /// </summary>
        protected override string SrvId { get; }
        /// <summary>
        /// 処理名
        /// </summary>
        protected override string SrvNm { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="provider">サービスプロバイダー</param>
        /// <param name="logger">ロガー</param>
        public Yml2MdService(IServiceProvider provider, IOptions<Yml2MdSrvSettings> options, ILogger<Yml2MdService> logger) : base(provider, logger)
        {
            // 処理IDの設定
            SrvId = Resources.SrvIdYml2Md;
            // 処理名の設定
            SrvNm = Resources.SrvNmYml2Md;

            // 設定情報
            settings = options.Value;
        }

        /// <summary>
        /// YAML to Markdown処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            // Markdownテンプレートの存在チェック
            if (!File.Exists(settings.TemplateFile))
            {
                logger.LogError(Resources.MsgErrTemplateFileNotFound);
                return false;
            }
            // YAMLフォルダの存在チェック
            if (!Directory.Exists(settings.YmlFolder))
            {
                logger.LogInformation(Resources.MsgYmlFileNotFound);
                return true;
            }

            // YAMLフォルダ内の全てのymlファイルをMarkdownに変換
            var ymlFiles = Directory.GetFiles(settings.YmlFolder, "*.yml");
            foreach (var file in ymlFiles)
            {
                // YAMLデータの取得
                var ymlData = DeserializeYaml<DbDefinition>(file);

                // Markdownの出力
                var mdFile = $"{settings.ExportFolder}\\{Path.GetFileNameWithoutExtension(file)}.md";
                ExportMd(ymlData, settings.TemplateFile, mdFile);
            }

            return true;
        }

        /// <summary>
        /// YAMLファイルをデシリアライズします。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>YAMLデータ</returns>
        public T DeserializeYaml<T>(string path) where T : YmlDataBase
        {
            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                // デシリアライザインスタンス作成
                var deserializer = new Deserializer();
                // yamlデータのオブジェクトを作成
                return deserializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// Markdownファイル出力します。
        /// </summary>
        /// <param name="data">YAMLデータ</param>
        /// <param name="path">テンプレートファイルパス</param>
        /// <param name="export">出力ファイルパス</param>
        /// <returns>処理結果</returns>
        public bool ExportMd<T>(T data, string path, string export) where T : YmlDataBase
        {
            // テンプレートの読込み
            var md = File.ReadAllText(path, Encoding.UTF8);

            // YAMLファイルデータをテンプレートに埋め込み
            md = replaceItem(md, data);

            // ファイル出力
            File.WriteAllText(export, md);
            return true;

            /// <summary>
            /// テンプレート文字列を置き換えます。
            /// </summary>
            /// <param name="text">テンプレート文字列</param>
            /// <param name="key">キー値</param>
            /// <param name="value">置き換えデータ</param>
            /// <returns>変換後の文字列</returns>
            string replaceText(string text, string key, object value)
            {
                // IF項目の変換
                var result = replaceIfText(text, key, value);
                // 文字列の置き換え
                return result.Replace($"{{{{{key}}}}}", $"{value}");
            }

            /// <summary>
            /// テンプレート文字列のIF項目を置き換えます。
            /// </summary>
            /// <param name="text">テンプレート文字列</param>
            /// <param name="key">キー値</param>
            /// <param name="value">置き換えデータ</param>
            /// <returns>変換後の文字列</returns>
            string replaceIfText(string text, string key, object value)
            {
                // IF項目のキー値
                var ifKey = $"{{{{#if {key}}}}}";
                var ifEndKey = $"{{{{/if}}}}";

                // IF項目が存在しない場合、そのまま返す
                var index = text.LastIndexOf(ifKey);
                if (index <= 0)
                {
                    return text;
                }

                // IF項目の表示チェック
                var isIfTrue = hasIfValue(value);
                // テンプレート文全てのIF項目を変換
                var result = text;
                while (0 < index)
                {
                    // 対象のIF項目を取得
                    var ifText = result.Substring(index);
                    var ifEndIndex = ifText.IndexOf(ifEndKey);
                    ifText = ifText.Substring(0, ifEndIndex + ifEndKey.Length);

                    // IF項目の表示
                    if (isIfTrue)
                    {
                        // IF項目の中身のみ残す
                        result = result.Remove(index + ifEndIndex, ifEndKey.Length);
                        result = result.Remove(index, ifKey.Length);
                    }
                    else
                    {
                        // IF項目の全てを削除
                        result = result.Remove(index, ifText.Length);
                    }

                    // 次のIF項目を検索
                    index = result.LastIndexOf(ifKey);
                }

                return result;
            }

            /// <summary>
            /// IF項目をチェックします。
            /// </summary>
            /// <param name="value">置き換えデータ</param>
            /// <returns>IF項目のチェック結果</returns>
            bool hasIfValue(object value)
            {
                // NULLの場合、全てFalse
                if (value == null)
                {
                    return false;
                }

                // boolの場合、値で判定
                if (value.GetType() == typeof(bool))
                {
                    return (bool)value;
                }

                // stringの場合、値の有無で判定
                if (value.GetType() == typeof(string))
                {
                    return !string.IsNullOrEmpty(value.ToString());
                }

                return false;
            }

            /// <summary>
            /// テンプレート文字列のEach項目を置き換えます。
            /// </summary>
            /// <param name="text">テンプレート文字列</param>
            /// <param name="items">Each項目データ</param>
            /// <param name="key">キー値</param>
            /// <returns>変換後の文字列</returns>
            string replaceListText(string text, IList items, string key)
            {
                // Each項目のキー値
                var eachKey = $"{{{{#each {key}}}}}";
                var eachEndKey = $"{{{{/each}}}}";

                // Each項目が存在しない場合、そのまま返す
                var index = text.LastIndexOf(eachKey);
                if (index <= 0)
                {
                    return text;
                }

                // テンプレート文全てのEach項目を変換
                var result = text;
                while (0 < index)
                {
                    // 対象のEach項目を取得
                    var eachText = result.Substring(index);
                    var eachEndIndex = eachText.IndexOf(eachEndKey);
                    eachText = eachText.Substring(0, eachEndIndex + eachEndKey.Length);

                    // テンプレート文からEach項目を削除
                    result = result.Remove(index, eachText.Length);
                    // 繰り返しのテンプレート文の抽出
                    eachText = eachText.Substring(0, eachEndIndex).Remove(0, eachKey.Length);

                    // 繰り返しテンプレート文の埋め込み
                    var eachValues = new StringBuilder();
                    foreach (var item in items)
                    {
                        eachValues.Append(replaceItem(eachText, item));
                    }
                    // テンプレート文にEach項目を追加
                    result = result.Insert(index, eachValues.ToString());

                    // 次のEach項目を検索
                    index = result.LastIndexOf(eachKey);
                }

                return result;
            }

            /// <summary>
            /// テンプレート文字列を置き換えます。
            /// </summary>
            /// <param name="text">テンプレート文字列</param>
            /// <param name="item">置き換えデータ</param>
            /// <returns>変換後の文字列</returns>
            string replaceItem(string text, object item)
            {
                var props = item.GetType().GetProperties();
                foreach (var prop in props)
                {
                    // YamlMember属性のみ
                    var attr = prop.GetCustomAttribute<YamlMemberAttribute>();
                    if (attr == null)
                    {
                        continue;
                    }

                    // 通常クラスの場合、文字列の置き換え
                    if (prop.PropertyType.IsTypeDefinition)
                    {
                        text = replaceText(text, attr.Alias, prop.GetValue(item));
                        continue;
                    }

                    // List以外は未実装(必要にな場合は実装する。)
                    if (prop.PropertyType.GetGenericTypeDefinition() != typeof(List<>))
                    {
                        throw new NotImplementedException();
                    }

                    // List項目の取得
                    var lstItems = prop.GetValue(item) as IList;
                    if (lstItems == null)
                    {
                        // List以外は未実装(必要にな場合は実装する。)
                        throw new NotImplementedException();
                    }

                    // テンプレート文の置き換え
                    text = replaceListText(text, lstItems, attr.Alias);
                }

                return text;
            }
        }
    }
}
