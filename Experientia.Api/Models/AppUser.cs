
using Microsoft.AspNetCore.Identity;

namespace Experientia.Api.Models;
public class AppUser : IdentityUser<long>
{
    public string FullName { get; set; } = "";
}