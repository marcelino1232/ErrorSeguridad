using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicoUsuario servicoUsuario;
        private readonly ITransaccionesRepository transaccionesRepository;
        private readonly ICategoriaRepository categoriaRepository;
        private readonly ICuentaRepository cuentaRepository;
        private readonly IMapper mapper;

        public TransaccionesController(IServicoUsuario servicoUsuario,ITransaccionesRepository transaccionesRepository,ICategoriaRepository categoriaRepository,ICuentaRepository cuentaRepository
            ,IMapper mapper)
        {
            this.servicoUsuario = servicoUsuario;
            this.transaccionesRepository = transaccionesRepository;
            this.categoriaRepository = categoriaRepository;
            this.cuentaRepository = cuentaRepository;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(int mes, int ano)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            DateTime FechaInicio;
            DateTime FechaFin;

            if (mes <= 0 || mes > 12 || ano <= 1990)
            {
                var hoy = DateTime.Today;
                FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                FechaInicio = new DateTime(ano, mes, 1);
            }

            FechaFin = FechaInicio.AddMonths(1).AddDays(-1);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var transacciones = await transaccionesRepository.GetAllByUsuarioId(parametro);


            var modelo = new ReporteTransaccionesDetallas();


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

   
        private async Task<IEnumerable<SelectListItem>> ObtenerCuenta(int usuarioId)
        {
            var cuentas = await cuentaRepository.GetAll(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categoria = await categoriaRepository.GetAll(usuarioId, tipoOperacion);
            return categoria.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategoria([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuenta(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);

            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuenta(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await cuentaRepository.GetById(modelo.CuentaId, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await categoriaRepository.GetById(modelo.CategoriaId, usuarioId);

            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await transaccionesRepository.Crear(modelo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var Transaccion = await transaccionesRepository.GetById(id, usuarioId);


            if (Transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActializarViewModel>(Transaccion);
          
            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = Transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            modelo.Cuentas =  await ObtenerCuenta(usuarioId);
            modelo.UrlRetorno = urlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActializarViewModel modelo)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                modelo.Cuentas = await ObtenerCuenta(usuarioId);

                return View(modelo);
            }

            var cuenta = await cuentaRepository.GetById(modelo.CuentaId, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await categoriaRepository.GetById(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transacciones>(modelo);

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await transaccionesRepository.Actualizar(transaccion,modelo.MontoAnterior,modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string urlRetorno = null)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var Transaccion = await transaccionesRepository.GetById(id, usuarioId);

            if (Transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await transaccionesRepository.Delete(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }

    }
}
