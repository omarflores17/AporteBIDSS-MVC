using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ProductId { get; set; }
        
        [Display(Name = "Producto")]
        public string ProductName { get; set; }
        [Display(Name = "Código de producto")]
        public string ProductCode { get; set; }
        [Display(Name = "Código de barra")]
        public string Barcode { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name = "Url")]
        public string ProductImageUrl { get; set; }
        [Display(Name = "Estado")]
        public Int64? IdEstado { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
      
       
        [Display(Name = "Unidad de medida")]
        public int? UnitOfMeasureId { get; set; }
        public double DefaultBuyingPrice { get; set; } = 0.0;
        public double DefaultSellingPrice { get; set; } = 0.0;
        [Display(Name = "Sucursal")]
        public int? BranchId { get; set; }
        [Display(Name = "Moneda")]
        public int? CurrencyId { get; set; }

        public int? LineaId { get; set; }

        public int? GrupoId { get; set; }

        public int? MarcaId { get; set; }
        [ForeignKey("LineaId")]
        public virtual Linea Linea { get; set; }
        [ForeignKey("GrupoId")]
        public virtual Grupo Grupo { get; set; }
        [ForeignKey("MarcaId")]
        public virtual Marca Marca { get; set; }

        public int? FundingInterestRateId { get; set; }
        [ForeignKey("FundingInterestRateId")]
        public FundingInterestRate FundingInterestRate { get; set; }

        public decimal? Prima { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Valor_prima { get; set; }
        public long? ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]
        public ElementoConfiguracion ProductType { get; set; }

        // Campos de TArea 1031 

        [StringLength(50)]
        public string Serie { get; set; }
        [StringLength(50)]
        public string Modelo { get; set; }

        public bool FlagConsignacion { get; set; }

        [StringLength(100)]
        public string SerieMotor { get; set; }
        [StringLength(100)]
        public string SerieChasis { get; set; }

        [Display(Name = "Impuesto a Aplicar para Contratos")]
        public Int64? TaxId { get; set; }
        [ForeignKey("TaxId")]
        public Tax Tax { get; set; }
        [Display(Name = "Porcentaje de Descuento")]
        public decimal? PorcentajeDescuento { get; set; }

        public bool Regalia { get; set; }

        [Required]
        public string UsuarioCreacion { get; set; }
        [Required]
        public string UsuarioModificacion { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        public virtual UnitOfMeasure UnitOfMeasure { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Branch Branch { get; set; }
        public string Correlative { get; set; }
      
    }
}
