public class CustomException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public CustomException(string code) : base(ErrorCatalog.Get(code).Message)
    {
        Code = code;
        StatusCode = ErrorCatalog.Get(code).StatusCode;
    }
}