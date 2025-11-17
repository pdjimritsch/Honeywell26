
namespace MediaPlayer.Security.Abstraction;

using Headers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Link: https://www.restapitutorial.com/lessons/httpmethods.html#:~:text=The%20primary%20or%20most%2Dcommonly,but%20are%20utilized%20less%20frequently.
/// 
/// A PATCH is not necessarily idempotent, although it can be. Contrast this with PUT; which is always idempotent. 
/// The word "idempotent" means that any number of repeated, identical requests will leave the resource in the same state. 
/// For example if an auto-incrementing counter field is an integral part of the resource, 
/// then a PUT will naturally overwrite it (since it overwrites everything), but not necessarily so for PATCH.
/// </remarks>
public partial interface IHttpCommunication : IAsyncDisposable, IDisposable
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    HttpRequestHeaders? DefaultRequestHeaders { get; }

    /// <summary>
    /// 
    /// </summary>
    AggregateException? Errors { get; }

    /// <summary>
    /// 
    /// </summary>
    IAppSecurityContext? SecurityContext { get; set; }

    #endregion
}
