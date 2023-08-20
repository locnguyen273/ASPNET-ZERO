namespace API.Errors
{
  public class ApiException
  {
    public int StatusCode { get; set; }

    public ApiException(int statusCode, string message, string details)
    {
      StatusCode = statusCode;
      Message = message;
      Details = details;
    }

    public string Message { get; set; }
    public string Details { get; set; }
  }
}