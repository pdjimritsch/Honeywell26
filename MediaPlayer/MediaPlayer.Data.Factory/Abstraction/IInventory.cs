using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace MediaPlayer.Data.Factory.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface IInventory
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<object> Keys { get; }

    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add(object? key, object? value, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add(object? key, object? value, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add(KeyValuePair<object?, object?>? pair, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add<K, V>(K? key, V? value, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add<K, V>(K? key, V? value, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add<K, V>(KeyValuePair<K?, V?>? pair, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    bool Add<K, V>(KeyValuePair<K?, V?>? pair, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(object? key, object? value, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync(object? key, object? value, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync<K, V>(K? key, V? value, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync<K, V>(K? key, V? value, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync<K, V>(KeyValuePair<K?, V?>? pair, TimeSpan? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    ValueTask<bool> AddAsync<K, V>(KeyValuePair<K?, V?>? pair, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    void Clear();

    /// <summary>
    /// 
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Remove(object? key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ValueTask<bool> RemoveAsync(object? key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="expiration"></param>
    void SetOptions(IOptions<MemoryCacheOptions> options, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    Task SetOptionsAsync(IOptions<MemoryCacheOptions> options, DateTimeOffset? expiration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void TryGetValue(object? key, out object? value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    Task<object?> TryGetValueAsync(object? key);

    #endregion
}
