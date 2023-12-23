using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_app.core.Models.Auth;
using todo_app.core.Models.ResponseModels.Auth;

namespace todo_app.core.Services;

public interface IAuthService
{

    Task<AuthResponse> RegisterAsync(RegisterModel model);

    Task<AuthResponse> LoginAsync(LoginModel model);
}
 