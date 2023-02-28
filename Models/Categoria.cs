using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        public string Nombre { get; set; }
        [Display(Name = "Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; }    
        public int UsuarioId { get; set; }
    }
}
