using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaPlayer.Pages;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class RecentMovieModel : PageModel
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private readonly IHostEnvironment? _environment;

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Movie? Movie { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public bool PlayVideo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public List<Movie> Selection { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Visitor? Visitor { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="environment"></param>
    public RecentMovieModel(IHostEnvironment environment) : base()
    {
        _environment = environment;

        Movie = null;

        PlayVideo = false;

        Selection = [];

        Visitor = null;
    }

    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public void OnGet()
    {
        if (Request.Headers.ContainsKey("Referer"))
        {
            var previousPage = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Index"))
            {
                AppGenerator.IsContentAppearing = true;
            }
            else if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Catalogue"))
            {
                AppGenerator.IsContentAppearing = true;
            }
        }

        Movie = AppGenerator.Movie;

        PlayVideo = (Movie != null);

        var parameters = AppGenerator.RouteParameters as IEnumerable<Movie>;

        if (parameters != null)
        {
            Selection = new(parameters);
        }

        AppGenerator.MessageIndex = 2;

        Visitor = CurrentVisitor.Get(HttpContext);
    }

    #endregion
}
