using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface ITransaccionesRepository
    {
        Task Crear(Transacciones transacciones);
    }
    public class TransaccionesRepository : ITransaccionesRepository
    {
        private readonly string ConnectionStrings;
        public TransaccionesRepository(IConfiguration configuration)
        {
            ConnectionStrings = configuration.GetConnectionString("Conexion");
        }

        public async Task Crear(Transacciones transacciones)
        {
            using var connection = new SqlConnection(ConnectionStrings);
            var id = await connection.QuerySingleAsync<int>(@"", transacciones);
        }
    }
}
