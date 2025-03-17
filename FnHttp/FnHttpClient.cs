using LanguageExt.Sys.Live;

namespace FnHttp;

public interface IFnHttpClient
{
    internal IDeserializer DefaultDeserializer { get; }
    ValueTask<OneOf<FnHttpResponse, FnHttpError>> Send(FnHttpRequest request, CancellationToken cancellationToken = default);
}

public class FnHttpClient : IFnHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly FnHttpClientOptions _options;

    public FnHttpClient(HttpClient httpClient, FnHttpClientOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    IDeserializer IFnHttpClient.DefaultDeserializer { get; } = new SystemTextJsonDeserializer();

    public async ValueTask<OneOf<FnHttpResponse, FnHttpError>> Send(FnHttpRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = request.ToHttpRequestMessage();
        
        var cts = new CancellationTokenSource(request.Timeout);
        var lts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
        
        var environment = EnvIO.New(source:lts, token: lts.Token);
        environment.Resources.Acquire(cts);
        environment.Resources.Acquire(httpRequest);
        
        var send = IO.liftAsync(env => _httpClient.SendAsync(httpRequest, request.CompletionOption, env.Token)).Catch(ResolveError);

        IO<FnHttpContent> Content(HttpResponseMessage res) => IO.liftVAsync((env) => FnHttpContent.From(res, request.Content, env.Token));

        var c = from a in send
        from b in Content(a)
        select new FnHttpResponse
        {
            HttpStatusCode = a.StatusCode,
            IsSuccessStatusCode = a.IsSuccessStatusCode,
            CharSet = a.Content.Headers.ContentType?.CharSet,
            ContentType = a.Content.Headers.ContentType?.MediaType,
            ContentLength = a.Content.Headers.ContentLength,
            Content = b
        };

        var fin = await c.BracketIO().RunSafeAsync(environment);
        
        return fin.Match(OneOf<FnHttpResponse, FnHttpError>.FromT0, e=> OneOf<FnHttpResponse, FnHttpError>.FromT1(ResolveError(e)));
    }

    private static FnHttpError ResolveError(Error error)
    {
        var exception = error.ToException() switch
        {
            InvalidOperationException e => new FnHttpRequestError(e),
            HttpRequestException e => new FnHttpTransportError(e),
            TaskCanceledException { CancellationToken.IsCancellationRequested: false } e => new FnHttpTransportError(e),
            OperationCanceledException => new FnHttpRequestError("Request was explicitly canceled by the caller."),
            { } e => FnHttpError.Exceptional(e)
        };

        return exception;
    }
}