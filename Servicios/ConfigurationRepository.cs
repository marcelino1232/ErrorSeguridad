using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Servicios
{
    public static class ConfigurationRepository
    {
        public static void AddService(this IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddTransient<ITipoCuentaRepository, TipoCuentaRepository>();
            services.AddTransient<ICuentaRepository, CuentaRepository>();
            services.AddTransient<IServicoUsuario, ServicoUsuario>();
            services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            services.AddTransient<ITransaccionesRepository, TransaccionesRepository>();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Program));
            services.AddTransient<IUsuarioRepositry, UsuarioRepository>();
            services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
            services.AddTransient<SignInManager<Usuario>>();
            services.AddIdentityCore<Usuario>();


        }
    }
}
