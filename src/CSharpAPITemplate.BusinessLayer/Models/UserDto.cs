using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Models;

/// <inheritdoc cref="User"/>
public class UserDto : BaseEntityDto
{
    /// <inheritdoc cref="User.FirstName"/>
    public string? FirstName { get; set; }
        
    /// <inheritdoc cref="User.LastName"/>
    public string? LastName { get; set; }
        
    /// <inheritdoc cref="User.Email"/>
    public string? Email { get; set; }
        
    /// <inheritdoc cref="User.TemporaryEmail"/>
    public string? TemporaryEmail { get; set; }
        
    /// <inheritdoc cref="User.EmailConfirmationRequestDate"/>
    public DateTime? EmailConfirmationRequestDate { get; set; }
    
    /// <inheritdoc cref="User.PasswordChangeRequestDate"/>
    public DateTime? PasswordChangeRequestDate { get; set; } 

    /// <inheritdoc cref="User.Phone"/>
    public string? Phone { get; set; }
}