using System.Text;

namespace MediaPlayer.Middleware;

public static partial class MiddlewareError
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="violation"></param>
    /// <param name="logger"></param>
    public static void SetError(HttpContext? context, Exception? violation, ILogger? logger)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"DateTime: {DateTime.UtcNow}");

        if (context != null)
        {
            builder.AppendLine($"StatusCode: {context.Response.StatusCode}. Request: {context.Request.Method}. Destination: {context.Request.Path.Value ?? string.Empty}");
        }

        if (violation != null)
        {
            builder.AppendLine($"Error: {violation.Message}");

            builder.AppendLine($"StackTrace:\n{violation.StackTrace}");
        }

        if (logger != null)
        {
            logger.LogError(builder.ToString());
        }
    }

    #endregion
}
