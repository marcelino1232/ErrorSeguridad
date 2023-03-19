using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;

        public UsuariosController(UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            return RedirectToAction("Index", "Transacciones");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel registro)
        {

            if (!ModelState.IsValid)
            {
                return View(registro);
            }

            var usuario = new Usuario()
            {
                Email = registro.Email
            };

            var resultado = await userManager.CreateAsync(usuario, password: registro.Password);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registro);
            }
        }

        
    }
}
