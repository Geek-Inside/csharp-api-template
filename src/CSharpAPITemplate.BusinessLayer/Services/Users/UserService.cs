#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Extensions;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Data;
using CSharpAPITemplate.Domain.Auth;
using CSharpAPITemplate.Domain.Entities;
using CSharpAPITemplate.Domain.Enums;
using CSharpAPITemplate.Infrastructure.Encryption;
using CSharpAPITemplate.Infrastructure.Results;
using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using SkillZilla.BusinessLayer.SendGrid.TemplateData;

namespace CSharpAPITemplate.BusinessLayer.Services.Users;

public class UserService : BaseService<User, UserDto>, IUserService
{
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _sendGridClient;
        
    public UserService(
        ApplicationDbContext database,
        IMapper mapper,
        ILogger<UserService> logger,
        IConfiguration configuration,
        ISendGridClient sendGridClient) : base(database, mapper, logger)
    {
        _configuration = configuration;
        _sendGridClient = sendGridClient;
    }
        
    public async Task<Result<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest authRequest, CancellationToken cancellationToken)
    {
        var user = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(x => x.Email == authRequest.Email.ToLowerInvariant(), cancellationToken: cancellationToken);
        if (user == null) 
            return BlResult<AuthenticateResponse>.NotFound($"User with email {authRequest.Email.ToLowerInvariant()} not found.");

        var passHasher = new PasswordHasher<object?>();
        var checkResult = passHasher.VerifyHashedPassword(null, user.HashedPassword ?? string.Empty, authRequest.Password ?? string.Empty);
        if (checkResult == PasswordVerificationResult.Failed)
            return BlResult<AuthenticateResponse>.NotFound($"Invalid email or password.");
            
        var token = GenerateJwtToken(user);
        var result = new AuthenticateResponse(user, token);
        return BlResult<AuthenticateResponse>.Ok(result);
    }
        
    public async Task<Result<AuthenticateResponse>> RegisterAsync(UserRegisterCredentials credentials, CancellationToken cancellationToken)
    {
        var checkResult = await CheckEmailAsync(credentials.Email.ToLowerInvariant(), cancellationToken);
        if (!checkResult.IsSuccessStatusCode)
            return checkResult.ToResult<AuthenticateResponse>();

        if (string.IsNullOrWhiteSpace(credentials.Password))
            return BlResult<AuthenticateResponse>.BadRequest("Password should not be empty.");

        if (credentials.Password != credentials.PasswordConfirm)
            return BlResult<AuthenticateResponse>.BadRequest("Passwords are not equal.");
            
        var hashedPassword = new PasswordHasher<object?>().HashPassword(null, credentials.Password);
        var newUser = new User
        {
            TemporaryEmail = credentials.Email.ToLowerInvariant(),
            HashedPassword = hashedPassword,
            Roles = UserRoles.User
        };
            
        await Database.Users.AddAsync(newUser, cancellationToken);
        await Database.SaveChangesAsync(cancellationToken);

        await RequestEmailConfirmationAsync(newUser.Id, cancellationToken);
            
        var token = GenerateJwtToken(newUser);
        var result = new AuthenticateResponse(newUser, token);
        return BlResult<AuthenticateResponse>.Ok(result);
    }

    /// <summary>
    /// Checks if email already in use or incorrect.
    /// </summary>
    /// <param name="email">Email that we are checking.</param>
    /// <returns>Conflict result if in use, BadRequest if incorrect, NoContent otherwise.</returns>
    public async Task<Result<UserDto>> CheckEmailAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.IsValidEmail())
            return BlResult<UserDto>.BadRequest("Email incorrect.");

        var isEmailUsed = await Database.Users
            .OnlyActive()
            .AnyAsync(i => i.Email == email, cancellationToken: cancellationToken);
        if (isEmailUsed)
            return BlResult<UserDto>.BadRequest("Email already in use.");

        return BlResult<UserDto>.NoContent();
    }

    public async Task<Result<UserDto>> RequestPasswordForgotAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BlResult<UserDto>.NotFound("Incorrect email.");
            
        var user = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(i => i.Email == email || i.TemporaryEmail == email, cancellationToken: cancellationToken);

        if (user is null)
            return BlResult<UserDto>.NotFound($"There is no user with such email.");

        if (string.IsNullOrWhiteSpace(user.Email) || user.Email != email)
            return BlResult<UserDto>.BadRequest("User should have approved email to change password.");

        var timePassedFromLastRequest = DateTime.UtcNow - user.PasswordChangeRequestDate;
        if (user.PasswordChangeRequestDate != null && timePassedFromLastRequest.HasValue && timePassedFromLastRequest.Value.Minutes < TimeSpan.FromMinutes(1).Minutes)
            return BlResult<UserDto>.BadRequest("Too frequent password change requests. Wait 1 minute between requests.");

        user.PasswordChangeRequestDate = DateTime.UtcNow;

        // Send actual email message though SendGrid API.
        var token = GenerateEncryptedDataToken(user.Id.ToString());
        var urlEncodedToken = WebUtility.UrlEncode(token);

        var uiAppUrl = _configuration.GetSection("Setup:Endpoints:UIApp:Url").Value;
        var senderEmail = _configuration.GetSection("Setup:SendGrid:FromEmail").Value;
        var templateId = _configuration.GetSection("Setup:SendGrid:Templates:ResetPassword").Value;
        var userName = string.IsNullOrWhiteSpace($"{user.FirstName} {user.LastName}".Trim())
            ? "Dear User"
            : $"{user.FirstName} {user.LastName}".Trim();
        var msg = new SendGridMessage();
        msg.SetSubject("Password Reset");
        msg.SetFrom(senderEmail, "API Example");
        msg.AddTo(user.Email, userName);
        msg.SetTemplateId(templateId);
        msg.SetTemplateData(new PasswordResetTemplateData($"{uiAppUrl}/reset-password/{urlEncodedToken}"));
            
        var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Accepted)
            return BlResult<UserDto>.BadRequest("Filed to send email.");

        await Database.SaveChangesAsync(cancellationToken);
        return BlResult<UserDto>.NoContent();
    }

    public async Task<Result<UserDto>> ConfirmEmailAsync(string token, CancellationToken cancellationToken)
    {
        EmailConfirmationCredentials? credentials;
        try
        {
            var aesSecret = _configuration.GetSection("Setup:AesSecret").Value;
            var decryptedToken = AESGCM.SimpleDecryptWithPassword(token, aesSecret);
            credentials = JsonConvert.DeserializeObject<EmailConfirmationCredentials>(decryptedToken);
        }
        catch (Exception e)
        {
            return BlResult<UserDto>.BadRequest("Incorrect token.");
        }
            
        if (credentials == null)
            return BlResult<UserDto>.BadRequest("Incorrect token.");

        var isTokenCorrect = long.TryParse(credentials.UserId, out var userId);
        if (!isTokenCorrect)
            return BlResult<UserDto>.BadRequest("Incorrect token.");

        var user = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
            return BlResult<UserDto>.BadRequest("Incorrect token.");

        if (user.EmailConfirmationRequestDate == null && !string.IsNullOrEmpty(user.Email))
            return BlResult<UserDto>.BadRequest("Your email already confirmed.");

        if (user.TemporaryEmail != credentials.Email || user.EmailConfirmationRequestDate == null)
            return BlResult<UserDto>.BadRequest("Email confirmation failed. Try to resent email confirmation.");

        user.Email = user.TemporaryEmail;
        user.TemporaryEmail = null;
        user.EmailConfirmationRequestDate = null;
        await Database.SaveChangesAsync(cancellationToken);
        
        return BlResult<UserDto>.NoContent();
    }

    public async Task<Result<UserDto>> RequestEmailConfirmationAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
            return BlResult<UserDto>.NotFound($"There is no user with such email.");

        if (string.IsNullOrEmpty(user.TemporaryEmail) && !string.IsNullOrEmpty(user.Email))
            return BlResult<UserDto>.BadRequest($"Email already confirmed.");

        if (string.IsNullOrEmpty(user.TemporaryEmail))
            return BlResult<UserDto>.BadRequest($"Email for confirmation is empty.");

        // Set 1 minute delay between requests.
        var timePassedFromLastRequest = DateTime.UtcNow - user.EmailConfirmationRequestDate;
        if (user.EmailConfirmationRequestDate != null && timePassedFromLastRequest.HasValue && timePassedFromLastRequest.Value.Minutes < TimeSpan.FromMinutes(1).Minutes)
            return BlResult<UserDto>.BadRequest("Too frequent password change requests. Wait 1 minute between requests.");

        user.EmailConfirmationRequestDate = DateTime.UtcNow;

        // Send actual email message though SendGrid API.
        var tokenData =
            JsonConvert.SerializeObject(new EmailConfirmationCredentials(user.Id.ToString(), user.TemporaryEmail));
        var token = GenerateEncryptedDataToken(tokenData);
        var urlEncodedToken = WebUtility.UrlEncode(token);

        var uiAppUrl = _configuration.GetSection("Setup:Endpoints:UIApp:Url").Value;
        var senderEmail = _configuration.GetSection("Setup:SendGrid:FromEmail").Value;
        var templateId = _configuration.GetSection("Setup:SendGrid:Templates:EmailConfirmation").Value;
        var msg = new SendGridMessage();
        msg.SetSubject("Email Confirmation");
        msg.SetFrom(senderEmail, "API Example");
        msg.AddTo(user.TemporaryEmail, "Dear User");
        msg.SetTemplateId(templateId);
        msg.SetTemplateData(new EmailConfirmationTemplateData($"{uiAppUrl}/email-confirmation/{urlEncodedToken}"));
            
        var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Accepted)
            return BlResult<UserDto>.BadRequest("Filed to send email.");

        await Database.SaveChangesAsync(cancellationToken);
        return BlResult<UserDto>.NoContent();
    }

    /// <summary>
    /// Changes password with unforgotten current password.
    /// </summary>
    public async Task<Result<UserDto>> ChangePasswordAsync(ChangePasswordCredentials credentials, long userId, CancellationToken cancellationToken)
    {
        var currentUser = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken: cancellationToken);

        if (currentUser is null)
            return BlResult<UserDto>.NotFound($"Current user not found.");

        if (string.IsNullOrWhiteSpace(credentials.Password))
            return BlResult<UserDto>.BadRequest($"Invalid password.");
            
        if (string.IsNullOrWhiteSpace(credentials.NewPassword) || string.IsNullOrWhiteSpace(credentials.NewPasswordConfirm))
            return BlResult<UserDto>.BadRequest("New password should not be empty.");
            
        var passHasher = new PasswordHasher<object?>();
        var checkResult = passHasher.VerifyHashedPassword(null, currentUser.HashedPassword ?? string.Empty, credentials.Password ?? string.Empty);
        if (checkResult == PasswordVerificationResult.Failed)
            return BlResult<UserDto>.BadRequest($"Invalid password.");

        if (!credentials.NewPassword.Equals(credentials.NewPasswordConfirm, StringComparison.InvariantCulture))
            return BlResult<UserDto>.BadRequest($"Passwords are not equal.");
            
        currentUser.HashedPassword = new PasswordHasher<object?>().HashPassword(null, credentials.NewPassword);
        await Database.SaveChangesAsync(cancellationToken);
        
        return BlResult<UserDto>.NoContent();
    }
        
    /// <summary>
    /// Changes forgotten password after email confirmation. 
    /// </summary>
    public async Task<Result<UserDto>> ForgotPasswordChangeAsync(ForgotPasswordCredentials credentials, CancellationToken cancellationToken)
    {
        var aesSecret = _configuration.GetSection("Setup:AesSecret").Value;
        var decryptedToken = AESGCM.SimpleDecryptWithPassword(credentials.Token, aesSecret);
        var isTokenCorrect = long.TryParse(decryptedToken, out var userId);
        if (!isTokenCorrect)
            return BlResult<UserDto>.BadRequest("Incorrect token.");

        var user = await Database.Users
            .OnlyActive()
            .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
            return BlResult<UserDto>.BadRequest("Incorrect token.");
            
        if (user.PasswordChangeRequestDate == null)
            return BlResult<UserDto>.BadRequest("Password already changed.");

        if (string.IsNullOrWhiteSpace(credentials.NewPassword) || string.IsNullOrWhiteSpace(credentials.NewPasswordConfirm))
            return BlResult<UserDto>.BadRequest("New password should not be empty.");
            
        if (!credentials.NewPassword.Equals(credentials.NewPasswordConfirm, StringComparison.InvariantCulture))
            return BlResult<UserDto>.BadRequest("Passwords are not equal.");
            
        user.HashedPassword = new PasswordHasher<object?>().HashPassword(null, credentials.NewPassword);
        user.PasswordChangeRequestDate = null;
        await Database.SaveChangesAsync(cancellationToken);
        
        return BlResult<UserDto>.NoContent();
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(long userId, CancellationToken cancellationToken)
    {
        return await GetAsync(userId, cancellationToken: cancellationToken);
    }
        
    public Result<UserDto> CheckIfUserExists(long userId)
    {
        var isExists = Database.Users.Any(i => i.Id == userId);
        return isExists 
            ? BlResult<UserDto>.NoContent()
            : BlResult<UserDto>.NotFound("User doesn't exists.");
    }

    /// <summary>
    /// Generates new JWT token for user auth.
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Setup:JwtSecret").Value);
        var expireMinutes = double.Parse(_configuration.GetSection("Setup:JwtExpireInMinutes").Value);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("roles", user.Roles.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates encrypted token with string data in it.
    /// </summary>
    /// <param name="data">String data that should be encrypted in token.</param>
    /// <returns>Encrypted string that contains data in payload.</returns>
    private string GenerateEncryptedDataToken(string data)
    {
        var key = AESGCM.NewKey();
        var aesSecret = _configuration.GetSection("Setup:AesSecret").Value;
        return AESGCM.SimpleEncryptWithPassword(data, aesSecret);
    }
}