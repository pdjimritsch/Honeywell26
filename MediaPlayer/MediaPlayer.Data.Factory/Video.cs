namespace MediaPlayer.Data.Factory;

using Abstraction;

public partial class Video : IVideo
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string? ContentDirectory { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? ContentType { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public long ContentLength { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public string? FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string? FileName { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? Title { get; set; } = null!;

    #endregion
}
