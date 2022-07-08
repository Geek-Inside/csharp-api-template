using Newtonsoft.Json;

namespace CSharpAPITemplate.Domain.Auth
{
    /// <summary>
    /// Credentials for changing forgotten password.
    /// Requires token that generates in password forgot request.
    /// </summary>
    public class ForgotPasswordCredentials
    {
        /// <summary>
        /// Encrypted token with user id.
        /// </summary>
        [JsonRequired]
        public string Token { get; set; }
        
        /// <summary>
        /// New user password.
        /// </summary>
        [JsonRequired]
        public string NewPassword { get; set; }
 
        /// <summary>
        /// New user password confirmation. Should be equal to <see cref="NewPassword"/>
        /// </summary>
        [JsonRequired]
        public string NewPasswordConfirm { get; set; }
    }
}