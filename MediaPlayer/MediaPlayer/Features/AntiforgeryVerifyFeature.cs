namespace MediaPlayer.Features;

internal sealed partial class AntiforgeryVerifyFeature : IAntiforgeryValidationFeature
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    public static readonly IAntiforgeryValidationFeature Valid = new AntiforgeryVerifyFeature(true, null);

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isValid"></param>
    /// <param name="error"></param>
    internal AntiforgeryVerifyFeature(bool isValid, AntiforgeryValidationException? error) : base()
    {
        IsValid = isValid;

        Error = error;
    }

    #endregion

    #region IAntiforgeryValidationFeature Members

    /// <summary>
    /// 
    /// </summary>
    public bool IsValid { get; private set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public Exception? Error { get; private set; } = null!;

    #endregion

}
