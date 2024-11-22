using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace builtintauth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("user", Name = "user")]
        [Authorize(AuthenticationSchemes = "Identity.Bearer")]
        public ActionResult<string> GetUser()
        {
            ClaimsPrincipal user = User;
            var cur_user = user;
            return Ok($"Hello {cur_user.Identity?.Name ?? string.Empty}");
        }
    }
}
