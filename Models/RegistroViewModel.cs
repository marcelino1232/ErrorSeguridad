using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class RegistroViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(16)]
        public string Password { get; set; }
    }
}
