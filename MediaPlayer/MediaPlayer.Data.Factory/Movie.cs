using System.Diagnostics.CodeAnalysis;

namespace MediaPlayer.Data.Factory;

using Abstraction;

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

    #endregion

    #region IMovie Members

    /// <summary>
    /// Movie size in bytes
    /// </summary>
    public long ContentLength { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ContentType { get; private set; }

    /// <summary>
    /// Movie filename reference
    /// </summary>
    public string? FileName { get; private set; }

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
    public bool AddOrRemoveMovie(IVisitor? visitor, string? filename, bool unregister = false)
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
        else if (string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(filename) && (visitor != null))
        {
            if (File.Exists(filename))
            {
                FileName = filename;

                ContentLength = visitor.VideoContentLength;

                ContentType = visitor.VideoContentType;

                _visitors.Add(visitor);

                return true;
            }
        }
        else if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(filename) && (visitor != null))
        {
            if (FileName.Equals(filename, StringComparison.OrdinalIgnoreCase))
            {
                ContentLength = visitor.VideoContentLength;

                ContentType = visitor.VideoContentType;

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

    #endregion
}
