using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace todo_app.api.Controllers;

public class SharedController : ControllerBase
{
    protected string GetUserId()
    {
        return User.FindFirstValue("uid")!;
    }
}
