using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository categoriaRepository;
        private readonly IServicoUsuario servicoUsuario;

        public CategoriaController(ICategoriaRepository categoriaRepository,IServicoUsuario servicoUsuario)
        {
            this.categoriaRepository = categoriaRepository;
            this.servicoUsuario = servicoUsuario;
        }
        
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var categorias = await categoriaRepository.GetAll(usuarioId);

            return View(categorias);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await categoriaRepository.Crear(categoria);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var categoria = await categoriaRepository.GetById(id, usuarioId);

            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var valid = await categoriaRepository.GetById(categoria.Id, usuarioId);

            if (valid is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoria.UsuarioId = usuarioId;
            await categoriaRepository.Update(categoria);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var categoria = await categoriaRepository.GetById(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(Categoria categoria)
        {
            var usuarioId = servicoUsuario.ObtenerUsuarioId();
            var valid = await categoriaRepository.GetById(categoria.Id, usuarioId);

            if (valid is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await categoriaRepository.Delete(categoria.Id);

            return RedirectToAction("Index");
        }


    }
}
