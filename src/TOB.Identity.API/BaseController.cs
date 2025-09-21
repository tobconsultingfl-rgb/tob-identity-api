using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TOB.Identity.API;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected string CurrentUserId => User.FindAll(ClaimTypes.NameIdentifier).Select(qr => qr.Value).FirstOrDefault();
    protected string CurrentUserTenantId => User.Claims.FirstOrDefault(t => t.Type == "extension_TenantId").Value;
    protected List<string> CurrentUserRoles => User.Claims.FirstOrDefault(t => t.Type == "extension_Roles").Value?.Split(",").ToList();
    protected string CurrentUserEmail => User.FindAll(ClaimTypes.Email).Select(qr => qr.Value).FirstOrDefault();
}
