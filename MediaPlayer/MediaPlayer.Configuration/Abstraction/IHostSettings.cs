using Microsoft.Extensions.Configuration;

namespace MediaPlayer.Configuration.Abstraction;

public partial interface IHostSettings
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    IConfiguration? Configuration { get; }

    /// <summary>
    /// 
    /// </summary>
    long MaxMovieSize { get; }

    #endregion
}
