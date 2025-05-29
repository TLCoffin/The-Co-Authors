using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_CoAuthors.Models;
using static The_CoAuthors.Models.DataTransfers;

namespace The_CoAuthors.wwwroot
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Register a user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            var user = new IdentityUser { UserName = register.Username };
            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok("Registered and logged in");
            }

            return BadRequest(result.Errors);
        }

        // Log a user in
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (result.Succeeded)
            {
                return Ok("Logged in");
            }

            return Unauthorized();
        }

        // Log a user out
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }

        // Give a user the DLC claim (such that user can access content)
        [Authorize]
        [HttpPost("grant-dlc")]
        public async Task<IActionResult> GrantDlc()
        {
            var user = await _userManager.GetUserAsync(User);
            await _userManager.AddClaimAsync(user, new Claim("hasDLC", "true"));
            return Ok("DLC granted");
        }
    }
}
