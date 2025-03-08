using System.Text.Json;

namespace FnHttp;

public interface IDeserializer
{
    ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError>> Deserialize<T>(FnHttpResponse response, CancellationToken cancellationToken = default);
}

public class SystemTextJsonDeserializer(JsonSerializerOptions options) : IDeserializer
{
    public SystemTextJsonDeserializer() : this(new JsonSerializerOptions(JsonSerializerDefaults.Web))
    {
        
    }

    public async ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError>> Deserialize<T>(FnHttpResponse response, CancellationToken cancellationToken = default)
    {
        var result = response.EnsureSuccessStatusCode();
        if (result.IsT1)
        {
            // var error = new FnHttpRequestError($"Request failed with status code {response.HttpStatusCode}.",
            //     (int)response.HttpStatusCode);
            
            //return LanguageExt.Aff<T?>.Fail(error);

            //return FailAff<T?>(error);

            return result.AsT1;
        }
             
        
        // var rawContent = response.Content;
        //
        // var task = rawContent.Match(
        //     a => new ValueTask<T>(JsonSerializer.Deserialize<T?>(a, options)!),
        //     b => new ValueTask<T>(JsonSerializer.Deserialize<T?>(b, options)!),
        //     c => JsonSerializer.DeserializeAsync<T?>(c, options, cancellationToken)!
        // );
        //
        // return AffMaybe<T?>(async () =>
        // {
        //     try
        //     {
        //         return await task;
        //     }
        //     catch (Exception e)
        //     {
        //         return new DeserializationError(e);
        //     }
        // });
        
        /*return response.Content.Match(
            str => Fin<T?>.Succ(JsonSerializer.Deserialize<T>(str, options)).ToAff(), // Synchronous
            bytes => Fin<T?>.Succ(JsonSerializer.Deserialize<T>(bytes, options)).ToAff(), // Synchronous
            stream => AffMaybe<T?>(async () => // Asynchronous
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
                }
                catch (Exception ex)
                {
                    return new DeserializationError(ex); // Handle deserialization failure
                }
            })
        );*/
        
        var data = response.Content.Match(
            str => new ValueTask<T?>(JsonSerializer.Deserialize<T?>(str, options)), 
            bytes => new ValueTask<T?>(JsonSerializer.Deserialize<T?>(bytes, options)),
            stream => JsonSerializer.DeserializeAsync<T?>(stream, options, cancellationToken)
        );

        try
        {
            return await data;
        }
        catch (JsonException ex)
        {
            return new DeserializationError(ex); // Handle deserialization failure
        }
        
    }
}