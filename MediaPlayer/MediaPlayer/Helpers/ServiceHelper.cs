namespace MediaPlayer.Helpers;

/// <summary>
/// 
/// </summary>
public static partial class ServiceHelper
{
    #region Extensions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void GetCacheOptions(MemoryCacheOptions options)
    {
        options.CompactionPercentage = 0.5;
        options.SizeLimit = 16384 * 4; // 64 MB
    }

    #endregion
}
