using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin; 

namespace CustomGoogleLogin
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/Index"),
                SlidingExpiration = true
            });
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "1093444619972-q3so85moggccevhjjn64hr7sqa1rih7b.apps.googleusercontent.com",
                ClientSecret = "xlwPW2pLV861Q_VmDdXnsyxX",
                CallbackPath = new PathString("/GoogleLoginCallback")
            });
        }
    }
}
