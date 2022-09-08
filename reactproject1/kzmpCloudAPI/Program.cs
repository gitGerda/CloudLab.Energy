using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using kzmpCloudAPI.Components.Auth;
using kzmpCloudAPI.CustomMiddlewares.SimpleTokenChecking;
using kzmpCloudAPI.CustomMiddlewares;
using kzmpCloudAPI.Services;
using Hangfire;
using kzmpCloudAPI.Controllers.SignalR;
using System.Globalization;
using System.Net;
using Hangfire.MySql;
using kzmpCloudAPI.Services.BackgroundServices.XML80020_RabbitMQ_Consumer;
using kzmpCloudAPI.Database.EF_Core;
using System.Runtime.CompilerServices;
using NLog.Web;
using NLog;

[assembly: InternalsVisibleTo("kzmpCloudAPI.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.issuer,
                ValidateAudience = true,
                ValidAudience = AuthOptions.audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.key)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<kzmp_energyContext>(option =>
    {
        string connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
        option.UseMySql(connectionStr, ServerVersion.Parse("8.0.29-mysql"));
    });
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "ClientApp/build";
    });
    builder.Services.AddReportXml80020TransientService();
    builder.Services.AddLogging();

    builder.Services.AddIndicationHandlerSingletonService();
    builder.Services.AddRabbitMQPersistentConnectionSingletonService();
    builder.Services.AddRabbitMQConsumerSingletonService();
    builder.Services.AddRabbitMQPublisherConsumerSingletonService();
    builder.Services.AddHostedService<Xml80020ConsumerService>();
    builder.Services.AddHangfire(conf => conf.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                            .UseSimpleAssemblyNameTypeSerializer()
                                            .UseRecommendedSerializerSettings()
                                            .UseStorage(new MySqlStorage(builder.Configuration["HANGFIRE_STORAGE_CONN_STR"], new MySqlStorageOptions())));
    builder.Services.AddSingleton(new HttpClient(new SocketsHttpHandler()));
    builder.Services.AddHangfireShedulerCustomService();
    builder.Services.AddFilesCompressingScopedService();
    //builder.Services.AddSignalR();


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseRUCulture();
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSimpleTokenChecking();

    app.UseSpaStaticFiles();
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";
    });

    app.MapControllers();
    //app.MapHub<DataCollectionWorkersHub>("/dataCollectionWorkersSignalRHub");

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
