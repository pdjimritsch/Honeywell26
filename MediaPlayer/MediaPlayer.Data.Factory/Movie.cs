using System.Diagnostics.CodeAnalysis;

namespace MediaPlayer.Data.Factory;

using Abstraction;
using System.Security.Cryptography.X509Certificates;
using System.Text;

/// <summary>
/// Visited movie
/// </summary>
public partial class Movie : IMovie, IEquatable<IMovie>, IEqualityComparer<IMovie>
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const string Key = nameof(Movie);

    /// <summary>
    /// 
    /// </summary>
    private readonly List<IVisitor> _visitors;

    #endregion

    #region Constructors

    public Movie() : base() => _visitors = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="video"></param>
    public Movie(IVideo? video) : this()
    {
        Initialize(video);
    }

    #endregion

    #region IMovie Members

    /// <summary>
    /// 
    /// </summary>
    public string? ContentDirectory { get; private set; } = null!;

    /// <summary>
    /// Movie size in bytes
    /// </summary>
    public long ContentLength { get; private set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public string? ContentType { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? FileExtension { get; private set; } = null!;

    /// <summary>
    /// Movie filename reference
    /// </summary>
    public string? FileName { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? Title { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Guid Token { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<IVisitor> Visitors => _visitors;

    /// <summary>
    /// Registers or unregisters the viewed movie.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="filename"></param>
    /// <param name="unregister"></param>
    /// <returns></returns>
    public bool AddOrRemoveMovie(IVisitor? visitor, IVideo? video, string? filename, bool unregister = false)
    {
        if (unregister)
        {
            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(filename) && 
                FileName.Equals(filename, StringComparison.OrdinalIgnoreCase) &&
                (visitor != null))
            {
                var any = _visitors.Any(v => v.Token == visitor.Token);

                if (any) _visitors.Remove(visitor);

                return any;
            }
        }
        else if (string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(filename) && 
            (visitor != null) && (video != null))
        {
            if (File.Exists(filename))
            {
                ContentDirectory = video.ContentDirectory;

                FileExtension = video.FileExtension;

                FileName = filename;

                ContentLength = video.ContentLength;

                ContentType = video.ContentType;

                Title = video.Title;

                _visitors.Add(visitor);

                return true;
            }
        }
        else if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(filename) && 
            (visitor != null) && (video != null))
        {
            if (FileName.Equals(filename, StringComparison.OrdinalIgnoreCase))
            {
                ContentDirectory = video.ContentDirectory;

                ContentLength = video.ContentLength;

                ContentType = video.ContentType;

                FileExtension = video.FileExtension;

                Title = video.Title;

                var any = _visitors.Exists(v => v.Token == visitor.Token);

                if (!any)
                {
                    _visitors.Add(visitor);
                };

                return !any;
            }
        }

        return false;
    }

    #endregion

    #region IEquatable<IMovie> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IMovie? other) => IsEqualTo(this, other);

    #endregion

    #region IEqualityComparer<IMovie> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(IMovie? x, IMovie? y) => IsEqualTo(x, y);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode([DisallowNull] IMovie obj) => obj.Token.GetHashCode();

    #endregion

    #region Overrides

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Token.GetHashCode();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => IsEqualTo(this, obj as IMovie);

    #endregion

    #region Internal Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static bool IsEqualTo(IMovie? x, IMovie? y)
    {
        var succeded = false;

        if (x != null && y != null)
        {
            succeded = (x.FileName ?? string.Empty).Equals(y.FileName ?? string.Empty, StringComparison.Ordinal) &&
                 x.ContentLength == y.ContentLength &&
                 (x.ContentType ?? string.Empty).Equals(y.ContentType ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
        else if (x == null && y == null)
        {
            succeded = true;
        }

        return succeded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="video"></param>
    private void Initialize(IVideo? video)
    {
        if (video != null)
        {
            FileExtension = video.FileExtension;

            FileName = video.FileName;

            ContentLength = video.ContentLength;

            ContentType = video.ContentType;

            Title = video.Title;
        }
    }

    #endregion
}
