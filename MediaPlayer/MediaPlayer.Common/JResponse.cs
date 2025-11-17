namespace MediaPlayer.Common;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class JResponse<T> where T: class, new()
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public List<T> Container { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public AggregateException? Errors { get; set; } = default;

    #endregion
}
