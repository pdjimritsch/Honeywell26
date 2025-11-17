using MediaPlayer.Features;

using System.Net;

namespace MediaPlayer.Middleware;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// https://plbonneville.com/blog/sending-an-anti-forgery-token-with-asp.net-core-mvc-ajax-requests/
/// </remarks>
public partial class AntiforgeryMiddleware
{
    #region Constants

    /// <summary>
    /// 
    /// </summary>
    private readonly static string AnttiforgeryKey = "__MediaPlayer_Antiforgery";

    /// <summary>
    /// 
    /// </summary>
    private static readonly object VaultAntiforgery = new ();

    #endregion

    #region Members

    /// <summary>
    /// 
    /// </summary>
    private readonly RequestDelegate? _next;

    /// <summary>
    /// 
    /// </summary>
    private readonly IAntiforgery? _antiforgery;

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger? _logger;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public AntiforgeryMiddleware(IAntiforgery? antiforgery, RequestDelegate? next, ILogger? logger) : base()
    {
        _antiforgery = antiforgery;

        _next = next;

        _logger = logger;
    }

    #endregion

    #region Functions

    #pragma warning disable VSTHRD200

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context != null)
            {
                var endpoint = context.GetEndpoint();

                if (endpoint is not null)
                {
                    context.Items[AnttiforgeryKey] = VaultAntiforgery;
                }

                var method = context.Request.Method;

                if ((endpoint is not null) &&
                    (endpoint.Metadata.GetMetadata<IAntiforgeryMetadata>() is { RequiresValidation: true }))
                {
                    await InitiateVerificationAsync(context);
                }
            }
        }
        catch (Exception violation)
        {
            MiddlewareError.SetError(context, violation, _logger);
        }
        finally
        {
            if (_next != null && context != null)
            {
                await _next(context);
            }
        }

        await Task.Yield();
    }

#pragma warning restore VSTHRD200

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InitiateVerificationAsync(HttpContext context)
    {
        try
        {
            if (_antiforgery != null && context != null)
            {
                await _antiforgery.ValidateRequestAsync(context);
            }

            if (context != null)
            {
                context.Features.Set(AntiforgeryVerifyFeature.Valid);
            }
        }
        catch (AntiforgeryValidationException violation)
        {
            if (context != null)
            {
                context.Features.Set<IAntiforgeryValidationFeature>(new AntiforgeryVerifyFeature(false, violation));

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
            }
        }

        if (_next != null && context != null)
        {
            await _next(context);
        }

        await Task.Yield();
    }

    #endregion

}
