using MediaPlayer.Security;
using MediaPlayer.Security.Abstraction;
using System.Net;

namespace MediaPlayer.Extensions;

public static partial class SecurityContext
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder? InjectSecurity(this WebApplicationBuilder? builder)
    {
        // register REST based services
        builder?.Services.AddSingleton<IAppSecurityContext>(provider =>
        {
            return new AppSecurityContext
            {
                CheckCertificateRevocationList = true,
                SecurityProtocol = SecurityProtocolType.Tls13,
            };
        });

        return builder;
    }

    #endregion
}
