using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Authentication.Models
{
    public class AcceptInvitation
    {
        public bool Succeeded { get; set; }
        public User User { get; set; }        
    }
}