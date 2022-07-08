using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Domain.Auth;
using CSharpAPITemplate.Infrastructure.Results.Base;

namespace CSharpAPITemplate.BusinessLayer.Services.Users;

public interface IUserService : IBaseService<UserDto>
{
    Task<Result<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest authRequest, CancellationToken cancellationToken);
    Task<Result<AuthenticateResponse>> RegisterAsync(UserRegisterCredentials credentials, CancellationToken cancellationToken);
    Task<Result<UserDto>> CheckEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<UserDto>> RequestPasswordForgotAsync(string email, CancellationToken cancellationToken);
    Task<Result<UserDto>> ConfirmEmailAsync(string token, CancellationToken cancellationToken);
    Task<Result<UserDto>> RequestEmailConfirmationAsync(long userId, CancellationToken cancellationToken);
    Task<Result<UserDto>> ChangePasswordAsync(ChangePasswordCredentials credentials, long userId, CancellationToken cancellationToken);
    Task<Result<UserDto>> ForgotPasswordChangeAsync(ForgotPasswordCredentials credentials, CancellationToken cancellationToken);
    Task<Result<UserDto>> GetCurrentUserAsync(long userId, CancellationToken cancellationToken);
    Result<UserDto> CheckIfUserExists(long userId);
}