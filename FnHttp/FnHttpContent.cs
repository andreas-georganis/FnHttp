using OneOf;

namespace FnHttp;

[GenerateOneOf]
public partial class FnHttpContent : OneOfBase<string, byte[], Stream, Unit>, IDisposable, IAsyncDisposable
{
    internal static async ValueTask<FnHttpContent> From(HttpResponseMessage response, ContentAs contentAs, CancellationToken cancellationToken = default)
    {
        return contentAs switch
        {
            ContentAs.ByteArray => new FnHttpContent(await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false)),
            ContentAs.Stream => new FnHttpContent(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)),
            ContentAs.String => new FnHttpContent(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)),
            _ => new FnHttpContent(Unit.Default),
        };
    }
    
    private bool _disposed;
    public void Dispose()
    {
        if (_disposed) return;

        if (Value is not Stream stream) return;
        
        stream.Dispose();
        GC.SuppressFinalize(this);
        _disposed = true;

    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        if (Value is Stream stream)
        {
            await stream.DisposeAsync();
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}