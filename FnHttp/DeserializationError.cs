namespace FnHttp;

public record DeserializationError: FnHttpResponseError
{
    public DeserializationError(Exception ex) : base(ex)
    {
    }
}