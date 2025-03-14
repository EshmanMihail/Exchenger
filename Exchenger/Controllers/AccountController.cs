using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Exchenger.Models;
using Microsoft.EntityFrameworkCore;

namespace Exchenger.Controllers
{
    public class AccountController : Controller
    {
        private readonly ExchengerDbContext db;

        public AccountController(ExchengerDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string phoneNumber)
        {
            var user = db.Customers.Include(c => c.Role).FirstOrDefault(c => c.Email == email && c.PhoneNumber == phoneNumber);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                Response.Cookies.Append("UserId", user.Id.ToString());

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }
    }
}
