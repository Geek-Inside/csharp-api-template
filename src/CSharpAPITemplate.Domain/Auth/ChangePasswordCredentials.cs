using Newtonsoft.Json;

namespace CSharpAPITemplate.Domain.Auth
{
    public class ChangePasswordCredentials
    {
        [JsonRequired]
        public string Password { get; set; }
        
        [JsonRequired]
        public string NewPassword { get; set; }
 
        [JsonRequired]
        public string NewPasswordConfirm { get; set; }
    }
}