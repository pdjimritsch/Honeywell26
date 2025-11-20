using MediaPlayer.Configuration;
using MediaPlayer.Security;
using MediaPlayer.Security.Abstraction;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;

using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CoreJsonOption = Microsoft.AspNetCore.Http.Json;

namespace MediaPlayer;

using Extensions;
using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.ViewModels;
using Microsoft.AspNetCore.HttpLogging;

/// <summary>
/// 
/// </summary>
public static partial class AppGenerator
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    internal static string? ContentDirectory = null!;

    /// <summary>
    /// Retains customised banner content.
    /// </summary>
    internal static Dictionary<int, MessageTemplate> Messages = [];
    
    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="appRoot"></param>
    /// <returns></returns>
    public static WebApplication? BuildApplication(
        this WebApplicationBuilder? builder, IConfiguration? configuration, string? appRoot)
    {
        WebApplication? application = default;

        if (builder == null) return application;

        application = builder.Build();

        if (application == null) return application;

        /**
         * Exception Handling for rednered MVC 5 razor pages
         */
        if (application.Environment.IsDevelopment())
        {
            application.UseDeveloperExceptionPage();
        }
        else
        {
            application.UseExceptionHandler("/Error");
            application.UseHsts();
        }

        //application.EnableCors(configuration);

        application.UseHttpLogging();

        application.UseSession();

        application.UseRouting();

        if (!application.Environment.IsDevelopment())
        {
            application.UseHttpsRedirection();
        }

        /**
         * MIDDLEWARE Registration
         */
        //application
        //    .UseMiddleware<AntiforgeryMiddleware>()
        //    .UseMiddleware<ApiKeyMiddleware>()
        //    .UseMiddleware<ExceptionHandlerMiddleware>();

        application.MapAssets(configuration);

        return application;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static HostSettings GetAppConfiguration(this IServiceCollection? services, IConfiguration? configuration)
    {
        var settings = new HostSettings(configuration);

        services?.AddSingleton<IHostSettings>(settings);

        return settings;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appRoot"></param>
    /// <param name="environment"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplicationOptions GetWebApplicationOptions(string appRoot, string environment, string[] args)
    {
        return new WebApplicationOptions
        {
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
            Args = args,
            ContentRootPath = Directory.GetCurrentDirectory(),
            EnvironmentName = string.IsNullOrEmpty(environment) ? Environments.Development : environment.Trim(),
            WebRootPath = string.IsNullOrEmpty(appRoot) ? "/" : appRoot.Trim(),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static WebApplication? MapAssets(this WebApplication? application, IConfiguration? configuration)
    {
        application?.MapControllers();

        application?.MapStaticAssets();

        application?.MapRazorPages().WithStaticAssets();

        return application;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder? SetCors(this IHostApplicationBuilder? builder, IConfiguration? configuration)
    {
        builder?.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.Configure(configuration);
            });
        });

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder? SetHttpClient(this IHostApplicationBuilder builder, IConfiguration? configuration)
    {
        builder?.Services.AddHttpClient<IHttpCommunication, HttpCommunication>(nameof(HttpCommunication), (client, provider) =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);

            return new HttpCommunication(provider, client)
            {
                SecurityContext = provider.GetRequiredService<IAppSecurityContext>() ??
                    new AppSecurityContext
                    {
                        CheckCertificateRevocationList = true,
                        SecurityProtocol = SecurityProtocolType.Tls13,
                    }
            };
        });

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder? SetJsonOptions(this IHostApplicationBuilder? builder)
    {
        builder?.Services.Configure<CoreJsonOption.JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.MaxDepth = int.MaxValue;
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
        });

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// /** Checks the occurrence of distributed denial of service attacks */
    /// </remarks>
    /// <param name="options"></param>
    public static void SetRequestRateLimit(RateLimiterOptions options)
    {
        if (options != null)
        {
            Func<string, FixedWindowRateLimiterOptions> constraint = (value) =>
            {
                return new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20,
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    Window = TimeSpan.FromSeconds(30),
                };
            };

            Func<HttpContext, RateLimitPartition<string>> service = (ctx) =>
            {
                var addr = ctx.Connection.RemoteIpAddress;

                string key = StringExtensions.Generate(256);

                if (addr != null)
                {
                    key = addr.ToString();
                }
                else if (ctx != null && ctx.User != null && ctx.User.Identity != null)
                {
                    if (ctx.User.Identity.Name != null)
                    {
                        key = ctx.User.Identity.Name.Trim();
                    }
                }

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey: key, factory: (partition) => constraint(partition));
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx => service(ctx));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="args"></param>
    public static void Start(string environment, string[] args)
    {
        string appRoot = "/";

        var builder = WebApplication.CreateBuilder(GetWebApplicationOptions(appRoot, environment, args));

        var browserUrl = builder.Configuration["ASPNETCORE_URLS"];

        var properties = builder.Services.GetAppConfiguration(builder.Configuration);

        builder.Services.AddOptions();

        builder.Services.AddHealthChecks();

        // add forward header
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        // add data protection middleware
        builder.Services.AddDataProtection(options =>
        {
            options.ApplicationDiscriminator = Assembly.GetExecutingAssembly().GetName().Name;
        })
        .AddKeyManagementOptions(options => { options.AutoGenerateKeys = true; })
        .PersistKeysToFileSystem(new DirectoryInfo(AppContext.BaseDirectory))
        .SetApplicationName(nameof(MediaPlayer))
        .SetDefaultKeyLifetime(TimeSpan.FromDays(7));

        // add antiforgery middleware token
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = @"X-XSRF-Token";
        });

        builder.Services.AddHttpLogging(options => 
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody;
            options.RequestBodyLogLimit = 4096; // Limit body size to prevent excessive logging
            options.ResponseBodyLogLimit = 4096;
            // You can also redact sensitive headers or specify headers to log
            options.RequestHeaders.Add("X-My-Custom-Header");
            options.ResponseHeaders.Add("X-My-Response-Header");
            options.ResponseHeaders.Add("Content-Type");
            options.RequestHeaders.Add("Authorization"); // This will be redacted by default
        });

        /** Checks the occurrence of distributed denial of service attacks */
        builder.Services.AddRateLimiter(options =>
        {
            SetRequestRateLimit(options);
        });

        //builder.SetCors(builder.Configuration);

        // configure JSON response options
        builder.SetJsonOptions();

        // register web application injectable services
        builder.InjectServices(properties.Configuration, appRoot);

        // register routing security
        builder.InjectSecurity();

        // register HTTP client
        builder.SetHttpClient(properties.Configuration);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // register user session 
        builder.Services.AddSession();

        builder.Services.AddDistributedMemoryCache(options =>
        {
            options.TrackLinkedCacheEntries = true;
        });

        //builder.Services.AddProblemDetails();

        builder.Services.AddRazorComponents(options =>
        {
            options.MaxFormMappingRecursionDepth = 10;

            options.MaxFormMappingCollectionSize = 50;

            options.DetailedErrors = true;

            options.MaxFormMappingKeySize = 1024;
        });

        builder.Services.AddRazorPages();

        var application = builder.BuildApplication(properties.Configuration, appRoot);

        application?.Run();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static MessageTemplate GetCurrentMessage(int index)
    {
        if ((Messages.Count == 0) || (index < 0) || (index >= Messages.Count))
            return new MessageTemplate(string.Empty, MessageTemplateType.Error);

        return Messages[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    internal static string GetMessageStyle(MessageTemplate template)
    {
        return template.Type == MessageTemplateType.Error ? "warning" : "information";
    }

    /// <summary>
    /// Imports the video into the video folder
    /// </summary>
    /// <param name="video">
    /// Selected video resource.
    /// </param>
    internal static string SaveVideo(IFormFile? video)
    {
        string filename = string.Empty;

        if ((video != null) && !string.IsNullOrEmpty(video.FileName))
        {

            if (!string.IsNullOrEmpty(ContentDirectory))
            {
                filename = Path.Combine(ContentDirectory, "videos", video.FileName);
            }
            else
            {
                ContentDirectory = AppContext.BaseDirectory;

                filename = Path.Combine(AppContext.BaseDirectory, "wwwroot/videos", video.FileName);
            }

            if (File.Exists(filename))
            {
                var reference = new FileInfo(filename);

                if (reference.Length != video.Length)
                {
                    try { File.Delete(filename); } finally { }

                    using FileStream stream = new(filename, FileMode.Create, FileAccess.Write);

                    video.CopyTo(stream);

                    stream.Flush();

                    stream.Close();
                }
            }
            else
            {
                using FileStream stream = new(filename, FileMode.Create, FileAccess.Write);

                video.CopyTo(stream);

                stream.Flush(true);

                stream.Close();
            }
        }

        return filename;
    }

    #endregion
}
