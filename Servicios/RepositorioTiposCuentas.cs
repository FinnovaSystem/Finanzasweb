using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioTiposCuentas{

        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task Actualizar(TipoCuenta tipoCuenta);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Borrar(int id);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //! CREAR ------------------------------------------------------------------
        public async Task Crear(TipoCuenta tipoCuenta){ // Task "sin nada" simplemente retorna void
            using var connection = new SqlConnection(connectionString);
            
            // querySingle permite hacer un query qe estamos SEGUROS que 
            // retornara 1 SOLO resultado
            // en este caso, despues de crear un tipoCuenta se desea extrar el id de tipoCuenta
            
            //*SIN procedimiento almacenado
            //var id = await connection.QuerySingleAsync<int>(
            //    @"insert into TiposCuentas (Nombre, UsuarioId, Orden)
            //    values (@Nombre, @UsuarioId, 0);
            //    select SCOPE_IDENTITY();",
            //    tipoCuenta
            //); 
            //SCOPE_IDENTITY() es quien retorna el id recien creado, originalmente requiere params
            // pero al pasarle tipoCuenta como 2do param al dapper, este infiere las props necesarias y las pasa por nosotros
            // tanto para insertar como para para el scope_identity

            //* CON procedimiento almacenado. 
            //* nota: NO se puede pasar objeto tipoCuenta al proc almac, se debe pasar SOLO los params exactos
            var id = await connection.QuerySingleAsync<int>(
                    "TiposCuentas_Insertar",
                    new { usuarioId =tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre },
                    commandType: System.Data.CommandType.StoredProcedure
                );
            tipoCuenta.Id = id;
        }

        //! validate si ya existe nombre a crear
        public async Task<bool> Existe(string nombre, int usuarioId){
            using var connection = new SqlConnection(connectionString);

            // QueryFirstOrDefaultAsync: retorna el 1er registro ó el valor x defecto en caso de qe no exista registro
            // en este caso, el valor por defecto de int es 0 (cero)
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"select 1
                from TiposCuentas
                where Nombre = @Nombre and UsuarioId = @UsuarioId",
                new {nombre, usuarioId}
            );

            //* en este punto "existe" puede tener valor 0 o 1
            
            // si existe un registro con el nombre dado x params
            // retornara true, sino retornara false
            return existe == 1;
        }

        //! LISTAR tipocuentas x usuario
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId){

            using var connection = new SqlConnection(connectionString);
            
            return await connection.QueryAsync<TipoCuenta>(
                @"Select Id, Nombre, Orden 
                from TiposCuentas
                where UsuarioId = @UsuarioId
                order by Orden
                ",
                new {usuarioId}
            );
        }

        //! ACTUALIZAR (UPDATE)
        public async Task Actualizar(TipoCuenta tipoCuenta){
            using var connection = new SqlConnection(connectionString);
            
            // execute: ejecuta un query qe NO retorna nada, en este caso un update
            await connection.ExecuteAsync(
                @"update TiposCuentas 
                  set nombre = @Nombre
                  where id = @id
                ",
                tipoCuenta
            );
        
        }

        // obtiene x id tipo cuenta y ademas id usuario, asi se asegura que 
        // el usuario qe va a manipular la tabla sea el dueño legitimo
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId){
            using var connection = new SqlConnection(connectionString);

            // el resultao es mapeado a un TipoCuenta
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
                @"select *
                from TiposCuentas
                where id = @id and UsuarioId = @UsuarioId
                ",
                new{id, usuarioId} // se pasa nuevo objeto anonimo
            );
        }

        //! DELETE
        public async Task Borrar(int id){
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"delete tiposcuentas
                where id = @id
                ",
                new{id}
            );

        }

        //! para re-ordenar la tabla de tipo cuentas segun arrastre del usuario
        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados){
            
            using var connection = new SqlConnection(connectionString);
            
            var query = "update tiposcuentas set orden = @orden where id = @id";

            //? dapper ejecutará la query por cada elemento del IEnumerable, automaticamente
            await connection.ExecuteAsync(query, tiposCuentasOrdenados);
        }


    }
}