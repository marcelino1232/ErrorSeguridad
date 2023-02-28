using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface ICuentaRepository
    {
        Task Crear(Cuenta cuenta);
        Task Delete(int id);
        Task<IEnumerable<Cuenta>> GetAll(int usuarioId);
        Task<Cuenta> GetById(int id, int usuarioId);
        Task Update(Cuenta cuenta);
    }
    public class CuentaRepository : ICuentaRepository
    {
        private readonly string connectionString;
        public CuentaRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(
                @"insert into Cuentas (Nombre,TipoCuentaId,Balance,Descripcion) 
                values (@Nombre,@TipoCuentaId,@Balance,@Descripcion)
                select SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> GetAll(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(
                @"select c.id, c.Nombre, Balance, tc.Nombre as TipoCuenta from Cuentas as c
                inner join TipoCuentas as tc on tc.Id = c.TipoCuentaId
                where tc.UsuarioId = @UsuarioId order by tc.Orden", 
                new { usuarioId });
        }

        public async Task<Cuenta> GetById(int id , int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"select c.Id,c.Nombre,Balance,Descripcion,c.TipoCuentaId
                from Cuentas as c inner join TipoCuentas as tc on 
                tc.Id = c.TipoCuentaId where tc.UsuarioId=@UsuarioId and c.Id = @Id",
                new {id,usuarioId});
        }

        public async Task Update(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update  Cuentas
            set Nombre=@Nombre,TipoCuentaId=@TipoCuentaId,Balance=@Balance,Descripcion=@Descripcion
            where Id = @Id;", cuenta);

        }
        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Delete Cuentas where Id= @Id", new { id });
        }
    
    }
}
