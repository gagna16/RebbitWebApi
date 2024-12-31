using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RebbitWebApi.Model
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
