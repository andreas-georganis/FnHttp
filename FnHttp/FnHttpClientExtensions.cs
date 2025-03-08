using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using LanguageExt.ClassInstances.Const;
using OneOf;

namespace FnHttp;

public static partial class FnHttpClientExtensions
{
    public static async ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError, Error>> SendAsync<T>(
        this IFnHttpClient client, 
        FnHttpRequest request,
        CancellationToken cancellationToken = default)
    {
        var fnHttpResponseFin = await client.SendAff<T?>(request, cancellationToken).Run().ConfigureAwait(false);

        var fnHttpResponse = fnHttpResponseFin.Match(s =>
        {
            var data = s.Data;

            OneOf<T?, FnHttpRequestError, DeserializationError, Error> d = data switch
            {
                { IsT0: true } => data.AsT0,
                { IsT1: true } => data.AsT1,
                { IsT2: true } => data.AsT2,
                _ => data.AsT2
            };

            return d;

        }, OneOf<T?, FnHttpRequestError, DeserializationError, Error>.FromT3);

        return fnHttpResponse;
    }
    
    public static Aff<FnHttpResponse<T?>> SendAff<T>(
        this IFnHttpClient client, 
        FnHttpRequest request,
        CancellationToken cancellationToken = default)
    {
        return client
            .SendAff(request, cancellationToken)
            .MapAsync(async x =>
        {
            await using var response = x;
            
            var data = await client.DefaultDeserializer.Deserialize<T>(response, cancellationToken);
            
            return new FnHttpResponse<T?>
            {
                HttpStatusCode = response.HttpStatusCode,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                Data = data,
                CharSet = response.CharSet,
                ContentType = response.ContentType,
                ContentLength = response.ContentLength
            };
        });
    }
}

