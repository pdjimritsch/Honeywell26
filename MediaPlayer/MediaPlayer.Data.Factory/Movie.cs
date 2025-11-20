using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaPlayer.Data.Factory;

using Abstraction;

/// <summary>
/// Visited movie
/// </summary>
[ImmutableObject(true)] 
public sealed partial class Movie : IMovie, IEquatable<IMovie>, IEqualityComparer<IMovie>
{
    #region Shared Services

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static Movie? Parse(string? content)
    {
        Movie? movie = default;

        if (!string.IsNullOrWhiteSpace(content))
        {
            JsonSerializerOptions options = new()
            {
                AllowTrailingCommas = false,
                IncludeFields = false,
                MaxDepth = int.MaxValue,
            };

            try
            {
                movie = JsonSerializer.Deserialize<Movie>(content, options);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message);
                }

                movie = default;
            }
        }

        return movie;
    }

    #endregion

    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const string Key = nameof(Movie);

    /// <summary>
    /// 
    /// </summary>
    private readonly List<Visitor> _visitors;

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
    [JsonInclude]
    [JsonPropertyOrder(0)]
    [JsonPropertyName(nameof(ContentDirectory))]
    public string? ContentDirectory { get; private set; } = null!;

    /// <summary>
    /// Movie size in bytes
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(1)]
    [JsonPropertyName(nameof(ContentLength))]
    [JsonRequired]
    public long ContentLength { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(2)]
    [JsonPropertyName(nameof(ContentType))]
    public string? ContentType { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(3)]
    [JsonPropertyName(nameof(FileExtension))]
    public string? FileExtension { get; set; } = null!;

    /// <summary>
    /// Movie filename reference
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(4)]
    [JsonPropertyName(nameof(FileName))]
    public string? FileName { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(5)]
    [JsonPropertyName(nameof(Title))]
    public string? Title { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(6)]
    [JsonPropertyName(nameof(Token))]
    [Required]
    public Guid Token { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(7)]
    [JsonPropertyName(nameof(Visitors))]
    [Required]
    public List<Visitor> Visitors {  get => _visitors; set => _visitors.AddRange(value); }

    /// <summary>
    /// Registers or unregisters the viewed movie.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="filename"></param>
    /// <param name="unregister"></param>
    /// <returns></returns>
    public bool AddOrRemoveMovie(Visitor? visitor, IVideo? video, string? filename, bool unregister = false)
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
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => IsEqualTo(this, obj as IMovie);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Token.GetHashCode();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => GetProperties(this);

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
    /// <param name="movie"></param>
    /// <returns></returns>
    private static string GetProperties(Movie movie)
    {
        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = false,
            IncludeFields = false,
            MaxDepth = int.MaxValue,
        };

        return JsonSerializer.Serialize(movie, options);
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
