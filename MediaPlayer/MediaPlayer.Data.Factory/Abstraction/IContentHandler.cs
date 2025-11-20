namespace MediaPlayer.Data.Factory.Abstraction;

public partial interface IContentHandler
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    bool IsContentAppearing { get; set; }

    /// <summary>
    /// 
    /// </summary>
    int MessageIndex { get; set; }

    #endregion
}
