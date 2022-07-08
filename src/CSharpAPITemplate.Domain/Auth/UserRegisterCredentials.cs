using Newtonsoft.Json;

namespace CSharpAPITemplate.Domain.Auth
{
    public class UserRegisterCredentials
    {
        [JsonRequired]
        public string Email { get; set; }

        [JsonRequired]
        public string Password { get; set; }
 
        [JsonRequired]
        public string PasswordConfirm { get; set; }
    }
}