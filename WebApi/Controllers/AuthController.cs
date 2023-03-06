using Infrastructure.Database;
using Infrastructure.Entities;
using Meama_Tasks.Models.Auth;
using Meama_Tasks.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Meama_Tasks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly JwtService _jwtService;

        public AuthController(UserManager<AppUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<AuthResponseVM>> SignIn(AuthRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            var token = _jwtService.CreateToken(user, role!);

            return Ok(token);
        }


    }
}