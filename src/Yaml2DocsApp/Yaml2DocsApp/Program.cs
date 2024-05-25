using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Yaml2DocsApp;
using Yaml2DocsApp.Properties;
using Yaml2DocsApp.Services;

// 処理対象
List<string> targets = null;
// ロガー
ILogger<Program> logger;
// サービスプロバイダー
IServiceProvider provider;

// 初期化
Setup();

var datetime = DateTime.Now;
logger.LogInformation(Resources.MsgBatStartLog);

// 各処理の実行
var isAbort = false;
foreach (var target in targets)
{
    // サービスクラスの取得
    var service = getService(target);
    if (service == null)
    {
        logger.LogError(Resources.MsgErrServiceNotFound, target);
        continue;
    }

    // サービスクラスの実行
    if (!service.Execute())
    {
        isAbort = true;
    }
    Thread.Sleep(1000);
}

// 全処理の終了ログ
if (isAbort)
{
    logger.LogError(Resources.MsgBatEndLog, DateTime.Now.Subtract(datetime));
}
else
{
    logger.LogInformation(Resources.MsgBatEndLog, DateTime.Now.Subtract(datetime));
}
Thread.Sleep(1000);

/// <summary>
/// バッチ処理を初期化します。
/// </summary>
void Setup()
{
    // アプリケーション設定
    var confBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
    var configuration = confBuilder.Build();

    // 処理対象の取得
    if (targets == null)
    {
        targets = configuration.GetSection("Targets").Get<List<string>>();
    }

    // サービスの生成
    var services = new ServiceCollection();
    services.AddScoped<IConfiguration>(_ => configuration);
    services.AddLogging(builder => {
        builder.AddConfiguration(configuration.GetSection("Logging"));
        builder.AddConsole();
        builder.AddLog4Net();
    });
    services.AddSingleton<IYml2MdService, Yml2MdService>();
    provider = services.BuildServiceProvider();

    // ロガーの生成
    var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
    logger = loggerFactory.CreateLogger<Program>();
}

/// <summary>
/// 処理名から対象のサービスクラスを取得します。
/// </summary>
/// <param name="target">処理名</param>
/// <returns>サービスクラス</returns>
IServiceBase getService(string target)
{
    switch (target)
    {
        case "B1001":
            return provider.GetService<IYml2MdService>();
        default:
            return null;
    }
}
