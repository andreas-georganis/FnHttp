using System.Buffers;
using System.Net;
using System.Text.Json;

namespace FnHttp;

public abstract class FnHttpResponseBase
{
    public string? CharSet { get; init; }
    
    public long? ContentLength { get; init; }
    
    public string? ContentType { get; init; }
    
    public HttpStatusCode HttpStatusCode { get; init; }
    
    public bool IsSuccessStatusCode { get; init; }
}



public class FnHttpResponse : FnHttpResponseBase, IDisposable, IAsyncDisposable
{
    // internal Aff<RawContent> Content { get; set; }
    public required RawContent Content { get; init; }
    
    public OneOf<FnHttpResponse, FnHttpRequestError> EnsureSuccessStatusCode()
    {
        if (IsSuccessStatusCode == false)
            return new FnHttpRequestError($"Request failed with status code {HttpStatusCode}.", (int)HttpStatusCode);

        return this;
    }

    public void Dispose()
    {
        Content.Dispose();
        // Content.Map(x=>
        // {
        //      x.Dispose();
        //
        //      return Unit.Default;
        // });
    }

    public ValueTask DisposeAsync()
    {
        return Content.DisposeAsync();
        // await Content.MapAsync(async x =>
        // {
        //     await x.DisposeAsync();
        //     return Unit.Default;
        // }).RunUnit();
    }
}

public class FnHttpResponse<T> : FnHttpResponseBase
{
    public OneOf<T?, FnHttpRequestError, DeserializationError> Data { get; init; }
}
