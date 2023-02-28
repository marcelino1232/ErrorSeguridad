using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly ITipoCuentaRepository tipoCuentaRepository;
        private readonly IServicoUsuario servicoUsuario;
        private readonly ICuentaRepository cuentaRepository;
        private readonly IMapper mapper;

        public CuentasController(ITipoCuentaRepository tipoCuentaRepository,IServicoUsuario servicoUsuario, ICuentaRepository cuentaRepository , IMapper mapper)
        {
            this.tipoCuentaRepository = tipoCuentaRepository;
            this.servicoUsuario = servicoUsuario;
            this.cuentaRepository = cuentaRepository;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var cuentasConTipoCuentas = await cuentaRepository.GetAll(usuarioId);

            var modelo = cuentasConTipoCuentas.GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentaViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var modelo = new CuentaCreacionViewModel();

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);


            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var tipoCuenta = await tipoCuentaRepository.GetById(cuenta.TipoCuentaId , usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await cuentaRepository.Crear(cuenta);

           return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await tipoCuentaRepository.GetAll(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
      
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var cuentas = await cuentaRepository.GetById(id, usuarioId);


            if(cuentas is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuentas);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var cuentas = await cuentaRepository.GetById(cuentaEditar.Id, usuarioId);


            if (cuentas is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await tipoCuentaRepository.GetById(cuentaEditar.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            //if (!ModelState.IsValid)
            //{
            //    var modelo = new CuentaCreacionViewModel()
            //    {
            //        Id = cuentaEditar.Id,
            //        TipoCuentaId = cuentaEditar.TipoCuentaId,
            //        Nombre = cuentaEditar.Nombre,
            //        Balance = cuentaEditar.Balance,
            //        Descripcion = cuentaEditar.Descripcion
            //    };

            //    modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);


            //    return View(modelo);
            //}

            await cuentaRepository.Update(cuentaEditar);
            

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var cuenta = await cuentaRepository.GetById(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            var cuenta = await cuentaRepository.GetById(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await cuentaRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
