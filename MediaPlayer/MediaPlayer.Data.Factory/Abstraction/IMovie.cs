namespace MediaPlayer.Data.Factory.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface IMovie
{
    #region Properties

    /// <summary>
    /// Movie size in bytes
    /// </summary>
    long ContentLength { get; }

    /// <summary>
    /// 
    /// </summary>
    string? ContentType { get; }

    /// <summary>
    /// Movie filename reference
    /// </summary>
    string? FileName { get; }

    /// <summary>
    /// 
    /// </summary>
    Guid Token { get; } 

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<IVisitor> Visitors { get; }

    #endregion

    #region Functions

    /// <summary>
    /// Registers or unregisters the viewed movie.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="filename"></param>
    /// <param name="unregister"></param>
    /// <returns></returns>
    bool AddOrRemoveMovie(IVisitor? visitor, string? filename, bool unregister = false);

    #endregion
}
