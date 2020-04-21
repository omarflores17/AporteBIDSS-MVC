using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class FundingInterestRate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "Meses")]
        public int Months { get; set; }
        [Display(Name = "Tarifa")]
        public double Rate { get; set; }
        public int GrupoConfiguracionInteresesId { get; set; }
        public string GroupKey { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

    }
}
