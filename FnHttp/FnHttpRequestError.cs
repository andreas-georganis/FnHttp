namespace FnHttp;

[Serializable]
public record FnHttpRequestError : Expected
{
    public FnHttpRequestError(string message, int code) : base(message, code, None)
    {
    }
}