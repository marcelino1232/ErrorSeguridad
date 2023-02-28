namespace ManejoPresupuesto.Servicios
{
    public static class ConfigurationRepository
    {
        public static void AddService(this IServiceCollection services)
        {
            services.AddTransient<ITipoCuentaRepository, TipoCuentaRepository>();
            services.AddTransient<ICuentaRepository, CuentaRepository>();
            services.AddTransient<IServicoUsuario, ServicoUsuario>();
            services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            services.AddAutoMapper(typeof(Program));
        }
    }
}
