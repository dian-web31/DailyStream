using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DailyStream.Models;


namespace DailyStream.Controllers
{
    public class AccountController : Controller
    {
        private readonly DailystreamContext _context;

        public AccountController(DailystreamContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            // Buscar usuario existente
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            // Si no existe, crear nuevo usuario automáticamente
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Username = username,
                    Password = password // TODO: debería estar hasheado
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            // Crear cookie de autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.NameIdentifier, usuario.Idusuario.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Cookie persistente
            };

            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}