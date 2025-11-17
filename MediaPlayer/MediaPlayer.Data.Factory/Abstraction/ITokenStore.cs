namespace MediaPlayer.Data.Factory.Abstraction;

/// <summary>
/// 
/// </summary>
public partial interface ITokenStore
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Add(string key, object value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kvp"></param>
    /// <returns></returns>
    bool Add(KeyValuePair<string, object> kvp);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    bool AddRange(string key, IEnumerable<object> values);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    bool AddRange(IDictionary<string, object> attributes);

    /// <summary>
    /// 
    /// </summary>
    void Clear();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    void Clear(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    int Count();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Count(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(string key, object value);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerable<KeyValuePair<string, IEnumerable<object>>> Get();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IEnumerable<object> Get(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    bool Remove(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Remove(string key, object value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    bool RemoveRange(IEnumerable<string> keys);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    bool RemoveRange(string key, IEnumerable<object> values);

    #endregion
}