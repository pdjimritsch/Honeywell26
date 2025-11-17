using MediaPlayer.Data.Factory.Abstraction;

namespace MediaPlayer.ViewModels;

public sealed partial class WatchedMovie
{
    #region Properties;

    /// <summary>
    /// 
    /// </summary>
    public long ContentLength { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public IVisitor? Visitor { get; set; } = null!;

    #endregion
}
