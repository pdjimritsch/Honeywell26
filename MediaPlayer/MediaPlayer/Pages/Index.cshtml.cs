using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;

using System.IO;

namespace MediaPlayer.Pages;

[OutputCache(Duration = 5)] public class IndexModel : PageModel
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private readonly IHostEnvironment? _environment;

    /// <summary>
    /// 
    /// </summary>
    private readonly IHostSettings? _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="environment"></param>
    public IndexModel(IHostEnvironment? environment, IHostSettings? properties) : base()
    {
        _environment = environment;

        _settings = properties;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded video
    /// </summary>
    [BindProperty] public IFormFile? UploadedVideo { get; set; }

    /// <summary>
    /// Current session visitor
    /// </summary>
    [BindProperty] public Visitor? Visitor { get; set; }

    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public void OnGet()
    {
        if (_environment != null && string.IsNullOrEmpty(AppGenerator.ContentDirectory))
        {
            AppGenerator.ContentDirectory = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        AppGenerator.IsContentAppearing = !AppGenerator.IsContentAppearing;
    }

    /// <summary>
    /// 
    /// </summary>
    public IActionResult OnPost()
    {
        if (UploadedVideo != null && !string.IsNullOrEmpty(UploadedVideo.FileName))
        {
            var reference = new FileInfo(UploadedVideo.FileName);

            AppGenerator.VideoContentLength = UploadedVideo.Length;

            AppGenerator.VideoContentType = UploadedVideo.ContentType;

            if (AppGenerator.VideoContentLength > (_settings?.MaxMovieSize ?? 0))
            {
                // The business unit had placed a maximum size for the movies to be enjoyed.

                // The same rule has been implemented within the page client-side.

                AppGenerator.IsContentAppearing = true;

                return RedirectToPagePermanent("Index");
            }

            AppGenerator.VideoTitle = reference.Name;

            AppGenerator.VideoFileName = UploadedVideo.FileName;

            AppGenerator.VideoFileExtension = reference.Extension;

            string filename;

            if (!string.IsNullOrEmpty(AppGenerator.ContentDirectory))
            {
                filename = Path.Combine(AppGenerator.ContentDirectory, "videos", AppGenerator.VideoFileName);
            }
            else
            {
                AppGenerator.ContentDirectory = AppContext.BaseDirectory;

                filename = Path.Combine(AppContext.BaseDirectory, "wwwroot/videos", AppGenerator.VideoFileName);
            }
            
            if (System.IO.File.Exists(filename))
            {
                reference = new FileInfo(filename);

                if (reference.Length != AppGenerator.VideoContentLength)
                {
                    try { System.IO.File.Delete(filename); } finally { }

                    using FileStream stream = new(filename, FileMode.Create, FileAccess.Write);

                    UploadedVideo.CopyTo(stream);

                    stream.Flush();

                    stream.Close();
                }
            }
            else
            {
                using FileStream stream = new(filename, FileMode.Create, FileAccess.Write);

                UploadedVideo.CopyTo(stream);

                stream.Flush();

                stream.Close();
            }

            Visitor = CurrentVisitor.Get(HttpContext);

            if (Visitor == null)
            {
                Visitor = CurrentVisitor.Set(HttpContext);
            }
            else
            {
                Visitor = CurrentVisitor.Set(HttpContext, Visitor);
            }

            var movie = TokenStore.Instance.Get(filename).Cast<Movie>().FirstOrDefault();

            if (movie == null)
            {
                movie = new();

                movie.AddOrRemoveMovie(Visitor, filename);
            }
            else
            {
                var visitors = movie.Visitors;

                if (visitors == null)
                {
                    movie.AddOrRemoveMovie(Visitor, filename);
                }
                else
                {
                    var count = visitors.Count(v => v.Equals(Visitor));

                    if (count == 0)
                    {
                        movie.AddOrRemoveMovie(Visitor, filename);
                    }
                }
            }

            TokenStore.Instance.Add(filename, movie);

            AppGenerator.IsContentAppearing = true;

            return RedirectToPagePermanent("RecentMovie");
        }

        AppGenerator.IsContentAppearing = true;

        return RedirectToPagePermanent("Index");
    }

    #endregion
}
