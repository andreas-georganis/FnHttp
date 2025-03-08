namespace FnHttp;

public record DeserializationError: Exceptional
{
    public DeserializationError(Exception ex) : base((Exceptional)Exceptional.New(ex))
    {
    }
};