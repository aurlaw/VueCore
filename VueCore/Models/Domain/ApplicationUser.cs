using Microsoft.AspNetCore.Identity;

namespace VueCore.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName {get;set;}

        [PersonalData]
        public string LastName {get;set;}
    }
}