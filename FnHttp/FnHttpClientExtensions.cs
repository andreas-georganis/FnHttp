using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using OneOf;

namespace FnHttp;

public static partial class FnHttpClientExtensions
{
    public static async ValueTask<OneOf<FnHttpResponse<T?>, FnHttpError>> Send<T>(
        this IFnHttpClient client, 
        FnHttpRequest request,
        CancellationToken cancellationToken = default)
    {
        var fnHttpResponse = await client.Send(request, cancellationToken).ConfigureAwait(false);
        
        if (fnHttpResponse.IsT1)
        {
            return fnHttpResponse.AsT1;
        }
        
        var result = fnHttpResponse.AsT0.EnsureSuccessStatusCode();
        if (result.IsT1)
        {
            return result.AsT1;
        }
        
        await using var response = fnHttpResponse.AsT0;
            
        var data = await client.DefaultDeserializer.Deserialize<T>(fnHttpResponse.AsT0.Content, cancellationToken);
            
        return new FnHttpResponse<T?>
        {
            HttpStatusCode = response.HttpStatusCode,
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            Data = data,
            CharSet = response.CharSet,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength
        };
        
        
        // var fnHttpResponseT = fnHttpResponse.Match(s =>
        // {
        //     var data = s.Data;
        //
        //     OneOf<T?, FnHttpError, DeserializationError, Error> d = data switch
        //     {
        //         { IsT0: true } => data.AsT0,
        //         { IsT1: true } => data.AsT1,
        //         { IsT2: true } => data.AsT2,
        //         _ => data.AsT2
        //     };
        //
        //     return d;
        //
        // }, OneOf<T?, FnHttpError, DeserializationError, Error>.FromT3);

    }
    
   
}

