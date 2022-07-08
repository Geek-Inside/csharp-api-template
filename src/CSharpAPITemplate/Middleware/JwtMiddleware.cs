using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CSharpAPITemplate.BusinessLayer.Services.Users;
using Microsoft.IdentityModel.Tokens;

namespace CSharpAPITemplate.Middleware;

public class JwtMiddleware
{
	private readonly RequestDelegate _next;
	private IConfiguration _configuration;
	private IUserService _userService;

	public JwtMiddleware(
		RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context, IConfiguration configuration, IUserService userService)
	{
		_configuration = configuration;
		_userService = userService;

		var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

		if (token != null)
			AttachUserIdToContext(context, token);

		await _next(context);
	}

	private void AttachUserIdToContext(HttpContext context, string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var secret = Encoding.ASCII.GetBytes(_configuration.GetSection("Setup:JwtSecret").Value);
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(secret),
				ValidateIssuer = false,
				ValidateAudience = false,
				// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			var jwtToken = (JwtSecurityToken) validatedToken;
			var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
			var roles = jwtToken.Claims.First(x => x.Type == "roles").Value;

			var userResult = _userService.CheckIfUserExists(long.Parse(userId));
			if (userResult.IsSuccessStatusCode)
			{
				context.Items["UserId"] = userId;
				context.Items["Roles"] = roles;
			}
		}
		catch
		{
			// Do nothing if jwt validation fails,
			// user is not attached to context so request won't have access to secure routes
		}
	}
}