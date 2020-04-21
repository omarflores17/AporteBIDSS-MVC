using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class RegistroGastos
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Sucursal")]
        public int BranchId { get; set; }
        [Display(Name = "Concepto de gasto")]
        public string Detalle { get; set; }
        public string Identidad { get; set; }
        [Required]
        [Display(Name = "Tipo de gasto")]
        public int TipoGastosId { get; set; }
        public string Concepto { get; set; }
        public string Documento { get; set; }
        public double monto { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }
        [Display(Name = "Usuario de modificacion")]
        public string UsuarioModificacion { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "Fecha de Modificación")]
        public DateTime FechaModificacion { get; set; }
    }
}
