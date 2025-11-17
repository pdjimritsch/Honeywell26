using System;

namespace MediaPlayer.Data.Factory.Abstraction;

public partial interface IVisitor
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    string? ContentDirectory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    bool IsContentAppearing { get; set; }

    /// <summary>
    /// Unique visitor identity
    /// </summary>
    Guid Token { get; }

    /// <summary>
    /// 
    /// </summary>
    string? VideoContentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    long VideoContentLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? VideoFileExtension { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? VideoFileName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? VideoTitle { get; set; }

    #endregion

}