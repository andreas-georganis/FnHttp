using System.Text.Json;

namespace FnHttp;

public interface IDeserializer
{
    ValueTask<OneOf<T?, DeserializationError>> Deserialize<T>(FnHttpContent content, CancellationToken cancellationToken = default);
}

public class SystemTextJsonDeserializer(JsonSerializerOptions options) : IDeserializer
{
    public SystemTextJsonDeserializer() : this(new JsonSerializerOptions(JsonSerializerDefaults.Web))
    {
        
    }

    public async ValueTask<OneOf<T?, DeserializationError>> Deserialize<T>(FnHttpContent content, CancellationToken cancellationToken = default)
    {
        
        
        var data = content.Match(
            str => new ValueTask<T?>(JsonSerializer.Deserialize<T?>(str, options)), 
            bytes => new ValueTask<T?>(JsonSerializer.Deserialize<T?>(bytes, options)),
            stream => JsonSerializer.DeserializeAsync<T?>(stream, options, cancellationToken),
            _ => default
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