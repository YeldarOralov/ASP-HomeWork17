using HW17.DataAccess;
using HW17.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HW17.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DataContext context;

        public AuthService(IHttpContextAccessor httpContext, DataContext context)
        {
            this.httpContextAccessor = httpContext;
            this.context = context;
        }

        public async Task<bool> AuthenticateUser(string login, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Login == login && x.Password == password);

            if (user is null) return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return true;
        }

        public async Task<bool> RegistrateUser(string login, string password)
        {
            await context.Users.AddAsync(new User { Login = login, Password = password });
            await context.SaveChangesAsync();

            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return true;
        }

        public async void SignOutUser()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public string DecryptClaim()
        {
            // Get the encrypted cookie value
            var opt = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions>>();
            var cookie = opt.CurrentValue.CookieManager.GetRequestCookie(httpContextAccessor.HttpContext, ".AspNetCore.Cookies");

            // Decrypt if found
            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                var claims = ticket.Principal.Claims;
                var list = claims.ToList();

                return list.FirstOrDefault().Value;
            }
            return null;
        }
    }
}
