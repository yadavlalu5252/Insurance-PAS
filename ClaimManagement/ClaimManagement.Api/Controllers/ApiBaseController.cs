using ClaimManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

public abstract class ApiBaseController : ControllerBase
{
    // the logged-in user's id, taken from the token
    protected long UserId =>
        HttpContext.RequestServices.GetRequiredService<ICurrentUser>().UserId;
}