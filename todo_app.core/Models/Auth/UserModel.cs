using Microsoft.AspNetCore.Identity;
using todo_app.core.Models.Data;

namespace todo_app.core.Models.Auth;
public class UserModel : IdentityUser
{
    public ICollection<Note> Notes { get; set; }
}
