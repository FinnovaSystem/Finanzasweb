using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ManejoPresupuesto.Controllers
{
    // [Route("[controller]")]
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        //? INYECCION
        public CuentasController(
            IRepositorioTiposCuentas repositorioTiposCuentas, 
            IRepositorioCuentas repositorioCuentas, 
            IServicioUsuarios servicioUsuarios,
            IMapper mapper
        )
        {
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioCuentas = repositorioCuentas;
        }//? FIN INYECCION

        [HttpGet]
        public async Task<IActionResult> Index(){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy( x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                    {
                        TipoCuenta = grupo.Key, // este key es "TipoCuenta", el valor qe se utilizó para realizar el groupby
                        Cuentas    = grupo.AsEnumerable()// se obtiene el IEnumerable de las cuentas pertenecientes al key
                    }
                ).ToList();
            
            return View(modelo);
        }

       [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);

            var modelo = new CuentaCreacionViewModel();

            //? creacion y rellenado de combobox con selectListItem
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            // modelo.TiposCuentas = tiposCuentas.Select( x => new SelectListItem(x.Nombre, x.Id.ToString()));


            return View(modelo);
        }

        // nota: para crear una cuenta DEBE EXISTIR un tipo cuenta
        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuentaVM){

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaVM.TipoCuentaId, usuarioId);

            if (tipoCuenta is null){ return RedirectToAction("NoEncontrado","Home"); }

            // si el form no es valido, 
            if(!ModelState.IsValid){
                cuentaVM.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuentaVM);
            }

            await repositorioCuentas.Crear(cuentaVM);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id,usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            //? para editar cuenta se puede usar el mismo viewmodel qe se usa para crear cuenta
            //? ya qe cuenta con los mismos campos

            // mapeo MANUAL del modelo con los datos actuales de la cuenta en cuestion
            // var modelo = new CuentaCreacionViewModel(){ 
            //     Id = cuenta.Id,
            //     Nombre = cuenta.Nombre,
            //     TipoCuentaId = cuenta.TipoCuentaId,
            //     Balance = cuenta.Balance,
            //     Descripcion = cuenta.Descripcion,
            // };
            
            //? mapeo automatico con automapper
            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            //?recordar qe viewmodel cuenta con una prop Enumerable qe guarda todos los "tiposcuentas" creados por el usuario
            //? al cargar pantalla de edicion se deben renderizar todos los tiposCuentas del usuario para que pueda decidir
            //? si actualizar el tipocuenta de su cuenta en cuestion
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            // retorno de la vista con los datos de la cuenta a editar
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaVM){

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            // verificar qe cuenta a editar efectivamente exista
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaVM.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            // verificar que exista el tipocuenta enviado por frontend
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaVM.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            await repositorioCuentas.Actualizar(cuentaVM);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            // valida qe exista cuenta pedida x frontend y qe pertenezca al usuario en cuestion
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id){
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            // valida qe exista cuenta pedida x frontend y qe pertenezca al usuario en cuestion
            var cuenta = repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }

            await repositorioCuentas.Borrar(id);

            return RedirectToAction("Index");

        }


        //? para rellenar el select "tipocuenta" dinamicamente al crear una nueva cuenta por frontend
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId){
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select( x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

    }
}