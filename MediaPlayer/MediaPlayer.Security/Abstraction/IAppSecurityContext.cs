namespace MediaPlayer.Security.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface IAppSecurityContext
{
    #region Properties

    /// <summary>
    /// Gets or sets a System.Boolean value that indicates whether the 
    /// certificate is checked against the certificate authority revocation list.
    /// </summary>
    /// <remarks>
    /// True if the certificate revocation list is checked; otherwise False.
    /// </remarks>
     bool CheckCertificateRevocationList { get; set; }

    /// <summary>
    ///  Gets or sets the maximum number of concurrent connections allowed by a
    ///  System.Net.ServicePoint object.
    /// </summary>
    /// <remarks>
    /// The maximum number of concurrent connections allowed by a 
    /// System.Net.ServicePoint.
    /// object. 
    /// The default connection limit is 10 for ASP.NET hosted applications
    /// and 2 for all others.
    /// When an app is running as an ASP.NET host, it is not possible
    /// to alter the value of this property through the config file if
    /// the autoConfig property is set to True.
    /// However, you can change the value programmatically when the autoConfig 
    /// property is true.
    /// Set your preferred value once, when the AppDomain loads.
    /// </remarks>
     int DefaultConnectionLimit { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates how long a Domain Name Service (DNS)
    /// resolution os considered valid.
    /// </summary>
    /// <remarks>
    /// The time-out value, in milliseconds. 
    /// A value of -1 indicates an infinite time-out period. 
    /// The default value is 120,000 milliseconds (two minutes).
    /// </remarks>
     int DnsRefreshTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether a Domain Name Service (DNS)
    /// resolution rotates among the applicable Internet Protocol (IP) addresses.
    /// </summary>
    /// <remarks>
    /// False if a DNS resolution always returns the first IP address for a particular
    /// host; otherwise true. The default is false.
    /// </remarks>
     bool EnableDnsRoundRobin { get; set; }

    /// <summary>
    /// Gets or sets a System.Boolean value that determines whether 
    /// 100-Continue behavior is employed.
    /// </summary>
     bool Expect100Continue { get; set; }

    /// <summary>
    /// Gets or sets the maximum idle time of a System.Net.ServicePoint object.
    /// </summary>
    /// <remarks>
    /// The maximum idle time, in milliseconds, of a System.Net.ServicePoint object.
    /// The default value is 100,000 milliseconds (100 seconds).
    /// </remarks>
     int MaxServicePointIdleTime { get; set; }

    /// <summary>
    ///  The maximum number of System.Net.ServicePoint objects to maintain. The default
    ///  value is 0, which means there is no limit to the number of System.Net.ServicePoint
    ///   objects.
    /// </summary>
     int MaxServicePoints { get; set; }

    /// <summary>
    ///     Setting this property value to true causes all outbound TCP connections from
    ///     HttpWebRequest to use the native socket option SO_REUSE_UNICASTPORT on the socket.
    ///     This causes the underlying outgoing ports to be shared. This is useful for scenarios
    ///     where a large number of outgoing connections are made in a short time, and the
    ///     app risks running out of ports.
    /// </summary>
     bool ReusePort { get; set; }

    /// <summary>
    /// 
    /// </summary>
     SecurityProtocolType SecurityProtocol { get; set; }

    /// <summary>
    /// Gets or sets the callback to validate a server certificate.
    /// </summary>
    /// <remarks>
    /// The default setting for the callback is NULL.
    /// </remarks>
     RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// Determines whether the Nagle algorithm is used by the service points 
    /// managed by this System.Net.ServicePointManager object.
    /// </summary>
    /// <remarks>
    /// True to employ the Nagle algorithm. The default value is True.
    /// </remarks>
     bool UseNagleAlgorithm { get; set; }

    #endregion
}
