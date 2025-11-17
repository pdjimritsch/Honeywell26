using System.Collections.Concurrent;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

namespace MediaPlayer.Data.Factory;
using Abstraction;

/// <summary>
/// 
/// </summary>
public sealed partial class Inventory : IInventory, IAsyncDisposable, IDisposable
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private static Inventory? _instance;

    /// <summary>
    /// 
    /// </summary>
    private MemoryCache? _repository;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="factory"></param>
    private Inventory(IOptions<MemoryCacheOptions> options) : base()
    {
        _repository = new MemoryCache(options);
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public static Inventory? GetInstance(IOptions<MemoryCacheOptions> options) => _instance ??= new Inventory(options);

    #endregion

    #region IInventory Members

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<object> Keys => _repository?.Keys ?? [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(object? key, object? value, TimeSpan? expiration)
    {
        if (key == null) return false;

        if (value == null) return false;

        if (expiration == null && _repository != null)
        {
            return _repository.Set(key, value) != null;
        }
        else if (_repository != null && expiration != null)
        {
            return _repository.Set(key, value, expiration.Value) != null;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(object? key, object? value, DateTimeOffset? expiration)
    {
        if (key == null) return false;

        if (value == null) return false;

        if (expiration == null && _repository != null)
        {
            return _repository.Set(key, value) != null;
        }
        else if (_repository != null && expiration != null)
        {
            return _repository.Set(key, value, expiration.Value) != null;
        }

        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(KeyValuePair<object?, object?>? pair, TimeSpan? expiration)
    {
        if (pair == null) return false;

        return Add(pair.Value.Key, pair.Value.Value, expiration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add<K, V>(K? key, V? value, TimeSpan? expiration)
    {
        if (key == null) return false;

        if (value == null) return false;

        if (expiration == null && _repository != null)
        {
            return _repository.Set(key, value) != null;
        }
        else if (_repository != null && expiration != null)
        {
            return _repository.Set(key, value, expiration.Value) != null;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add<K, V>(K? key, V? value, DateTimeOffset? expiration)
    {
        if (key == null) return false;

        if (value == null) return false;

        if (expiration == null && _repository != null)
        {
            return _repository.Set(key, value) != null;
        }
        else if (_repository != null && expiration != null)
        {
            return _repository.Set(key, value, expiration.Value) != null;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="pair"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public bool Add<K, V>(KeyValuePair<K?, V?>? pair, TimeSpan? expiration)
    {
        if (pair == null) return false;

        return Add(pair.Value.Key, pair.Value.Value, expiration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="pair"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public bool Add<K, V>(KeyValuePair<K?, V?>? pair, DateTimeOffset? expiration)
    {
        if (pair == null) return false;

        return Add(pair.Value.Key, pair.Value.Value, expiration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync(object? key, object? value, TimeSpan? expiration)
    {
        if (key == null) return await ValueTask.FromResult(false);

        if (value == null) return await ValueTask.FromResult(false);

        if (expiration == null && _repository != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value) != null);
        }
        else if (_repository != null && expiration != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value, expiration.Value) != null);
        }

        return await ValueTask.FromResult(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync(object? key, object? value, DateTimeOffset? expiration)
    {
        if (key == null) return await ValueTask.FromResult(false);

        if (value == null) return await ValueTask.FromResult(false);

        if (expiration == null && _repository != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value) != null);
        }
        else if (_repository != null && expiration != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value, expiration.Value) != null);
        }

        return await ValueTask.FromResult(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync<K, V>(K? key, V? value, TimeSpan? expiration)
    {
        if (key == null) return await ValueTask.FromResult(false);

        if (value == null) return await ValueTask.FromResult(false);

        if (expiration == null && _repository != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value) != null);
        }
        else if (_repository != null && expiration != null)
        {
            return await Task.FromResult(_repository.Set(key, value, expiration.Value) != null);
        }

        return await ValueTask.FromResult(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync<K, V>(K? key, V? value, DateTimeOffset? expiration)
    {
        if (key == null) return await ValueTask.FromResult(false);

        if (value == null) return await ValueTask.FromResult(false);

        if (expiration == null && _repository != null)
        {
            return await ValueTask.FromResult(_repository.Set(key, value) != null);
        }
        else if (_repository != null && expiration != null)
        {
            return await Task.FromResult(_repository.Set(key, value, expiration.Value) != null);
        }

        return await ValueTask.FromResult(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync<K, V>(KeyValuePair<K?, V?>? pair, TimeSpan? expiration)
    {
        if (pair == null) return await ValueTask.FromResult(false);

        return await AddAsync(pair.Value.Key, pair.Value.Value, expiration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync<K, V>(KeyValuePair<K?, V?>? pair, DateTimeOffset? expiration)
    {
        if (pair == null) return await ValueTask.FromResult(false);

        return await AddAsync(pair.Value.Key, pair.Value.Value, expiration);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        _repository?.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task ClearAsync()
    {
        _repository?.Clear();

        await Task.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(object? key)
    {
        if (_repository != null && key != null)
        {
            _repository.Remove(key);

            return !_repository.TryGetValue(key, out object? value) || (value == null);
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async ValueTask<bool> RemoveAsync(object? key)
    {
        if (_repository != null && key != null)
        {
            _repository.Remove(key);

            return await ValueTask.FromResult(!_repository.TryGetValue(key, out object? value) || (value == null));
        }

        return await ValueTask.FromResult(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="expiration"></param>
    public void SetOptions(IOptions<MemoryCacheOptions> options, DateTimeOffset? expiration)
    {
        if ((options != null) && _repository?.GetCurrentStatistics() == null)
        {
            _repository = new MemoryCache(options);
        }
        else if (_repository != null && options != null)
        {
            var statistics = _repository.GetCurrentStatistics();

            if (((statistics?.CurrentEntryCount ?? 0) == 0) || (_repository.Count == 0))
            {
                _repository = new MemoryCache(options);
            }
            else if (_repository != null)
            {
                var keys = _repository.Keys;

                if (keys.Any())
                {
                    var repository = new ConcurrentDictionary<object, object>();

                    foreach (var key in keys)
                    {
                        if ((key != null) && (_repository?.TryGetValue(key, out var container) ?? false) && (container != null))
                        {
                            repository.TryAdd(key, container);
                        }
                    }

                    if (repository.IsEmpty)
                    {
                        _repository = new MemoryCache(options);
                    }
                    else
                    {
                        _repository?.Clear();

                        _repository = new MemoryCache(options);

                        foreach (var kv in repository)
                        {
                            if (expiration != null)
                            {
                                _repository.Set(kv.Key, kv.Value, expiration.Value);
                            }
                            else
                            {
                                _repository.Set(kv.Key, kv.Value);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="expiration"></param>
    public async Task SetOptionsAsync(IOptions<MemoryCacheOptions> options, DateTimeOffset? expiration)
    {
        if ((options != null) && _repository?.GetCurrentStatistics() == null)
        {
            _repository = new MemoryCache(options);
        }
        else if (_repository != null && options != null)
        {
            var statistics = _repository.GetCurrentStatistics();

            if (((statistics?.CurrentEntryCount ?? 0) == 0) || (_repository.Count == 0))
            {
                _repository = new MemoryCache(options);
            }
            else if (_repository != null)
            {
                var keys = _repository.Keys;

                if (keys.Any())
                {
                    var repository = new ConcurrentDictionary<object, object>();

                    foreach (var key in keys)
                    {
                        if ((key != null) && (_repository?.TryGetValue(key, out var container) ?? false) && (container != null))
                        {
                            repository.TryAdd(key, container);
                        }
                    }

                    if (repository.IsEmpty)
                    {
                        _repository = new MemoryCache(options);
                    }
                    else
                    {
                        _repository?.Clear();

                        _repository = new MemoryCache(options);

                        foreach (var kv in repository)
                        {
                            if (expiration != null)
                            {
                                _repository.Set(kv.Key, kv.Value, expiration.Value);
                            }
                            else
                            {
                                _repository.Set(kv.Key, kv.Value);
                            }
                        }
                    }
                }
            }
        }

        await Task.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void TryGetValue(object? key, out object? value)
    {
        value = null;

        if (_repository != null && key != null)
        {
            _repository.TryGetValue(key, out value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public async Task<object?> TryGetValueAsync(object? key)
    {
        object? value = null;

        if (_repository != null && key != null)
        {
            _repository.TryGetValue(key, out value);
        }

        return await Task.FromResult(value);
    }

    #endregion

    #region IAsyncDisposable Members

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        _repository?.Dispose();

        await Task.Yield();
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _repository?.Dispose();
    }

    #endregion

}
