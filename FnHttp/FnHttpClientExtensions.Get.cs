using OneOf;

namespace FnHttp;

public static partial class FnHttpClientExtensions
{
    // public static async ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError>> GetTOrErrors<T>(
    //     this FnHttpClient client, 
    //     Uri? requestUri,
    //     CancellationToken cancellationToken = default)
    // {
    //     var response = await client.Get<T>(requestUri, cancellationToken).ConfigureAwait(false);
    //
    //     //var m = response.Match(OneOf<T?, Error>.FromT0, OneOf<T?, Error>.FromT1);
    //
    //     return response.Match<OneOf<T?, FnHttpRequestError, DeserializationError>>(
    //         s=> OneOf<T?, FnHttpRequestError, DeserializationError>.FromT0(s), 
    //         e =>
    //         {
    //             return default;
    //         });
    // }
    
    public static async ValueTask<OneOf<T?, FnHttpError, DeserializationError, Error>> Get<T>(
        this FnHttpClient client, 
        string? requestUri = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.Send<T>(new FnHttpRequest(requestUri){Method = HttpMethod.Get , CompletionOption = completionOption}, cancellationToken);
        
        return response.IsT0? response.AsT0.Data.AsT0: response.AsT1 ;
    }
    
    public static async ValueTask<OneOf<T?, FnHttpError, DeserializationError, Error>> Get<T>(
        this FnHttpClient client, 
        Uri? requestUri = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.Send<T>(new FnHttpRequest{ Uri = requestUri ,Method = HttpMethod.Get, CompletionOption = completionOption }, cancellationToken);
        
        return response.IsT0? response.AsT0.Data.AsT0: response.AsT1 ;
    }
}