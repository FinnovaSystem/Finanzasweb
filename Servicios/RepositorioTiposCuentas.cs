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
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta){ // Task "sin nada" simplemente retorna void
            using var connection = new SqlConnection(connectionString);
            
            // querySingle permite hacer un query qe estamos SEGUROS que 
            // retornara 1 SOLO resultado
            // en este caso, despues de crear un tipoCuenta se desea extrar el id de tipoCuenta
            var id = await connection.QuerySingleAsync<int>(
                @"insert into TiposCuentas (Nombre, UsuarioId, Orden)
                values (@Nombre, @UsuarioId, 0);
                select SCOPE_IDENTITY();",
                tipoCuenta
            ); 
            //SCOPE_IDENTITY() es quien retorna el id recien creado, originalmente requiere params
            // pero al pasarle tipoCuenta como 2do param al dapper, este infiere las props necesarias y las pasa por nosotros
            // tanto para insertar como para para el scope_identity
            tipoCuenta.Id = id;
        }


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

        // para GET listado tipocuentas x usuario
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId){

            using var connection = new SqlConnection(connectionString);
            
            return await connection.QueryAsync<TipoCuenta>(
                @"Select Id, Nombre, Orden 
                from TiposCuentas
                where UsuarioId = @UsuarioId
                ",
                new {usuarioId}
            );
        }
    }
}