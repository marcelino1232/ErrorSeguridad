using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transacciones
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [Display(Name ="Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        [Required]
        public decimal Monto { get; set; }
        [Range(1,maximum:int.MaxValue,ErrorMessage ="Debe Seleccionar una Categoria")]
        public int CategoriaId { get; set; }
        [StringLength(maximumLength:1000)]
        public string Nota { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una Cuenta")]
        public int CuentaId { get; set; }

        [Display(Name = "Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }
    }
}
