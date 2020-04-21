using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Contrato_Detalle
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de Contrato Detalle")]
        public Int64 Contrato_detalleId { get; set; }


        [Display(Name = "Contrato Id ")]
        public Int64 ContratoId { get; set; }

        [ForeignKey("ContratoId")]
        public Contrato Contrato { get; set; }

        [Display(Name = "Producto")]
        public Int64 ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Display(Name = "Cantidad")]
        public double Cantidad { get; set; }

        [Display(Name = "Precio")]
        public double Precio { get; set; }
        [Display(Name = "Monto")]
        public double Monto { get; set; }
        //[Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [Display(Name = "Producto_Serie")]
        public string Serie { get; set; }

        //  [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        
        [Display(Name = "Producto_Modelo")]
        public string Modelo { get; set; }

        [Display(Name = "Tasa de Interes")]
        public double FundingInterestRate { get; set; }

        [Display(Name = "ISV")]
        public double Tax { get; set; }

        [Display(Name = "Porcentaje de Descuentos")]
        public decimal? PorcentajeDescuento { get; set; }//PorcentajeDescuento almacena el valor de prima(en la base de datos)

        [StringLength(100)]
        public string SerieMotor { get; set; }
        [StringLength(100)]
        public string SerieChasis { get; set; }
        [Display(Name = "Valor de Prima ")]
        public Decimal Valor_prima { get; set; }
        /// <summary>
        ///  Auditoria 
        /// </summary>
        [Required]
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }
        [Required]
        [Display(Name = "Usuario de modificacion")]
        public string UsuarioModificacion { get; set; }
        [Required]
        [Display(Name = "Fecha de creación")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Fecha de Modificación")]
        public DateTime ModifiedDate { get; set; }

        public int ver { get; set; }
    }
}
