using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using todo_app.core.Models.Auth;

namespace todo_app.core.Models.Data;

public class Note : BaseModel
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    public string? Body { get; set; }

    [Required]
    public string UserId { get; set; }

    public UserModel User { get; set; }
}
