using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas{
        Task Crear(Cuenta cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
        Task Actualizar(CuentaCreacionViewModel cuentaVM);
        Task Borrar(int id);
    }
    
    public class RepositorioCuentas: IRepositorioCuentas
    {

        private readonly string connectionString;

        //! INYECCION region[blue]
        public RepositorioCuentas(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //endregion

        //? CRUD: CREATE------------------------------------------------------------------------
        public async Task Crear(Cuenta cuenta){
            using var connection = new SqlConnection(connectionString);
            
            var query = @"insert into Cuentas(Nombre, TipoCuentaId, Descripcion, Balance)
                          values(@Nombre, @TipoCuentaId, @Descripcion, @Balance)
                          select scope_identity()";

            var id = await connection.QuerySingleAsync<int>(query, cuenta);

            cuenta.Id = id;
          
        }//? FIN CRUD: CREATE------------------------------------------------------------------------
        
        //? CRUD: READ------------------------------------------------------------------------
        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId){
            using var connection = new SqlConnection(connectionString);

            var query = @"select c.id,c.Nombre, c.Balance , tc.Nombre as TipoCuenta
                          from Cuentas c
                          inner join TiposCuentas tc
                          on tc.Id  = c.TipoCuentaId 
                          where tc.UsuarioId  = @UsuarioId
                          ORDER by tc.Orden";

            var cuentas = await connection.QueryAsync<Cuenta>(query, new{usuarioId});

            return cuentas;
        }//? FIN CRUD: READ------------------------------------------------------------------------

        //? CRUD: UPDATE------------------------------------------------------------------------
        public async Task Actualizar(CuentaCreacionViewModel cuentaVM){
            
            var connection = new SqlConnection(connectionString);

            var query = @"update Cuentas
                        set Nombre = @Nombre,
                        Balance = @Balance,
                        Descripcion = @Descripcion,
                        TipoCuentaId = @TipoCuentaId
                        where Id = @Id";

            await connection.ExecuteAsync(query, cuentaVM);
        }
        //? FIN CRUD: UPDATE------------------------------------------------------------------------

        //? CRUD: DELETE------------------------------------------------------------------------
        public async Task Borrar(int id){
            using var connection = new SqlConnection(connectionString);

            var query = @"DELETE cuentas
                        where Id = @Id";

            await connection.ExecuteAsync(query,new {id});

        }
        //? FIN CRUD: DELETE------------------------------------------------------------------------

        //? INIT: utilidad para obtener cuenta x id--------------------------------------------------
        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId){
            
            using var connection = new SqlConnection(connectionString);
            
            var query = @"select c.id,c.Nombre, c.Balance, c.Descripcion, c.TipoCuentaId
                          from Cuentas c
                          inner join TiposCuentas tc
                          on tc.Id  = c.TipoCuentaId 
                          where tc.UsuarioId=@UsuarioId and c.Id=@id
                          ORDER by tc.Orden";
            
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                query, new{id,usuarioId}
            );
            
        }//? FIN: utilidad para obtener cuenta x id----------------------------------------------------

    }
}