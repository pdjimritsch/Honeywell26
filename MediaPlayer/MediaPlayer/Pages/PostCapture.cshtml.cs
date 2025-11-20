using MediaPlayer.Data.Factory;
using MediaPlayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

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
        var sequence = HttpContext.Session.Get(RouteParameters.Key);

        var parameters = RouteParameters.Parse(Encoding.UTF8.GetString(sequence ?? []));

        if (parameters != null)
        {
            Selection = [];

            Selection.AddRange(parameters.Movies);
        }

        if (Selection.Count > 0)
        {
            string title = string.Empty;

            if (Request.Form.ContainsKey("movie-preview"))
            {
                title = Request.Form["movie-preview"].ToString();

                var preview = Selection.FirstOrDefault(mv => mv.Title == title);

                if (preview != null)
                {
                    var bytes = Encoding.UTF8.GetBytes(preview.ToString().ToCharArray());

                    HttpContext.Session.Set(Movie.Key, bytes);
                }
            }
        }

        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return RedirectToPagePermanent("RecentMovie");
    }

    #endregion
}
