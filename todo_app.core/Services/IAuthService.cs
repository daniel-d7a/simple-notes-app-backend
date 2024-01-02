using System;
using System.Linq;
using todo_app.core.Models.Auth;
using todo_app.core.Models.ResponseModels.Auth;

namespace todo_app.core.Services;

public interface IAuthService
{

    Task<AuthResponse> RegisterAsync(RegisterModel model);

    Task<AuthResponse> LoginAsync(LoginModel model);
}
