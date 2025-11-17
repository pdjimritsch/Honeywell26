namespace MediaPlayer.Security;

using Abstraction;
using Headers;

/// <summary>
/// 
/// </summary>
public sealed partial class HttpCommunication : IHttpCommunication
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    readonly IServiceProvider? _provider;

    /// <summary>
    /// 
    /// </summary>
    readonly HttpClient? _client;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    public HttpCommunication(IServiceProvider? provider) : base()
    {
        _provider = provider;

        _client = _provider?.GetRequiredService<HttpClient>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="handler"></param>
    public HttpCommunication(IServiceProvider provider, HttpMessageHandler handler) : base()
    {
        _provider= provider;
        _client = new HttpClient(handler);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="client"></param>
    public HttpCommunication(IServiceProvider? provider, HttpClient? client) : base()
    {
        _provider = provider;
        _client = client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="clientFactory"></param>
    public HttpCommunication(IServiceProvider? provider, IHttpClientFactory? clientFactory) : base()
    {
        _provider = provider;
        _client = clientFactory?.CreateClient(nameof(HttpCommunication)) ??
            provider?.GetRequiredService<HttpClient>() ??
            default;
    }

    #endregion

    #region IHttpCommunication Members - Properties

    /// <summary>
    /// 
    /// </summary>
    public HttpRequestHeaders? DefaultRequestHeaders => _client?.DefaultRequestHeaders;

    /// <summary>
    /// 
    /// </summary>
    public AggregateException? Errors { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public IAppSecurityContext? SecurityContext { get; set; } = null!;

    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="header"></param>
    public static void SetRequestHeaders(HttpRequestMessage? message, ApiRequestHeader? header)
    {
        if ((message == null) || (header == null))
        {
            return;
        }

        message.Headers.Clear();

        if (!string.IsNullOrEmpty(header.ContentType) && !string.IsNullOrWhiteSpace(header.ContentType))
        {
            message.Headers.Add("Content-Type", header.ContentType);
        }

        if (header.Headers != null)
        {
            message.Headers.ConnectionClose = header.Headers.ConnectionClose;
            message.Headers.Date = header.Headers.Date;
            message.Headers.ExpectContinue = header.Headers.ExpectContinue;
            message.Headers.From = header.Headers.From;
            message.Headers.Host = header.Headers.Host;
            message.Headers.IfModifiedSince = header.Headers.IfModifiedSince;
            message.Headers.IfRange = header.Headers.IfRange;
            message.Headers.IfUnmodifiedSince = header.Headers.IfUnmodifiedSince;
            message.Headers.MaxForwards = header.Headers.MaxForwards;
            message.Headers.TransferEncodingChunked = header.Headers.TransferEncodingChunked;
        }

        if ((header.Headers?.Accept != null) && (message.Headers?.Accept != null))
        {
            header.Headers.Accept.CopyTo([.. message.Headers.Accept], 0);
        }

        if ((header.Headers?.AcceptCharset != null) && (message.Headers?.AcceptCharset != null))
        {
            header.Headers.AcceptCharset.CopyTo([.. message.Headers.AcceptCharset], 0);
        }

        if ((header.Headers?.AcceptEncoding != null) && 
            (message.Headers?.AcceptEncoding != null))
        {
            header.Headers.AcceptEncoding.CopyTo([.. message.Headers.AcceptEncoding], 0);
        }

        if ((header.Headers?.AcceptLanguage != null) && 
            (message.Headers?.AcceptLanguage != null))
        {
            header.Headers.AcceptLanguage.CopyTo([.. message.Headers.AcceptLanguage], 0);
        }

        if ((header.Headers?.Authorization != null) && (message.Headers?.Authorization != null))
        {
            message.Headers.Authorization = header.Headers.Authorization;
        }

        if ((header.Headers?.CacheControl != null) &&
            (header.Headers.CacheControl.Extensions != null) &&
            (message.Headers?.CacheControl != null) &&
            (message.Headers?.CacheControl?.Extensions != null))
        {
            header.Headers.CacheControl.Extensions.CopyTo([.. message.Headers.CacheControl.Extensions], 0);
        }

        if ((header.Headers?.CacheControl != null) && (header.Headers.CacheControl.NoCacheHeaders != null) &&
            (message.Headers?.CacheControl != null) && (message.Headers?.CacheControl.NoCacheHeaders != null))
        {
            header.Headers.CacheControl.NoCacheHeaders.CopyTo([.. message.Headers.CacheControl.NoCacheHeaders], 0);
        }

        if ((header.Headers?.CacheControl != null) && (header.Headers.CacheControl.PrivateHeaders != null) &&
            (message.Headers?.CacheControl != null) && (message.Headers?.CacheControl.PrivateHeaders != null))
        {
            header.Headers.CacheControl.PrivateHeaders.CopyTo([.. message.Headers.CacheControl.PrivateHeaders], 0);
        }

        if ((header.Headers?.CacheControl?.MaxAge.HasValue ?? false) && (message.Headers?.CacheControl?.MaxAge != null))
        {
            message.Headers.CacheControl.MaxAge = header.Headers.CacheControl.MaxAge.Value;
            message.Headers.CacheControl.MaxStale = header.Headers.CacheControl.MaxStale;
            message.Headers.CacheControl.MaxStaleLimit = header.Headers.CacheControl.MaxStaleLimit;
            message.Headers.CacheControl.MinFresh = header.Headers.CacheControl.MinFresh;
            message.Headers.CacheControl.MustRevalidate = header.Headers.CacheControl.MustRevalidate;
            message.Headers.CacheControl.NoCache = header.Headers.CacheControl.NoCache;
            message.Headers.CacheControl.NoStore = header.Headers.CacheControl.NoStore;
            message.Headers.CacheControl.NoTransform = header.Headers.CacheControl.NoTransform;
            message.Headers.CacheControl.OnlyIfCached = header.Headers.CacheControl.OnlyIfCached;
            message.Headers.CacheControl.Private = header.Headers.CacheControl.Private;
            message.Headers.CacheControl.ProxyRevalidate = header.Headers.CacheControl.ProxyRevalidate;
            message.Headers.CacheControl.Public = header.Headers.CacheControl.Public;
            message.Headers.CacheControl.SharedMaxAge = header.Headers.CacheControl.SharedMaxAge;
        }

        if ((header.Headers?.Connection != null) && (message.Headers?.Connection != null))
        {
            header.Headers.Connection.CopyTo([.. message.Headers.Connection], 0);
        }

        if ((header.Headers?.Expect != null) && (message.Headers?.Expect != null))
        {
            header.Headers.Expect.CopyTo([.. message.Headers.Expect], 0);
        }

        if ((header.Headers?.IfMatch != null) && (message.Headers?.IfMatch != null))
        {
            header.Headers.IfMatch.CopyTo([.. message.Headers.IfMatch], 0);
        }

        if ((header.Headers?.IfNoneMatch != null) && (message.Headers?.IfNoneMatch != null))
        {
            header.Headers.IfNoneMatch.CopyTo([.. message.Headers.IfNoneMatch], 0);
        }

        if ((header.Headers?.Pragma != null) && (message.Headers?.Pragma != null))
        {
            header.Headers?.Pragma.CopyTo([.. message.Headers.Pragma], 0);
        }

        if ((header.Headers?.ProxyAuthorization != null) && (message.Headers?.ProxyAuthorization != null))
        {
            message.Headers.ProxyAuthorization = header.Headers.ProxyAuthorization;
        }

        if ((header.Headers?.Range != null) && (message.Headers?.Range != null))
        {
            message.Headers.Range = header.Headers.Range;
        }

        if ((header.Headers?.Referrer != null) && (message.Headers?.Referrer != null))
        {
            message.Headers.Referrer = header.Headers.Referrer;
        }

        if ((header.Headers?.TE != null) && (message.Headers?.TE != null))
        {
            header.Headers.TE.CopyTo([.. message.Headers.TE], 0);
        }

        if ((header.Headers?.Trailer != null) && (message.Headers?.Trailer != null))
        {
            header.Headers.Trailer.CopyTo([.. message.Headers.Trailer], 0);
        }

        if ((header.Headers?.TransferEncoding != null) && (message.Headers?.TransferEncoding != null))
        {
            header.Headers.TransferEncoding.CopyTo([.. message.Headers.TransferEncoding], 0);
        }

        if ((header.Headers?.Upgrade != null) && (message.Headers?.Upgrade != null))
        {
            header.Headers.Upgrade.CopyTo([.. message.Headers.Upgrade], 0);
        }

        if ((header.Headers?.Via != null) && (message.Headers?.Via != null))
        {
            header.Headers.Via.CopyTo([.. message.Headers.Via], 0);
        }

        if ((header.Headers?.Warning != null) && (message.Headers?.Warning != null))
        {
            header.Headers.Warning.CopyTo([.. message.Headers.Warning], 0);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="header"></param>
    public static void SetRequestHeaders(HttpClient? client, ApiRequestHeader? header)
    {
        if ((client == null) || (header == null))
        {
            return;
        }

        client.DefaultRequestHeaders.Clear();

        if (!string.IsNullOrEmpty(header.ContentType) && !string.IsNullOrWhiteSpace(header.ContentType))
        {
            client.DefaultRequestHeaders.Add("Content-Type", header.ContentType);
        }

        if ((header.Headers != null) && (client.DefaultRequestHeaders != null))
        {
            client.DefaultRequestHeaders.ConnectionClose = header.Headers.ConnectionClose;
            client.DefaultRequestHeaders.Date = header.Headers.Date;
            client.DefaultRequestHeaders.ExpectContinue = header.Headers.ExpectContinue;
            client.DefaultRequestHeaders.From = header.Headers.From;
            client.DefaultRequestHeaders.Host = header.Headers.Host;
            client.DefaultRequestHeaders.IfModifiedSince = header.Headers.IfModifiedSince;
            client.DefaultRequestHeaders.IfRange = header.Headers.IfRange;
            client.DefaultRequestHeaders.IfUnmodifiedSince = header.Headers.IfUnmodifiedSince;
            client.DefaultRequestHeaders.MaxForwards = header.Headers.MaxForwards;
            client.DefaultRequestHeaders.TransferEncodingChunked = header.Headers.TransferEncodingChunked;
        }

        if ((header.Headers?.Accept != null) &&
            (client.DefaultRequestHeaders?.Accept != null))
        {
            header.Headers.Accept.CopyTo([.. client.DefaultRequestHeaders.Accept], 0);
        }

        if ((header.Headers?.AcceptCharset != null) &&
            (client?.DefaultRequestHeaders?.AcceptCharset != null))
        {
            header.Headers.AcceptCharset.CopyTo([.. client.DefaultRequestHeaders.AcceptCharset], 0);
        }

        if ((header.Headers?.AcceptEncoding != null) &&
            (client?.DefaultRequestHeaders?.AcceptEncoding != null))
        {
            header.Headers.AcceptEncoding.CopyTo([.. client.DefaultRequestHeaders.AcceptEncoding], 0);
        }

        if ((header.Headers?.AcceptLanguage != null) &&
            (client?.DefaultRequestHeaders?.AcceptLanguage != null))
        {
            header.Headers.AcceptLanguage.CopyTo([.. client.DefaultRequestHeaders.AcceptLanguage], 0);
        }

        if ((header.Headers?.Authorization != null) &&
            (client?.DefaultRequestHeaders?.Authorization != null))
        {
            client.DefaultRequestHeaders.Authorization = header.Headers.Authorization;
        }

        if ((header.Headers?.CacheControl != null) && 
            (header.Headers.CacheControl.Extensions != null) &&
            (client?.DefaultRequestHeaders?.CacheControl != null) &&
            (client?.DefaultRequestHeaders?.CacheControl?.Extensions != null))
        {
            header.Headers.CacheControl.Extensions.CopyTo([.. client.DefaultRequestHeaders.CacheControl.Extensions], 0);
        }

        if ((header.Headers?.CacheControl != null) &&
            (header.Headers.CacheControl.NoCacheHeaders != null) &&
            (client?.DefaultRequestHeaders?.CacheControl != null) &&
            (client?.DefaultRequestHeaders?.CacheControl.NoCacheHeaders != null))
        {
            header.Headers.CacheControl.NoCacheHeaders.CopyTo([.. client.DefaultRequestHeaders.CacheControl.NoCacheHeaders], 0);
        }

        if ((header.Headers?.CacheControl != null) &&
            (header.Headers.CacheControl.PrivateHeaders != null) &&
            (client?.DefaultRequestHeaders?.CacheControl != null) &&
            (client?.DefaultRequestHeaders?.CacheControl.PrivateHeaders != null))
        {
            header.Headers.CacheControl.PrivateHeaders.CopyTo([.. client.DefaultRequestHeaders.CacheControl.PrivateHeaders], 0);
        }

        if ((header.Headers?.CacheControl?.MaxAge.HasValue ?? false) &&
            (client?.DefaultRequestHeaders?.CacheControl?.MaxAge != null))
        {
            client.DefaultRequestHeaders.CacheControl.MaxAge = header.Headers.CacheControl.MaxAge.Value;
            client.DefaultRequestHeaders.CacheControl.MaxStale = header.Headers.CacheControl.MaxStale;
            client.DefaultRequestHeaders.CacheControl.MaxStaleLimit = header.Headers.CacheControl.MaxStaleLimit;
            client.DefaultRequestHeaders.CacheControl.MinFresh = header.Headers.CacheControl.MinFresh;
            client.DefaultRequestHeaders.CacheControl.MustRevalidate = header.Headers.CacheControl.MustRevalidate;
            client.DefaultRequestHeaders.CacheControl.NoCache = header.Headers.CacheControl.NoCache;
            client.DefaultRequestHeaders.CacheControl.NoStore = header.Headers.CacheControl.NoStore;
            client.DefaultRequestHeaders.CacheControl.NoTransform = header.Headers.CacheControl.NoTransform;
            client.DefaultRequestHeaders.CacheControl.OnlyIfCached = header.Headers.CacheControl.OnlyIfCached;
            client.DefaultRequestHeaders.CacheControl.Private = header.Headers.CacheControl.Private;
            client.DefaultRequestHeaders.CacheControl.ProxyRevalidate = header.Headers.CacheControl.ProxyRevalidate;
            client.DefaultRequestHeaders.CacheControl.Public = header.Headers.CacheControl.Public;
            client.DefaultRequestHeaders.CacheControl.SharedMaxAge = header.Headers.CacheControl.SharedMaxAge;
        }

        if ((header.Headers?.Connection != null) && 
            (client?.DefaultRequestHeaders?.Connection != null))
        {
            header.Headers.Connection.CopyTo([.. client.DefaultRequestHeaders.Connection], 0);
        }

        if ((header.Headers?.Expect != null) &&
            (client?.DefaultRequestHeaders?.Expect != null))
        {
            header.Headers.Expect.CopyTo([.. client.DefaultRequestHeaders.Expect], 0);
        }

        if ((header.Headers?.IfMatch != null) &&
            (client?.DefaultRequestHeaders?.IfMatch != null))
        {
            header.Headers.IfMatch.CopyTo([.. client.DefaultRequestHeaders.IfMatch], 0);
        }

        if ((header.Headers?.IfNoneMatch != null) &&
            (client?.DefaultRequestHeaders?.IfNoneMatch != null))
        {
            header.Headers.IfNoneMatch.CopyTo([.. client.DefaultRequestHeaders.IfNoneMatch], 0);
        }

        if ((header.Headers?.Pragma != null) &&
            (client?.DefaultRequestHeaders?.Pragma != null))
        {
            header.Headers?.Pragma.CopyTo([.. client.DefaultRequestHeaders.Pragma], 0);
        }

        if ((header.Headers?.ProxyAuthorization != null) &&
            (client?.DefaultRequestHeaders?.ProxyAuthorization != null))
        {
            client.DefaultRequestHeaders.ProxyAuthorization = header.Headers.ProxyAuthorization;
        }

        if ((header.Headers?.Range != null) &&
            (client?.DefaultRequestHeaders?.Range != null))
        {
            client.DefaultRequestHeaders.Range = header.Headers.Range;
        }

        if ((header.Headers?.Referrer != null) &&
            (client?.DefaultRequestHeaders?.Referrer != null))
        {
            client.DefaultRequestHeaders.Referrer = header.Headers.Referrer;
        }

        if ((header.Headers?.TE != null) &&
            (client?.DefaultRequestHeaders?.TE != null))
        {
            header.Headers.TE.CopyTo([.. client.DefaultRequestHeaders.TE], 0);
        }

        if ((header.Headers?.Trailer != null) &&
            (client?.DefaultRequestHeaders?.Trailer != null))
        {
            header.Headers.Trailer.CopyTo([.. client.DefaultRequestHeaders.Trailer], 0);
        }

        if ((header.Headers?.TransferEncoding != null) &&
            (client?.DefaultRequestHeaders?.TransferEncoding != null))
        {
            header.Headers.TransferEncoding.CopyTo([.. client.DefaultRequestHeaders.TransferEncoding], 0);
        }

        if ((header.Headers?.Upgrade != null) &&
            (client?.DefaultRequestHeaders?.Upgrade != null))
        {
            header.Headers.Upgrade.CopyTo([.. client.DefaultRequestHeaders.Upgrade], 0);
        }

        if ((header.Headers?.Via != null) &&
            (client?.DefaultRequestHeaders?.Via != null))
        {
            header.Headers.Via.CopyTo([.. client.DefaultRequestHeaders.Via], 0);
        }

        if ((header.Headers?.Warning != null) &&
            (client?.DefaultRequestHeaders?.Warning != null))
        {
            header.Headers.Warning.CopyTo([.. client.DefaultRequestHeaders.Warning], 0);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="headers"></param>
    public static void SetHeaders(HttpClient? client, HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>? headers)
    {
        if ((client == null) || (headers == null))
        {
            return;
        }

        client.DefaultRequestHeaders.Clear();



    }

    #endregion

    #region IAsyncDisposable Members

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        if (_client != null)
        {
            _client.Dispose();
        }

        await Task.Yield();
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (_client != null)
        {
            _client.Dispose();
        }
    }

    #endregion

    #region Internal Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    private static bool IsBinary(object? component)
    {
        if (component == null) return false;

        if (component is byte[] bytes)
        {
            return bytes.Length > 0;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="violation"></param>
    private void AddError(Exception violation)
    {
        if (violation == null)
        {
            return;
        }

        if (Errors == null)
        {
            Errors = new AggregateException(violation);
        }
        else
        {
            var violations = new List<Exception>([.. Errors.InnerExceptions]) { violation };

            Errors = new AggregateException(violations);
        }
    }

    #endregion
}
