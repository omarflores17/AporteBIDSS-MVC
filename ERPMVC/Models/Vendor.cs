using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Vendor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id Proveedor")]
        public Int64 VendorId { get; set; }
        [Display(Name = "Nombre del Proveedor")]
        public string VendorName { get; set; }
        [Display(Name = "Tipo de proveedor")]
        public int VendorTypeId { get; set; }
        [ForeignKey("VendorTypeId")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }
        [Display(Name = "Ciudad")]
        public string CityId { get; set; }
        [ForeignKey("CityId")]
        [Display(Name = "País")]
        public string CountryId { get; set; }
        [ForeignKey("CountryId")]
        [Display(Name = "Estado")]
        public string StateId { get; set; }
        [ForeignKey("StateId")]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [EmailAddress]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Correo invalido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Persona de contacto")]
        public string ContactPerson { get; set; }
        
        [Display(Name = "Fecha de creacion")]
        public DateTime FechaCreacion { get; set; }

        
        [Display(Name = "Fecha de Modificacion")]
        public DateTime FechaModificacion { get; set; }
       
        [Display(Name = "Usuario que lo crea")]
        public string UsuarioCreacion { get; set; }
        
        [Display(Name = "Usuario que lo modifica")]
        public string UsuarioModificacion { get; set; }
       
        [Display(Name = "Empresa Referencia")]
        public string CompanyReferenceone { get; set; }
        
        [Display(Name = "Empresa Referencia")]
        public string CompanyReferencetwo { get; set; }
        [Display(Name = "Moneda")]
        public int CurrencyId { get; set; }
        public string Estado { get; set; }
        [Display(Name = "Activo/Inactivo ")]
        public Int64 IdEstado { get; set; }
      
        [Display(Name = "Identidad del contacto")]
        public string Identidad { get; set; }
        [Display(Name = "Teléfono Referencia")]
        public string PhoneReferenceone { get; set; }
        [Display(Name = "Teléfono Referencia")]
        public string PhoneReferencetwo { get; set; }
        
        [Display(Name = "Monto Mínimo")]
        public double QtyMin { get; set; }
   
        [Display(Name = "Monto Mensual")]
        public double QtyMonth { get; set; }
       
        [Display(Name = "RTN del contacto")]
        public string RTN { get; set; }


    }
}
