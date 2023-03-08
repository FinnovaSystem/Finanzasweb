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

        public IActionResult Crear(){ return View(); }
        
        //!INDEX
        public async Task<IActionResult> Index(){
            var usuarioId  = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas =  await repositorioTiposCuentas.Obtener(usuarioId);

            return View(tiposCuentas);
        }

        // accion que reciba datos de form
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta){
            
            if (!ModelState.IsValid)
            {
                // retorna el form con la info que el
                // usuario ya tenia, para ahorrar tiempo
                // en rellenado
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

            Console.WriteLine("pasó!!");
            
            return View();
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

        


    }
}