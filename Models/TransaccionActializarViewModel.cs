namespace ManejoPresupuesto.Models
{
    public class TransaccionActializarViewModel :TransaccionCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnterior { get; set; }
        public string UrlRetorno { get;set; }
    }
}
