namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class UserResult
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public CompanyResult Company { get; set; }
        public string Email { get; internal set; }
    }
}