using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementApp.Model;
using Microsoft.AspNetCore.Identity;
using InventoryManagementApp.DTOs;

namespace InventoryManagementApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(new { message = "Пользователь успешно зарегестрирован" });
        }   
    }
}
