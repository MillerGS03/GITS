using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GITS
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/login"),
                SlidingExpiration = true
            });
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "1093444619972-q3so85moggccevhjjn64hr7sqa1rih7b.apps.googleusercontent.com",
                ClientSecret = "xlwPW2pLV861Q_VmDdXnsyxX",
                CallbackPath = new PathString("/GoogleLoginCallback"),
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new Claim("urn:google:accesstoken", context.AccessToken,
                            ClaimValueTypes.String, "Google"));
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}