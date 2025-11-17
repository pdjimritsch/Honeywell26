namespace MediaPlayer.Configuration;

using Abstraction;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Application configuration
/// </summary>
public partial class HostSettings : IHostSettings
{
    #region Constructors

    /// <summary>
    /// Accesses the properties within appsettings.json resource.
    /// </summary>
    /// <param name="configuration"></param>
    public HostSettings(IConfiguration? configuration) : base()
    {
        Configuration = configuration;

        MaxMovieSize = configuration?.GetValue<long?>(nameof(MaxMovieSize)) ?? 0;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public IConfiguration? Configuration { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public long MaxMovieSize { get; private set; }

    #endregion

}
