using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace todo_app.core.Models.Auth;

public class LoginModel
{
    [Required]
    [MaxLength(265)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(50)]
    public string Password { get; set; }
}
