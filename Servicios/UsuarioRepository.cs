using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IUsuarioRepositry
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }

    public class UsuarioRepository : IUsuarioRepositry
    {
        private readonly string connectionString;
        public UsuarioRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Conexion");
        }


        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"insert into Usuario
                  (Email,EmailNormalizado,PasswordHash)
                  values 
                  (@Email,@EmailNormalizado,@PasswordHash);
                  select SCOPE_IDENTITY();"
                , usuario);
            return id;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"select * from Usuario where 
                  EmailNormalizado=@emailNormalizado"
                , new {emailNormalizado});
        }
    }
}
