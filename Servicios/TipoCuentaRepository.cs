using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuesto.Servicios
{
    public interface ITipoCuentaRepository
    {
        Task Crear(TipoCuenta tipoCuenta);
        Task Delete(int id);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> GetAll(int usuarioId);
        Task<TipoCuenta> GetById(int Id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas);
        Task Update(TipoCuenta tipoCuenta);
    }

    public class TipoCuentaRepository : ITipoCuentaRepository
    {
        private readonly string connectionString;
        public TipoCuentaRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                         new {usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre},
                         commandType:CommandType.StoredProcedure);
                         
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre , int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT 1 FROM TipoCuentas
                WHERE Nombre = @Nombre AND UsuarioId=@UsuarioId", 
                new {nombre,usuarioId});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> GetAll(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(
                @"select Id, Nombre,Orden from TipoCuentas 
                where UsuarioId = @UsuarioId ORDER BY Orden",
                new {usuarioId});
        }

        public async Task Update(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"update TipoCuentas set Nombre = @Nombre where Id = @Id", tipoCuenta);
        }
        public async Task<TipoCuenta> GetById(int Id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var TipoCuenta = await connection.
                QueryFirstOrDefaultAsync<TipoCuenta>(
                @"select * from TipoCuentas
                where Id = @Id and UsuarioId=@usuarioId",
                new {Id,usuarioId}); 

            return TipoCuenta;
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"delete  TipoCuentas where Id = @Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas)
        {
            var query = "update TipoCuentas set Orden = @Orden where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query,tipoCuentas);
        }
    }
}
