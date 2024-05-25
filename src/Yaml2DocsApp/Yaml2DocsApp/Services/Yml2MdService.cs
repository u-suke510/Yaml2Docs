using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaml2DocsApp.Properties;
using YamlDotNet.Serialization;

namespace Yaml2DocsApp.Services
{
    /// <summary>
    /// YAML to Markdown処理のServiceクラスのサンプルインターフェース
    /// </summary>
    public interface IYml2MdService : IServiceBase
    {
    }

    /// <summary>
    /// YAML to Markdown処理のServiceクラスのサンプルクラス
    /// </summary>
    public class Yml2MdService : ServiceBase, IYml2MdService
    {
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
        public Yml2MdService(IServiceProvider provider, ILogger<Yml2MdService> logger) : base(provider, logger)
        {
            // 処理IDの設定
            SrvId = Resources.SrvIdYml2Md;
            // 処理名の設定
            SrvNm = Resources.SrvNmYml2Md;
        }

        /// <summary>
        /// YAML to Markdown処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            return true;
        }
    }
}
