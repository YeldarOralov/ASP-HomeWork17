using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HW17.Models;
using Microsoft.AspNetCore.Authorization;
using HW17.Services;

namespace HW17.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthService authService;

        public HomeController(ILogger<HomeController> logger, AuthService authService)
        {
            _logger = logger;
            this.authService = authService;
        }

        public IActionResult Index()
        {
            ViewBag.Claim = authService.DecryptClaim();
            return View();
        }
        public IActionResult LogOut()
        {
            authService.SignOutUser();
            return RedirectToAction("Index", "Auth");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
