using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GMS_BE.Data.DTOs.Account;
using GMS_BE.Models;
using GMS_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GMS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(JWTService jWtService, 
        SignInManager<User> signInManager, 
        UserManager<User> userManager)
        {
            _jwtService = jWtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null) return Unauthorized("Invalid username or Password");

            if(user.EmailConfirmed == false) return Unauthorized("Please conform your email.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if(!result.Succeeded) return Unauthorized("Invalid username or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if(await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest($"An existing account isusing {model.Email}, email address. Please try with another email address");
            }

            var userToAdd = new User
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Your account has been created, you can login");
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }


        #region Private helper methods
        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                Lastname = user.LastName,
                JWT = _jwtService.CreateJWT(user)
            };
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
        #endregion
        
    }
}