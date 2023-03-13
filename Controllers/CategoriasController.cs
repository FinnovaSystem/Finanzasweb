using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        //region[blue] //!inyeccion
        public CategoriasController(
            IRepositorioCategorias repositorioCategorias,
            IServicioUsuarios servicioUsuarios
        )
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }//endregion

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoria(Categoria categoria){
            if (!ModelState.IsValid) { return View(categoria);}
            
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            
            categoria.UsuarioId = usuarioId;

            await repositorioCategorias.Crear(categoria);

            return RedirectToAction("Index");

        }
    }
}