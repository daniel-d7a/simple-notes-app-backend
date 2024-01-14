using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_app.core.DTOs;

namespace todo_app.core.Models.ResponseModels.Auth
{
    public class AuthResponse
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDTO User { get; set; }


    }
}
