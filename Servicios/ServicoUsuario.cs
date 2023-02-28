namespace ManejoPresupuesto.Servicios
{
    public interface IServicoUsuario
    {
        int ObtenerUsuarioId();
    }
    public class ServicoUsuario : IServicoUsuario
    {
        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
