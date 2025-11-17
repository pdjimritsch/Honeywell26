using Microsoft.Extensions.Primitives;

using System.Net;

namespace MediaPlayer.Middleware;

/// <summary>
/// 
/// </summary>
public partial class ApiKeyMiddleware
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private readonly RequestDelegate? _next;

    /// <summary>
    /// 
    /// </summary>
    private readonly ILogger? _logger;

    /// <summary>
    /// 
    /// </summary>
    private const string AUTHORIZATION_HEADER_NAME = "Authorization";

    /// <summary>
    /// 
    /// </summary>
    private const string AUTHENTICATION_HEADER_NAME = "Authentication";

    /// <summary>
    /// 
    /// </summary>
    private const string AUTHORIZATION_BEARER_NAME = "Bearer";

    /// <summary>
    /// 
    /// </summary>
    private const string HEADER_KEY_NAME = "X-XSRF-TOKEN";

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public ApiKeyMiddleware(RequestDelegate? next, ILogger? logger) : base()
    {
        _next = next;

        _logger = logger;
    }

    #endregion

    #region Functions

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
                if (context.Request.Headers.ContainsKey(HEADER_KEY_NAME) &&
                    context.Request.Headers.TryGetValue(HEADER_KEY_NAME, out var tokens) && (tokens.Count > 0))
                {
                    await AuthenticateTokenAsync(context, tokens);
                }
                else if (context.Request.Headers.ContainsKey(AUTHORIZATION_HEADER_NAME) &&
                        context.Request.Headers.Authorization.Any(x => x?.StartsWith(AUTHORIZATION_BEARER_NAME) ?? false))
                {
                    await AuthorizeBearerTokenAsync(context, context.Request.Headers.Authorization);
                }
                else if (context.Request.Headers.ContainsKey(AUTHENTICATION_HEADER_NAME))
                {
                    await AuthenticateBearerTokenAsync(context);
                }
                else
                {
                    if (!context.Request.Headers.ContainsKey(HEADER_KEY_NAME) && !context.Session.IsAvailable)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401

                        await context.Response.WriteAsync($"Status Code: {context.Response.StatusCode}. The API request was not authorized to be processed.");
                    }
                    else if ((context.Request.Headers[HEADER_KEY_NAME].Count == 0) && !context.Session.IsAvailable)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.FailedDependency; // 424

                        await context.Response.WriteAsync($"Status Code: {context.Response.StatusCode}. The API request was not authorized to be processed.");
                    }
                    else if (context.Session.IsAvailable)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                }
            }
        }
        catch (Exception violation)
        {
            MiddlewareError.SetError(context, violation, _logger);
        }
        finally
        {
            if (context != null && _next != null)
            {
                await _next(context);
            }
        }

        await Task.Yield();
    }

    #endregion

    #region Internal Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task AuthenticateBearerTokenAsync(HttpContext context)
    {
        await Task.Run(async () => {

            if (_next != null) await _next(context);
        });

        await Task.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    private async Task AuthenticateTokenAsync(HttpContext context, StringValues? values)
    {
        await Task.Run(async () =>
        {
            if (_next != null) await _next(context);

            await Task.Yield();
        });

        await Task.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    private async Task AuthorizeBearerTokenAsync(HttpContext context, StringValues? values)
    {
        await Task.Run(async () =>
        {
            if (_next != null) await _next(context);

            await Task.Yield();
        });

        await Task.Yield();
    }

    #endregion
}
