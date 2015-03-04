using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace SatelliteService
{
    public class ConfigSourceValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (ConfigurationManager.AppSettings["Authorization.UserName"] != userName)
            {
                throw new SecurityTokenException("Invalid credentials");
            }

            if (ConfigurationManager.AppSettings["Authorization.Password"] != password)
            {
                throw new SecurityTokenException("Invalid credentials");
            }
        }
    }
}