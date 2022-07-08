using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.BusinessLayer.Services.Users;
using CSharpAPITemplate.Domain.Auth;
using CSharpAPITemplate.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAPITemplate.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : BaseController<UserDto>
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService,
            ILogger<UserController> logger) : base(userService, logger)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest authRequest, CancellationToken cancellationToken)
        {
            var result = await _userService.AuthenticateAsync(authRequest, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterCredentials credentials, CancellationToken cancellationToken)
        {
            var result = await _userService.RegisterAsync(credentials, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("password-change")]
        [JwtAuthorize]
        public async Task<IActionResult> PasswordChange([FromBody] ChangePasswordCredentials credentials, CancellationToken cancellationToken)
        {
            var result = await _userService.ChangePasswordAsync(credentials, UserId, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] string email, CancellationToken cancellationToken)
        {
            var result = await _userService.CheckEmailAsync(email, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] string token, CancellationToken cancellationToken)
        {
            var result = await _userService.ConfirmEmailAsync(token, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("forgot-password-change")]
        public async Task<IActionResult> ForgotPasswordChange([FromBody] ForgotPasswordCredentials credentials, CancellationToken cancellationToken)
        {
            var result = await _userService.ForgotPasswordChangeAsync(credentials, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("request-password-forgot/{email}")]
        public async Task<IActionResult> RequestPasswordForgot(string email, CancellationToken cancellationToken)
        {
            var result = await _userService.RequestPasswordForgotAsync(email, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPost("request-email-confirmation")]
        [JwtAuthorize]
        public async Task<IActionResult> RequestEmailConfirmation(CancellationToken cancellationToken)
        {
            var result = await _userService.RequestEmailConfirmationAsync(UserId, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpGet("current")]
        [JwtAuthorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var result = await _userService.GetCurrentUserAsync(UserId, cancellationToken);
            return result.ToActionResult();
        }
    }
}
