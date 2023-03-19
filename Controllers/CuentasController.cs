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
        private readonly ITransaccionesRepository transaccionesRepository;

        public CuentasController(ITipoCuentaRepository tipoCuentaRepository,IServicoUsuario servicoUsuario, ICuentaRepository cuentaRepository 
            , IMapper mapper, ITransaccionesRepository transaccionesRepository)
        {
            this.tipoCuentaRepository = tipoCuentaRepository;
            this.servicoUsuario = servicoUsuario;
            this.cuentaRepository = cuentaRepository;
            this.mapper = mapper;
            this.transaccionesRepository = transaccionesRepository;
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

        public async Task<IActionResult> Detalle(int id, int mes , int ano)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var cuenta = await cuentaRepository.GetById(id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            DateTime FechaInicio;
            DateTime FechaFin;

            if (mes <= 0 || mes > 12 || ano <= 1990)
            {
                var hoy = DateTime.Today;
                FechaInicio = new DateTime(hoy.Year,hoy.Month,1);
            }
            else
            {
                FechaInicio = new DateTime(ano, mes, 1);
            }

            FechaFin = FechaInicio.AddMonths(1).AddDays(-1);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = id,
                UsuarioId = usuarioId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var transacciones = await transaccionesRepository
                .GetAllByCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = new ReporteTransaccionesDetallas();
            ViewBag.Cuenta = cuenta.Nombre;


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetallas.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.transaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = FechaInicio;
            
            modelo.FechaFin = FechaFin;


            ViewBag.mesAnterior = FechaInicio.AddMonths(-1).Month;
            ViewBag.anoAnterior = FechaInicio.AddMonths(-1).Year;

            ViewBag.mesPosterior = FechaInicio.AddMonths(1).Month;
            ViewBag.anoPosterior = FechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;
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
