using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

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
        Visitor = CurrentVisitor.Get(HttpContext);

        if (Request.Headers.ContainsKey("Referer"))
        {
            var previousPage = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Index"))
            {
                Visitor?.IsContentAppearing = true;
            }
            else if (!string.IsNullOrEmpty(previousPage) && previousPage.Contains("Catalogue"))
            {
                Visitor?.IsContentAppearing = true;
            }
        }

        Movie = Movie.Parse(HttpContext.Session.GetString(nameof(Movie)));

        PlayVideo = (Movie != null);

        var sequence = HttpContext.Session.Get(RouteParameters.Key);

        var parameters = RouteParameters.Parse(Encoding.UTF8.GetString(sequence ?? []));

        if (parameters != null)
        {
            Selection = parameters?.Movies ?? [];
        }

        Visitor?.MessageIndex = 2;

        CurrentVisitor.Set(HttpContext, null, Visitor);
    }

    #endregion
}
