using OneOf;

namespace FnHttp;

[GenerateOneOf]
public partial class RawContent : OneOfBase<string, byte[], Stream>, IDisposable, IAsyncDisposable
{
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