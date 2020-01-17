using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HW17.DataAccess;
using HW17.Models;
using HW17.Services;

namespace HW17.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Login,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                if (await authService.RegistrateUser(user.Login, user.Password))
                {
                    RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([Bind("Login,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                if(await authService.AuthenticateUser(user.Login, user.Password))
                {
                    RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

    }
}
