using Microsoft.AspNetCore.Cors.Infrastructure;

namespace MediaPlayer.Extensions;

/// <summary>
/// 
/// </summary>
public static partial class CorsExtensions
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void Configure(this CorsPolicyBuilder? builder, IConfiguration? configuration)
    {
        string[] headers = ["Origin", "X-Requested-With", "Accept"];
        builder?.WithHeaders(headers);
        builder?.WithExposedHeaders(headers);

        string[] services = ["GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH", "HEAD", "JSONP"];
        builder?.WithMethods(services);

        builder?.AllowCredentials();
        builder?.AllowAnyOrigin();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static WebApplication? EnableCors(this WebApplication? application, IConfiguration? configuration)
    {
        application?.UseCors(options =>
        {
            options.Configure(configuration);
        });

        return application;
    }

    #endregion
}
