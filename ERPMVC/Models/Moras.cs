using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Moras
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public Int64 Id { get; set; }

        [MaxLength(5000)]
        [Display(Name = "Nombre del cliente")]
        public string Nombre { get; set; }

        public int Estado { get; set; }
        [Display(Name = "Mínimo Dias")]
        public int MinDias { get; set; }
        [Display(Name = "Máximo Dias")]
        public int MaxDias { get; set; }
        public int Intereses { get; set; }
        public bool Requerido { get; set; }
        //Campos de auditoria
        [Required]
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }
        [Required]
        [Display(Name = "Usuario de modificacion")]
        public string UsuarioModificacion { get; set; }
        [Required]
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Required]
        [Display(Name = "Fecha de Modificación")]
        public DateTime FechaModificacion { get; set; }

        #region Associations

       // public List<Moras> _Moras { get; set; } = new List<Moras>();
        #endregion
    }
}
