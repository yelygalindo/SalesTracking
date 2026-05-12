namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class ChangeCustomerStatusResponse
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}
