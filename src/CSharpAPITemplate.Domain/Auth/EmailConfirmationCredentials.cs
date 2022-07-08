using Newtonsoft.Json;

namespace CSharpAPITemplate.Domain.Auth
{
    /// <summary>
    /// Credentials for email changing.
    /// </summary>
    public class EmailConfirmationCredentials
    {
        public EmailConfirmationCredentials()
        {
        }
        
        public EmailConfirmationCredentials(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        /// <summary>
        /// User id in string representation.
        /// </summary>
        [JsonRequired]
        public string UserId { get; set; }
        
        /// <summary>
        /// Email for confirmation.
        /// </summary>
        [JsonRequired]
        public string Email { get; set; }
    }
}