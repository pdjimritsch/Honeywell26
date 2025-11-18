using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;
using MediaPlayer.Helpers;
using MediaPlayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaPlayer.Pages;

/// <summary>
/// Exhibits previous watched movies by the visitor 
/// </summary>
public class CatalogueModel : PageModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public List<WatchedMovie> Movies { get; set; } = [];

    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public void OnGet()
    {
        GetMovies();
    }

    /// <summary>
    /// The current visitor desires to watch his or another visitor uploaded movies. 
    /// </summary>
    /// <param name="posted"></param>
    /// <returns></returns>
    public IActionResult OnPost(object? posted)
    {
        var visitor = CurrentVisitor.Get(HttpContext);

        if (visitor == null)
        {
            visitor = new Visitor { IsContentAppearing = true };
        }
        else
        {
            visitor.IsContentAppearing = true;
        }

        string? preview = Request.Form["preview"].ToString();

        if (preview == null)
        {
            // The visitor request to watch the selected movie was aborted due to technical difficulties

            AppGenerator.IsContentAppearing = true;

            return RedirectToPagePermanent("Index");
        }

        // The visitor request to watch the requested movie will be sent to te video player

        GetMovies();

        var movie = Movies.FirstOrDefault(mv => mv.Title == preview);

        if (movie == null)
        {
            // The visitor request to watch the selected movie was aborted due to technical difficulties

            AppGenerator.IsContentAppearing = true;

            return RedirectToPagePermanent("Index");
        }

        AppGenerator.VideoFileName = movie.FileName;

        AppGenerator.VideoContentLength = movie.ContentLength;

        AppGenerator.VideoContentType = movie.ContentType;

        AppGenerator.VideoTitle = movie.Title;

        AppGenerator.IsContentAppearing = true;

        var video = new Video
        {
            ContentDirectory = AppGenerator.ContentDirectory,
            ContentType = movie.ContentType,
            ContentLength = movie.ContentLength,
            FileExtension = movie.fileExtension,
            FileName = movie.FileName,
            Title = movie.Title,
        };

        AppGenerator.Movie = new(video);

        if (visitor != null)
        {
            visitor.VideoContentLength = movie.ContentLength;

            visitor.VideoContentType = movie.ContentType;

            visitor.VideoFileName = movie.FileName;

            visitor.VideoTitle = movie.Title;

            visitor.IsContentAppearing = true;
        }

        return RedirectToPagePermanent("RecentMovie");
    }

    #endregion

    #region Functions

    /// <summary>
    /// Extrapolates the witnessed movies from the data store.
    /// </summary>
    private void GetMovies()
    {
        var collection = TokenStore.Instance.Get();

        Movies = [];

        foreach (var entry in collection)
        {
            var filename = entry.Key;

            var reference = new FileInfo(filename);

            var name = reference.Name;

            var movie = entry.Value.Cast<Movie>().FirstOrDefault();

            if (!string.IsNullOrEmpty(filename) && (movie != null))
            {
                foreach (var visitor in movie.Visitors ?? Enumerable.Empty<IVisitor>())
                {
                    Movies.Add(new WatchedMovie
                    {
                        ContentDirectory = AppGenerator.ContentDirectory ?? string.Empty,
                        ContentLength = movie.ContentLength,
                        ContentType = movie.ContentType ?? string.Empty,
                        fileExtension = reference.Extension,
                        FileName = filename ?? string.Empty,
                        Title = name ?? string.Empty,
                        Visitor = visitor
                    });
                }
            }
        }
    }

    #endregion
}
