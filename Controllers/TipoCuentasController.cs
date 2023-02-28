using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class TipoCuentasController : Controller
    {
        private readonly ITipoCuentaRepository tipoCuentaRepository;
        private readonly IServicoUsuario servicoUsuario;

        public TipoCuentasController(ITipoCuentaRepository tipoCuentaRepository, IServicoUsuario servicoUsuario)
        {
            this.tipoCuentaRepository = tipoCuentaRepository;
            this.servicoUsuario = servicoUsuario;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var tiposCuentas = await tipoCuentaRepository.GetAll(usuarioId);
            return View(tiposCuentas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var TipoCuenta = await tipoCuentaRepository.GetById(id, usuarioId);

            if(TipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(TipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var tipoCuentaExiste = await tipoCuentaRepository.GetById(tipoCuenta.Id,usuarioId);
            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await tipoCuentaRepository.Update(tipoCuenta);
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicoUsuario.ObtenerUsuarioId();

            var yaExisteTipoCuenta = await tipoCuentaRepository.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            
            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), 
                    $"El Nombre {tipoCuenta.Nombre} ya existe");
                return View(tipoCuenta);
            }

            await tipoCuentaRepository.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var yaExisteTipocuenta = await tipoCuentaRepository.Existe(nombre, usuarioId);

            if (yaExisteTipocuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var tipoCuenta = await tipoCuentaRepository.GetById(id, usuarioId);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTipoCuenta(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var tipoCuenta = await tipoCuentaRepository.GetById(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await tipoCuentaRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var tipoCuentas = await tipoCuentaRepository.GetAll(usuarioId);

            var IdsTiposCuentas = tipoCuentas.Select(x => x.Id);

            var IdsTiposCuentasNoUsuario = ids.Except(IdsTiposCuentas).ToList();

            if(IdsTiposCuentasNoUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor,indice) =>
            new TipoCuenta() { Id = valor,Orden=indice + 1}).AsEnumerable();

            await tipoCuentaRepository.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
