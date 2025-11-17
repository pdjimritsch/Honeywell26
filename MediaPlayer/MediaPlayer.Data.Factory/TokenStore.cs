using MediaPlayer.Data.Factory.Abstraction;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace MediaPlayer.Data.Factory;
using Abstraction;
using System.Xml.Linq;

/// <summary>
/// 
/// </summary>
[ImmutableObject(true)] public sealed partial class TokenStore : ITokenStore
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    private static TokenStore? _instance;

    /// <summary>
    /// 
    /// </summary>
    private static object _lock = new object();

    /// <summary>
    /// 
    /// </summary>
    private readonly ConcurrentDictionary<string, ConcurrentBag<object>> _container;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    private TokenStore() : base()
    {
        _container = new ConcurrentDictionary<string, ConcurrentBag<object>>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public static TokenStore Instance => _instance ??= new TokenStore();

    #endregion

    #region ITokenStore Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Add(string key, object value)
    {
        var succeeded = false;

        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()) && (value is not null))
        {
            var count = _container[key.Trim()].Count;

            _container.AddOrUpdate(key, new ConcurrentBag<object> { value }, (_key, container) =>
            {
                lock (_lock)
                {
                    if (!string.IsNullOrEmpty(_key) && _key.Equals(key, StringComparison.Ordinal))
                    {
                        if (container.IsEmpty)
                        {
                            container.Add(value);
                        }
                        else if (!container.Contains(value))
                        {
                            container.Add(value);
                        }
                    }

                    return container;
                }
            });

            succeeded = count < _container[key].Count;
        }
        else if (!string.IsNullOrEmpty(key) && (value != null))
        {
            succeeded = _container.TryAdd(key.Trim(), new ConcurrentBag<object> { value });
        }

        return succeeded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kvp"></param>
    /// <returns></returns>
    public bool Add(KeyValuePair<string, object> kvp)
    {
        var succeeded = false;

        if (!string.IsNullOrEmpty(kvp.Key) && _container.ContainsKey(kvp.Key.Trim()) && (kvp.Value is not null))
        {
            var count = _container[kvp.Key.Trim()].Count;

            _container.AddOrUpdate(kvp.Key.Trim(), new ConcurrentBag<object> { kvp.Value }, (_key, container) =>
            {
                lock (_lock)
                {
                    if (!string.IsNullOrEmpty(_key) && (_key.Trim() == kvp.Key.Trim()))
                    {
                        container.Add(kvp.Value);
                    }

                    return container;
                }
            });

            succeeded = count < _container[kvp.Key.Trim()].Count;
        }
        else if (string.IsNullOrEmpty(kvp.Key) && (kvp.Value is not null))
        {
            succeeded = _container.TryAdd(kvp.Key, new ConcurrentBag<object> { kvp.Value });
        }

        return succeeded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public bool AddRange(string key, IEnumerable<object> values)
    {
        bool succeeded = false;

        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()) && (values is not null) && values.Any())
        {
            var count = _container[key.Trim()].Count;

            _container.AddOrUpdate(key.Trim(), new ConcurrentBag<object> { values }, (_key, container) =>
            {
                lock (_lock)
                {
                    if (!string.IsNullOrEmpty(_key) && (_key.Trim() == key.Trim()))
                    {
                        foreach (var value in values)
                        {
                            container.Add(value);
                        }
                    }

                    return container;
                }
            });

            succeeded = count < _container[key.Trim()].Count;
        }
        else if (!string.IsNullOrEmpty(key) && (values != null) && values.Any())
        {
            lock (_lock)
            {
                var container = new ConcurrentBag<object>();

                foreach (var value in values)
                {
                    container.Add(value);
                }

                succeeded = _container.TryAdd(key.Trim(), container);
            }
        }

        return succeeded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public bool AddRange(IDictionary<string, object> attributes)
    {
        if (attributes is not null && attributes.Any())
        {
            foreach (var attribute in attributes)
            {
                if (!Add(attribute)) return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        _container.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void Clear(string key)
    {
        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()))
        {
            _container[key.Trim()].Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return _container.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int Count(string key)
    {
        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()))
        {
            return _container[key.Trim()].Count;
        }
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(string key)
    {
        return !string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(string key, object value)
    {
        if (!string.IsNullOrEmpty(key) && (value is not null) && _container.ContainsKey(key.Trim()))
        {
            var container = _container[key.Trim()];

            if (container is not null)
            {
                return container.Contains(value);
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, IEnumerable<object>>> Get()
    {
        var sequence = _container.AsEnumerable();

        foreach (var entry in sequence)
        {
            yield return new KeyValuePair<string, IEnumerable<object>>(entry.Key, entry.Value.AsEnumerable());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IEnumerable<object> Get(string key)
    {
        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()))
        {
            return _container[key.Trim()].AsEnumerable();
        }
        return Enumerable.Empty<object>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public bool Remove(string key)
    {
        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()))
        {
            return _container.TryRemove(key.Trim(), out _);
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Remove(string key, object value)
    {
        if (!string.IsNullOrEmpty(key) && _container.ContainsKey(key.Trim()))
        {
            var container = _container[key.Trim()];

            if (container is not null)
            {
                var count = container.ToList().Count;

                var list = container.ToList();

                list.Remove(value);

                _container.AddOrUpdate(key.Trim(), new ConcurrentBag<object> { list }, (_key, _dictionary) =>
                {
                    return new ConcurrentBag<object> { list };
                });

                return !_container.ContainsKey(key.Trim()) || (count > _container[key.Trim()].Count);
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public bool RemoveRange(IEnumerable<string> keys)
    {
        var count = _container.Keys.Count;

        if (keys is not null && keys.Any())
        {
            foreach (var key in keys)
            {
                if (_container.ContainsKey(key.Trim()))
                {
                    _container.TryRemove(key.Trim(), out _);

                    _container.Keys.Remove(key.Trim());
                }
            }
        }

        return count > _container.Keys.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public bool RemoveRange(string key, IEnumerable<object> values)
    {
        if (!string.IsNullOrEmpty(key) && (values is not null) && values.Any())
        {
            if (_container.ContainsKey(key.Trim()))
            {
                var container = _container[key.Trim()];

                if (container is not null)
                {
                    var count = container.Count;

                    foreach (var value in values)
                    {
                        container.ToList().Remove(value);
                    }

                    _container.AddOrUpdate(key.Trim(), new ConcurrentBag<object> { container }, (_key, _dictionary) =>
                    {
                        return container;
                    });

                    return count > container.Count;
                }
            }
        }
        return false;
    }

    #endregion
}
