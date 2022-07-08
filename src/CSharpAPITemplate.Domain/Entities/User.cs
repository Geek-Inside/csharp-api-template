using System.Text.Json.Serialization;
using CSharpAPITemplate.Domain.Common;

namespace CSharpAPITemplate.Domain.Entities;

/// <summary>
/// Represent user entity.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User first name.
    /// </summary>
    public string? FirstName { get; set; }
        
    /// <summary>
    /// User second name.
    /// </summary>
    public string? LastName { get; set; }
        
    /// <summary>
    /// User approved email.
    /// </summary>
    public string? Email { get; set; }
        
    /// <summary>
    /// User unapproved email.
    /// Email stores here when user in process of changing email or didn't approve his first email.
    /// </summary>
    public string? TemporaryEmail { get; set; }
        
    /// <summary>
    /// Time when email confirmation requested. Null if user already confirm email or didn't request email confirm.
    /// </summary>
    public DateTime? EmailConfirmationRequestDate { get; set; }
    
    /// <summary>
    /// Time when password change requested by user. Null if user already changed password or didn't request password change.
    /// </summary>
    public DateTime? PasswordChangeRequestDate { get; set; } 

    /// <summary>
    /// User telephone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// User roles. Seperated by comma.
    /// </summary>
    public string? Roles { get; set; }
        
    /// <summary>
    /// Encrypted password of user.
    /// </summary>
    [JsonIgnore]
    public string HashedPassword { get; set; }
}