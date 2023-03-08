using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        //! INYECCION //region[blue] 
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
        IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }
        // endregion

        //! LISTAR
        public async Task<IActionResult> Index(){
            var usuarioId  = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas =  await repositorioTiposCuentas.Obtener(usuarioId);

            return View(tiposCuentas);
        }
        
        //! CREAR vista html delf orm 
        public IActionResult Crear(){ return View(); }
        
        //! CREAR (recibe datos del form de la vista TiposCuentas/Crear)
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta){
            
            if (!ModelState.IsValid)
            {
                // retorna form con la info que el user ya tenia, 
                // para ahorrar tiempo en nuevo rellenado
                Console.WriteLine("no pasó validación!!");
                return RedirectToAction("Index");
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();
            
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(
                    nameof(tipoCuenta.Nombre), // nameof para indicar a que campo prop pertenecerá el error validation
                    $"El nombre {tipoCuenta.Nombre} ya existe!"
                );

                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        // para cargar la pagina qe permite edicion
        [HttpGet]
        public async Task<IActionResult> Editar(int id){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)// usuario no tiene permiso para editar o no existe el tipoCuenta
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id,usuarioId);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);

            return RedirectToAction("Index");


        }

        [HttpGet]
        //! para validation Remote (se usará x frontend)
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre){

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);

            if (yaExisteTipoCuenta)
            {
                // json permitira llevar datos de back a front
                return Json($"El nombre {nombre} ya existe!");
            }

            return Json(true);
        }

        //! ACCION al mover las filas de la tabla tipocuentas
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody]int[] ids){

            var usuarioId =  servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            
            // extraccion de los ids de los tiposCuentas provenientees de bd
            var idsTiposCuentas = tiposCuentas.Select( x => x.Id);

            // valida qe los ids enviados por front coincidan con los ids extraidos por backend
            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => 
                                            new TipoCuenta(){Id = valor, Orden = indice + 1}).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }


        //! DELETE vista de confirmacion
        public async Task<IActionResult> Borrar (int id){
            var usuarioId =  servicioUsuarios.ObtenerUsuarioId();
            var tipocuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            return View(tipocuenta);
        }

        //! DELETE
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipocuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            await repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

    }
}