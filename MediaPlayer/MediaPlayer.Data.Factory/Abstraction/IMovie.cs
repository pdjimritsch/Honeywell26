namespace MediaPlayer.Data.Factory.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface IMovie
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    string? ContentDirectory { get; }

    /// <summary>
    /// Movie size in bytes
    /// </summary>
    long ContentLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? ContentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? FileExtension { get; set; }

    /// <summary>
    /// Movie filename reference
    /// </summary>
    string? FileName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    Guid Token { get; set; } 

    /// <summary>
    /// 
    /// </summary>
    List<Visitor> Visitors { get; set; }

    #endregion

    #region Functions

    /// <summary>
    /// Registers or unregisters the viewed movie.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="filename"></param>
    /// <param name="unregister"></param>
    /// <returns></returns>
    bool AddOrRemoveMovie(Visitor? visitor, IVideo video, string? filename, bool unregister = false);

    #endregion
}
