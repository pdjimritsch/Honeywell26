using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaPlayer.Data.Factory;
using Abstraction;

[ImmutableObject(true)] public sealed partial class Visitor : IVisitor
{
    #region Shared Properties and Services

    /// <summary>
    /// 
    /// </summary>
    public const string Key = nameof(Visitor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static Visitor? Parse(string? content)
    {
        Visitor? visitor = default;

        if (!string.IsNullOrEmpty(content))
        {
            JsonSerializerOptions options = new()
            {
                AllowTrailingCommas = false,
                IncludeFields = false,
                MaxDepth = int.MaxValue,
            };

            try
            {
                visitor = JsonSerializer.Deserialize<Visitor>(content, options);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message);
                }

                visitor = default;
            }
        }

        return visitor;
    }

    #endregion

    #region IVisitor Members

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(0)]
    [JsonPropertyName(nameof(ContentDirectory))]
    public string? ContentDirectory { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(1)]
    [JsonPropertyName(nameof(IsContentAppearing))]
    [JsonRequired]
    public bool IsContentAppearing { get; set; } = true;

    [JsonInclude]
    [JsonPropertyOrder(2)]
    [JsonPropertyName(nameof(MessageIndex))]
    [JsonRequired]
    public int MessageIndex { get; set; } = -1;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(3)]
    [JsonPropertyName(nameof(Token))]
    [JsonRequired]
    public Guid Token { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(4)]
    [JsonPropertyName(nameof(VideoContentType))]
    public string? VideoContentType { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(5)]
    [JsonPropertyName(nameof(VideoContentLength))]
    [JsonRequired]
    public long VideoContentLength { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(6)]
    [JsonPropertyName(nameof(VideoFileExtension))]
    public string? VideoFileExtension { get; set; } = null!;

    /// <summary>
    /// Video filename with file extension
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(7)]
    [JsonPropertyName(nameof(VideoFileName))]
    public string? VideoFileName { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(8)]
    [JsonPropertyName(nameof(VideoTitle))]
    public string? VideoTitle { get; set; } = null!;

    #endregion

    #region Overrides

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        var visitor = obj as Visitor;

        return (visitor != null) && (visitor.Token == Token);
    }

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
    /// <param name="visitor"></param>
    /// <returns></returns>
    private string GetProperties(Visitor visitor)
    {
        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = false,
            IncludeFields = false,
            MaxDepth = int.MaxValue,
        };

        return JsonSerializer.Serialize(visitor, options);
    }

    #endregion
}
