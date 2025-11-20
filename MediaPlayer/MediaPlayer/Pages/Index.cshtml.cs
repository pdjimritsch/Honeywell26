using MediaPlayer.Configuration.Abstraction;
using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using System.Text;

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

        UploadedVideos = [];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Uploaded video
    /// </summary>
    [BindProperty] public List<IFormFile> UploadedVideos { get; set; }

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

        Visitor = CurrentVisitor.Get(HttpContext);

        if (Request.Headers.ContainsKey("Referer"))
        {
            var previousPage = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("RecentMovie"))
            {
                // reset the form for the visitor to continue with movie upload 

                Visitor?.IsContentAppearing = false;

                Visitor?.MessageIndex = 0;
            }
            else
            {
                // display the controls for the visitor to upload the videos.

                Visitor?.IsContentAppearing = true;

                Visitor?.MessageIndex = 1;
            }
        }
        else
        {
            Visitor?.IsContentAppearing = !(Visitor?.IsContentAppearing ?? false);

            if (Visitor?.IsContentAppearing ?? false)
            {
                // display the controls for the visitor to upload the videos.

                Visitor?.IsContentAppearing = !(Visitor?.IsContentAppearing ?? false);

                Visitor?.MessageIndex = 1;
            }
            else
            {
                // reset the form for the visitor to continue with movie upload 

                Visitor?.MessageIndex = 0;
            }
        }

        CurrentVisitor.Set(HttpContext, null, Visitor);
    }

    /// <summary>
    /// 
    /// </summary>
    public IActionResult OnPost()
    {
        var videos = UploadedVideos;

        var approved = videos.Count(v => v.Length <= (_settings?.MaxMovieSize ?? 0));

        if (approved == 0)
        {
            // The business unit had placed a maximum size for the movies to be enjoyed.

            // The same rule has been implemented within the page client-side.

            Visitor?.IsContentAppearing = true;

            Visitor?.MessageIndex = 0;

            return RedirectToPagePermanent("Index");
        }

        // Get the current session visitor

        Visitor = CurrentVisitor.Get(HttpContext);

        if ((Visitor == null) && (videos.Count == 1))
        {
            // Assign the selected video for the current visitor

            Visitor = CurrentVisitor.Set(HttpContext, videos[0]);
        }
        else if ((Visitor == null) && (videos.Count > 1))
        {
            // Create the visitor from the current session context

            Visitor = CurrentVisitor.Set(HttpContext, null);
        }
        else if ((Visitor != null) && (videos.Count == 1))
        {
            // Assign the selected video for the current visitor

            Visitor = CurrentVisitor.Set(HttpContext, videos[0], Visitor);
        }

        List<Movie> movies = [];

        int count = 0;

        foreach (var video in videos)
        {
            if (video.Length > (_settings?.MaxMovieSize ?? 0))
            {
                // Large sized videos will be ignored

                continue;
            }

            // Save the imported video

            var filename = AppGenerator.SaveVideo(video);

            var movie = TokenStore.Instance.Get(filename).Cast<Movie>().FirstOrDefault();

            var reference = new FileInfo(video.FileName);

            if (movie == null)
            {
                movie = new Movie();

                Video presentation = new()
                {
                    ContentDirectory = AppGenerator.ContentDirectory,
                    ContentLength = video.Length,
                    ContentType = video.ContentType,
                    FileExtension = reference.Extension,
                    FileName = video.FileName,
                    Title = reference.Name,
                };

                movie.AddOrRemoveMovie(Visitor, presentation, filename);
            }
            else
            {
                var visitors = movie.Visitors;

                if (visitors == null)
                {
                    // Save the visitor's reference to the selected video

                    Video presentation = new()
                    {
                        ContentDirectory = AppGenerator.ContentDirectory,
                        ContentLength = video.Length,
                        ContentType = video.ContentType,
                        FileExtension = reference.Extension,
                        FileName = video.FileName,
                        Title = reference.Name,
                    };


                    movie.AddOrRemoveMovie(Visitor, presentation, filename);
                }
                else
                {
                    count = visitors.Count(v => v.Equals(Visitor));

                    if (count == 0)
                    {
                        // Save the visitor's reference to the selected video

                        Video presentation = new()
                        {
                            ContentDirectory = AppGenerator.ContentDirectory,
                            ContentLength = video.Length,
                            ContentType = video.ContentType,
                            FileExtension = reference.Extension,
                            FileName = video.FileName,
                            Title = reference.Name,
                        };

                        movie.AddOrRemoveMovie(Visitor, presentation, filename);
                    }
                }
            }

            movies.Add(movie);
        }

        count = movies.Count;

        foreach (var movie in movies)
        {
            if (!string.IsNullOrEmpty(movie.FileName))
            {
                TokenStore.Instance.Add(movie.FileName, movie);
            }

            if (count == 1)
            {
                // Save the visitor's reference to the selected video

                HttpContext.Session.Set(nameof(Movie), Encoding.UTF8.GetBytes(movie.ToString().ToCharArray()));

                Visitor?.IsContentAppearing = true;

                return RedirectToPagePermanent("RecentMovie");
            }
        }

        if (count > 0)
        {
            Visitor?.IsContentAppearing = true;

            RouteParameters parameters = new();

            parameters.Movies.AddRange(movies);

            HttpContext.Session.Remove(nameof(Movie)); // Multiple movies have been selected

            HttpContext.Session.Set(RouteParameters.Key, Encoding.UTF8.GetBytes(parameters.ToString().ToCharArray()));

            return RedirectToPagePermanent("RecentMovie");
        }

        Visitor?.MessageIndex = 0;

        Visitor?.IsContentAppearing = true;

        CurrentVisitor.Set(HttpContext, null, Visitor);

        return RedirectToPagePermanent("Index");
    }

    #endregion
}
