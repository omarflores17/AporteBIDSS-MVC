using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ERPMVC.Models
{
    public class MotivosAjuste
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(Name = "Id")]
        public int Utlidad { get; set; }
        [Display(Name = "Estado")]
        public long? EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public Estados Estados { get; set; }
        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
