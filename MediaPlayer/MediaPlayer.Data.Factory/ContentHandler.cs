using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaPlayer.Data.Factory;

using Abstraction;

[ImmutableObject(true)] 
public sealed partial class ContentHandler : 
    IContentHandler, IEquatable<IContentHandler>, IEqualityComparer<IContentHandler>
{
    #region Shared Properties and Services

    /// <summary>
    /// 
    /// </summary>
    public const string Key = nameof(ContentHandler);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static ContentHandler? Parse(string? content)
    {
        ContentHandler? parameters = default;

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
                parameters = JsonSerializer.Deserialize<ContentHandler>(content, options);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message);
                }

                parameters = default;
            }
        }

        return parameters;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(0)]
    [JsonPropertyName(nameof(IsContentAppearing))]
    [Required]
    public bool IsContentAppearing { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(1)]
    [JsonPropertyName(nameof(MessageIndex))]
    [Required]
    public int MessageIndex { get; set; }

    #endregion

    #region IEquatable<IContentHandler> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IContentHandler? other) => IsEqualTo(this, other);

    #endregion

    #region IEqualityComparer<IContentHandler> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(IContentHandler? x, IContentHandler? y) => IsEqualTo(x, y);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode([DisallowNull] IContentHandler obj) => obj.GetHashCode();

    #endregion

    #region Overrides

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => IsEqualTo(this, obj as IContentHandler);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => base.GetHashCode();

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
    private static bool IsEqualTo(IContentHandler? x, IContentHandler? y)
    {
        if ((x != null) && (y != null))
        {
            return (x.IsContentAppearing == y.IsContentAppearing) &&
                (x.MessageIndex == y.MessageIndex);
        }

        return (x == null) && (y == null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static string GetProperties(IContentHandler parameters)
    {
        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = false,
            IncludeFields = false,
            MaxDepth = int.MaxValue,
        };

        return JsonSerializer.Serialize(parameters, options);
    }

    #endregion
}
