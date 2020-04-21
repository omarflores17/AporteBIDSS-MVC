using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class TipoGastos
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(Name = "Saldo")]
        public float Saldo { get; set; }
        [Display(Name = "Presupuesto")]
        public float Presupuesto { get; set; }
        [Display(Name = "Total")]
        public float Total { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        [Required]
        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        [Required]
        public DateTime FechaModificacion { get; set; }
    }
}
