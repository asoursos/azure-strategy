using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EasyTrade.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var claims = ((ClaimsIdentity) User.Identity).Claims;

            return View(claims);
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}