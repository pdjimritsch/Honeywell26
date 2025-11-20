using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediaPlayer.Data.Factory;

using Abstraction;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

/// <summary>
/// 
/// </summary>
[ImmutableObject(true)] public sealed partial class RouteParameters : 
    IRouteParameters, IEquatable<IRouteParameters>, IEqualityComparer<IRouteParameters>
{
    #region Shared Properties and Services

    /// <summary>
    /// 
    /// </summary>
    public const string Key = nameof(RouteParameters);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static RouteParameters? Parse(string? content)
    {
        RouteParameters? parameters = default;

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
                parameters = JsonSerializer.Deserialize<RouteParameters>(content, options);
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
    [JsonPropertyOrder(1)]
    [JsonPropertyName(nameof(Movies))]
    [Required]
    public List<Movie> Movies { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    [JsonInclude]
    [JsonPropertyOrder(0)]
    [JsonPropertyName(nameof(Token))]
    [Required]
    public Guid Token { get; set; } = Guid.NewGuid();

    #endregion

    #region IEquatable<IRouteParameters> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IRouteParameters? other) => IsEqualTo(this, other);

    #endregion

    #region IEqualityComparer<IRouteParameters> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(IRouteParameters? x, IRouteParameters? y) => IsEqualTo(x, y);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode([DisallowNull] IRouteParameters obj) => obj.Token.GetHashCode();

    #endregion

    #region Overrides

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => IsEqualTo(this, obj as IRouteParameters);

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
    private static bool IsEqualTo(IRouteParameters? x,  IRouteParameters? y)
    {
        if ((x != null) && (y != null))
        {
            return x.Token.ToString().Equals(y.Token.ToString(), StringComparison.Ordinal);
        }

        return (x == null) && (y == null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static string GetProperties(IRouteParameters parameters)
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
