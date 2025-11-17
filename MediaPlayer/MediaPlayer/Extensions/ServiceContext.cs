namespace MediaPlayer.Extensions;

using Helpers;
using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;

public static partial class ServiceContext
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="application_root"></param>
    /// <returns></returns>
    public static WebApplicationBuilder InjectServices(
        this WebApplicationBuilder builder, IConfiguration? configuration, string application_root)
    {
        builder.Services.AddMemoryCache(options => ServiceHelper.GetCacheOptions(options));

        builder.Services.AddSingleton<ITokenStore>(TokenStore.Instance);

        return builder;
    }

    #endregion
}
