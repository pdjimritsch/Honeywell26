using MediaPlayer.Common;
using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;

namespace MediaPlayer.Helpers
{
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
        public static Visitor? Set(HttpContext ctx, Visitor? visitor = null)
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

            if (visitor == null)
            {
                visitor = new Visitor 
                { 
                    ContentDirectory = AppGenerator.ContentDirectory,
                    IsContentAppearing= true,
                    VideoContentLength = AppGenerator.VideoContentLength,
                    VideoContentType = AppGenerator.VideoContentType,
                    VideoFileExtension = AppGenerator.VideoFileExtension,
                    VideoFileName = AppGenerator.VideoFileName,
                    VideoTitle = AppGenerator.VideoTitle,
                };
            }
            else
            {
                visitor.ContentDirectory = AppGenerator.ContentDirectory;
                visitor.IsContentAppearing = true;
                visitor.VideoContentLength = AppGenerator.VideoContentLength;
                visitor.VideoContentType = AppGenerator.VideoContentType;
                visitor.VideoFileExtension = AppGenerator.VideoFileExtension;
                visitor.VideoFileName = AppGenerator.VideoFileName;
                visitor.VideoTitle = AppGenerator.VideoTitle;
            }

            ctx.Response.Cookies.Append(Visitor.Key, visitor.ToString(), options);

            return visitor;
        }

        #endregion
    }
}
