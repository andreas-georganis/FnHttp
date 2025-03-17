namespace FnHttp;

[Serializable]
public record FnHttpError : Error
{
    private readonly OneOf<Expected, Exceptional> _error;
    public FnHttpError(OneOf<Expected, Exceptional> error)
    {
        _error = error;
        Message = error.Match(e => e.Message, e => e.Message);
        IsExceptional = error.Match(e => false, e => true);
        IsExpected = error.Match(e => true, e => false);
    }
    public static FnHttpError Expected(string message) => new((Error.New(message) as Expected)!);
    public static FnHttpError Expected(string message, int code) => new((Error.New(code, message) as Expected)!);
    public static FnHttpError Exceptional(Exception ex) => new((Error.New(ex) as Exceptional)!);

    public override ErrorException ToErrorException()
        => _error.Match(exp =>exp.ToErrorException(), exc=> exc.ToErrorException());

    public override string Message { get; }
   public override bool IsExceptional { get; }
   public override bool IsExpected { get; }
   
}

public record FnHttpRequestError : FnHttpError
{
    public FnHttpRequestError(string message):base(Expected(message))
    {
        
    }
    
    public FnHttpRequestError(Exception ex):base(Exceptional(ex))
    {
        
    }
}

public record FnHttpTransportError : FnHttpError
{
    public FnHttpTransportError(Exception ex):base(Exceptional(ex))
    {
        
    }
}

public record FnHttpResponseError : FnHttpError
{
    public FnHttpResponseError(Exception ex): base(Exceptional(ex))
    {
        
    }
    
    public FnHttpResponseError(string message, int code): base(Expected(message, code))
    {
        
    }
}
