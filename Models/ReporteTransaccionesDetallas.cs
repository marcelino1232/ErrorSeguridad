namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetallas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> transaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos =>
                transaccionesAgrupadas.Sum(x => x.BalanceDepositos);
        public decimal BalanceRetiros =>
                transaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;
        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transacciones> Transacciones { get; set; }
            public decimal BalanceDepositos =>
                Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso)
                .Sum(x => x.Monto);
            public decimal BalanceRetiros =>
                Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Gasto)
                .Sum(x => x.Monto);

        }
    }
}
