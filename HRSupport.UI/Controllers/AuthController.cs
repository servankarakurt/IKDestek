using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return RedirectToAction("Index", "Home");
            }

            // Basit rol seçimi – gerçek kimlik doğrulama ileride eklenebilir
            HttpContext.Session.SetString("Role", role);

            return role switch
            {
                "HR" => RedirectToAction("Index", "Home"),
                "Manager" => RedirectToAction("Index", "LeaveRequest"),
                "Employee" => RedirectToAction("Index", "LeaveRequest"),
                "Intern" => RedirectToAction("Index", "LeaveRequest"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

