namespace PromoCodeFactory.WebHost.ExceptionHandling
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
