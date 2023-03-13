using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias{
        Task Crear(Categoria categoria);
    }

    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;

        // region[green1] //!INYECCION
        public RepositorioCategorias(
            IConfiguration configuration
        )
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }//endregion

        public async Task Crear(Categoria categoria){
            using var connection = new SqlConnection(connectionString);
            
            var query = @"insert into Categorias(Nombre,TipoOperacionId,UsuarioId)
                        values(@Nombre,@TipoOperacionId,@UsuarioId)
                        select scope_identity()";

            var id = await connection.QuerySingleAsync<int>(query, categoria);

            categoria.Id = id;
        }

    }
}