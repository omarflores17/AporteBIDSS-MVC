using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
     public class Branch
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int BranchId { get; set; }

        [Display(Name = "Nombre Sucursal")]
        public string BranchName { get; set; }

        [Display(Name = "Descripcion")]
        public string Description { get; set; }

        [Display(Name = "Tipo de Sucursal")]
        public string BranchType { get; set; }

        [Display(Name = "Direccion")]
        public string Address { get; set; }

        [Display(Name = "Ciudad")]
        public long CityId { get; set; }

        [ForeignKey("CityId")]
        public City Ciudad { get; set; }

        [Display(Name = "País")]
        public long CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [Display(Name = "Limite de CNBS")]
        public decimal? LimitCNBS { get; set; }

        [Display(Name = "Estado")]
        public long StateId { get; set; }

        [ForeignKey("StateId")]
        public State Departamento { get; set; }

        [Display(Name = "Telefono ")]
        public string Phone { get; set; }

        [EmailAddress]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Correo invalido")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Encargado ")]
        public long EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employees Employee { get; set; }

        [Required]
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }

        public Int64 IdEstado { get; set; }

        [ForeignKey("IdEstado")]
        public Estados Estados { get; set; }

        [Required]
        [Display(Name = "Usuario de modificacion")]
        public string UsuarioModificacion { get; set; }

        [Required]
        [Display(Name = "Fecha de creacion")]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        List<PuntoEmision> PuntoEmision = new List<PuntoEmision>();
    }
}
