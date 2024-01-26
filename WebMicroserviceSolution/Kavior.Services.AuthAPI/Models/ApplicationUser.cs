using Microsoft.AspNetCore.Identity;

namespace Kavior.Services.AuthAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
    