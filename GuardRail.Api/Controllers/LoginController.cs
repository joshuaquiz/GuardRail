using System.Threading.Tasks;
using GuardRail.Api.Controllers.Models;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/login")]
    [ApiController]
    public sealed class LoginController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public LoginController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Route("")]
        [HttpPost]
        public async Task<bool> LoginAsync(
            LoginModel loginModel)
        {
            var user = await _guardRailContext
                .Users
                .SingleOrDefaultAsync(
                    x =>
                        x.Username == loginModel.Username
                        && x.Password == loginModel.Password,
                    HttpContext.RequestAborted);
            return user != null;
        }
    }
}