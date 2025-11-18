namespace MediaPlayer.Extensions;

using ViewModels;

public static partial class ApplicationEventContext
{
    #region Events

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IHostApplicationLifetime? Register(this IHostApplicationLifetime? lifetime, WebApplication? app)
    {
        lifetime?.ApplicationStarted.Register(() => 
        {
        });   

        lifetime?.ApplicationStopping.Register(() =>
        {
            AppGenerator.Messages.Clear();
        });

        return lifetime;
    }

    #endregion
}
