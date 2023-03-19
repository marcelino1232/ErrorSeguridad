using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface ITransaccionesRepository
    {
        Task Actualizar(Transacciones transacciones, decimal montoAnterior, int cuentaAnterior);
        Task Crear(Transacciones transacciones);
        Task Delete(int id);
        Task<IEnumerable<Transacciones>> GetAllByCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<IEnumerable<Transacciones>> GetAllByUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<Transacciones> GetById(int id, int usuarioId);
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
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", new
            {
                transacciones.UsuarioId,
                transacciones.FechaTransaccion,
                transacciones.Monto,
                transacciones.CategoriaId,
                transacciones.CuentaId,
                transacciones.Nota
            },
            commandType: System.Data.CommandType.StoredProcedure);

            transacciones.Id = id;
        }

        public async Task<Transacciones> GetById(int id, int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionStrings);
            return await connection.QueryFirstOrDefaultAsync<Transacciones>(
            @"select T.FechaTransacion as FechaTransaccion ,T.*,C.TipoOperacionId from Transacciones as T inner join Categoria as C 
            on T.CategoriaId = C.Id
            where T.Id = @Id and T.UsuarioId = @UsuarioId",
            new {
                id,
                usuarioId
            });
        }

        public async Task Actualizar(Transacciones transacciones, decimal montoAnterior,
            int cuentaAnteriorId)
        {

            using var connection = new SqlConnection(ConnectionStrings);

            await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transacciones.Id,
                transacciones.FechaTransaccion,
                transacciones.Monto,
                montoAnterior,
                transacciones.CategoriaId,
                transacciones.CuentaId,
                cuentaAnteriorId,
                transacciones.Nota
            },
            commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Delete(int id)
        {

            using var connection = new SqlConnection(ConnectionStrings);

            await connection.ExecuteAsync("Transacciones_Borrar", new
            {
               id
            },
            commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transacciones>> GetAllByCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(ConnectionStrings);

            return await connection.QueryAsync<Transacciones>(
                @"select t.Id,t.Monto,t.FechaTransacion as FechaTransaccion,c.Nombre as Categoria,
                cu.Nombre as Cuenta, c.TipoOperacionId
                from Transacciones as t
                inner join Categoria as c
                on c.Id = t.CategoriaId
                inner join Cuentas as cu
                on cu.Id = t.CuentaId
                where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                and FechaTransacion Between @FechaInicio and @FechaFin",
                modelo);
        }

        public async Task<IEnumerable<Transacciones>> GetAllByUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(ConnectionStrings);

            return await connection.QueryAsync<Transacciones>(
                @"select t.Id,t.Monto,t.FechaTransacion as FechaTransaccion,c.Nombre as Categoria,
                cu.Nombre as Cuenta, c.TipoOperacionId
                from Transacciones as t
                inner join Categoria as c
                on c.Id = t.CategoriaId
                inner join Cuentas as cu
                on cu.Id = t.CuentaId
                where t.UsuarioId = @UsuarioId
                and FechaTransacion Between @FechaInicio and @FechaFin
                ORDER BY FechaTransaccion DESC", modelo);
        }
    }
}
