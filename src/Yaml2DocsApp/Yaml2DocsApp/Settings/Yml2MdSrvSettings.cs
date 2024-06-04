namespace Yaml2DocsApp.Settings
{
    /// <summary>
    /// YAML to Markdown処理の設定情報クラス
    /// </summary>
    public class Yml2MdSrvSettings
    {
        /// <summary>
        /// Markdownテンプレートファイルパス
        /// </summary>
        public string TemplateFile { get; set; }
        /// <summary>
        /// YAMLファイル格納フォルダパス
        /// </summary>
        public string YmlFolder { get; set; }
        /// <summary>
        /// Markdown出力先フォルダパス
        /// </summary>
        public string ExportFolder { get; set; }
    }
}
