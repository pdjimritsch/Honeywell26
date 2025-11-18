using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Collections.Specialized.BitVector32;

namespace MediaPlayer.Pages;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class PostCaptureModel : PageModel
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
    [BindProperty] public List<Movie> Selection { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BindProperty] public Visitor? Visitor { get; set; }

    #endregion

    #region Constructors

    public PostCaptureModel(IHostEnvironment environment) : base()
    {
        Selection = [];
    }

    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public void OnGet()
    {
        Visitor = CurrentVisitor.Get(HttpContext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IActionResult OnPost() 
    {
        var parameters = AppGenerator.RouteParameters as IEnumerable<Movie>;

        if (parameters != null)
        {
            Selection = new(parameters);
        }

        if (Selection.Count > 0)
        {
            string title = string.Empty;

            if (Request.Form.ContainsKey("movie-preview"))
            {
                title = Request.Form["movie-preview"].ToString();

                AppGenerator.Movie = Selection.FirstOrDefault(mv => mv.Title == title);
            }
        }

        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return RedirectToPagePermanent("RecentMovie");
    }

    #endregion
}
