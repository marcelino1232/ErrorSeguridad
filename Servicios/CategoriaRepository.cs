using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface ICategoriaRepository
    {
        Task Crear(Categoria categoria);
        Task Delete(int id);
        Task<IEnumerable<Categoria>> GetAll(int usuarioId);
        Task<Categoria> GetById(int id, int usuarioId);
        Task Update(Categoria categoria);
    }
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly string connectionString;
        public CategoriaRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task<IEnumerable<Categoria>> GetAll(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
                @"select * from Categoria where UsuarioId=@UsuarioId",
                new { usuarioId });

        }

        public async Task<Categoria> GetById(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                @"select * from categoria where Id=@Id and UsuarioId=@UsuarioId"
                , new {id,usuarioId});
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Categoria (Nombre,TipoOperacionId,UsuarioId)
                  VALUES (@Nombre,@TipoOperacionId,@UsuarioId)
                  SELECT SCOPE_IDENTITY();
                  ", categoria);

            categoria.Id = id;
        }

        public async Task Update(Categoria categoria)
        {
             using var connection = new SqlConnection(connectionString);
             await connection.ExecuteAsync(@"Update Categoria Set Nombre=@Nombre,TipoOperacionId=@TipoOperacionId where Id=@Id", categoria);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Delete From Categoria where Id=@Id", new {id});
        }
    }
}
