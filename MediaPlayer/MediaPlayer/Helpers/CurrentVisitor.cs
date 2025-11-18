using MediaPlayer.Data.Factory;

namespace MediaPlayer.Helpers;

/// <summary>
/// 
/// </summary>
public static partial class CurrentVisitor
{
    #region Functions

    /// <summary>
    /// Gets the current context session visitor
    /// </summary>
    /// <param name="ctx">
    /// Http context
    /// </param>
    /// <returns>
    /// Current context session visitor
    /// </returns>
    public static Visitor? Get(HttpContext ctx)
    {
        Visitor? visitor = default;

        if (ctx.Request.Cookies.ContainsKey(Visitor.Key))
        {
            var content = ctx.Request.Cookies[Visitor.Key];

            visitor = Visitor.Parse(content);
        }

        if (visitor != null)
        {
            visitor.IsContentAppearing = true;
        }

        return visitor;
    }

    /// <summary>
    /// Updates the profile for the current context session visitor
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public static Visitor? Set(HttpContext ctx, IFormFile? video, Visitor? visitor = null)
    {
        if (ctx.Request.Cookies.ContainsKey(Visitor.Key) && (visitor == null))
        {
            visitor = Visitor.Parse(ctx.Request.Cookies[Visitor.Key]?.ToString() ?? string.Empty);
        }

        if (ctx.Request.Cookies.ContainsKey(Visitor.Key))
        {
            ctx.Response.Cookies.Delete(Visitor.Key);
        }

        var options = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddHours(4),
            HttpOnly = true,
            MaxAge = TimeSpan.FromHours(4)
        };

        if (visitor == null && video != null)
        {
            var reference = new FileInfo(video.FileName);

            visitor = new Visitor 
            { 
                ContentDirectory = AppGenerator.ContentDirectory,
                IsContentAppearing= true,
                VideoContentLength = video.Length,
                VideoContentType = video.ContentType,
                VideoFileExtension = reference.Extension,
                VideoFileName = video.FileName,
                VideoTitle = reference.Name
            };
        }
        else if ((visitor == null) && (video == null))
        {
            visitor = new Visitor
            {
                ContentDirectory = AppGenerator.ContentDirectory,
                IsContentAppearing = true
            };
        }
        else if (visitor != null && video != null)
        {
            var reference = new FileInfo(video.FileName);

            visitor.ContentDirectory = AppGenerator.ContentDirectory;
            visitor.IsContentAppearing = true;
            visitor.VideoContentLength = video.Length;
            visitor.VideoContentType = video.ContentType;
            visitor.VideoFileExtension = reference.Extension;
            visitor.VideoFileName = video.FileName;
            visitor.VideoTitle = reference.Name;
        }

        if (visitor != null)
        {
            ctx.Response.Cookies.Append(Visitor.Key, visitor.ToString(), options);
        }

        return visitor;
    }

    #endregion
}