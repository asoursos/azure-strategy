using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EasyTrade.App.Controllers
{
    public class AccountController : Controller
    {
        //// GET: /Account/Login
        [HttpGet]
        public async Task SignUp()
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                await HttpContext.Authentication.ChallengeAsync(Startup.AdSettings.SignUpOrSignInPolicyId.ToLower(),
                    authenticationProperties);
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public async Task SignIn()
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                await HttpContext.Authentication.ChallengeAsync(Startup.AdSettings.SignUpOrSignInPolicyId.ToLower(),
                    authenticationProperties);
            }
        }

        // GET: /Account/LogOff
        [HttpGet]
        public async Task LogOff()
        {
            if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
            {
                // try to find the tfp policy id claim (default)
                var scheme = HttpContext.User.FindFirst("tfp")?.Value;

                // fall back to legacy acr policy id claim
                if (string.IsNullOrEmpty(scheme))
                    scheme = HttpContext.User.FindFirst("http://schemas.microsoft.com/claims/authnclassreference")
                        ?.Value;

                await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.Authentication.SignOutAsync(scheme.ToLower(),
                    new AuthenticationProperties { RedirectUri = "/" });
            }
        }

        [EnableCors(ADSettings.AllowSpecificOrigin)]
        [HttpGet]
        [HttpOptions]
        public IActionResult SignUpOrSignInUniversal()
        {
            return View();
        }

        public async Task EditProfile()
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                await SignIn();
                return;
            }

            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            await HttpContext.Authentication.ChallengeAsync(Startup.AdSettings.UserProfilePolicyId.ToLower(),
                authenticationProperties);
        }

        [EnableCors(ADSettings.AllowSpecificOrigin)]
        [HttpGet]
        [HttpOptions]
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Claims()
        {
            return View();
        }
    }
}