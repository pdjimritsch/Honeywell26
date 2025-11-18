namespace MediaPlayer.Data.Factory.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface IVideo
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    string? ContentDirectory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? ContentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    long ContentLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? FileExtension { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? FileName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? Title { get; set; }

    #endregion
}
