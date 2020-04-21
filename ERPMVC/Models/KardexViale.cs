using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class KardexViale
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Kardex Id")]
        public Int64 Id { get; set; }
        [Display(Name = "Fecha")]
        public DateTime KardexDate { get; set; }

        [Display(Name = "Codigo Sucursal")]
        public Int64 BranchId { get; set; }

        [Display(Name = "Sucursal")]
        public string BranchName { get; set; }

        [Display(Name = "Bodega")]
        public Int64 WareHouseId { get; set; }

        [Display(Name = "Bodega")]
        public string WareHouseName { get; set; }

        [Display(Name = "Producto")]
        public Int64 ProducId { get; set; }

        [Display(Name = "Producto")]
        public string ProductName { get; set; }

        [Display(Name = "Saldo Anterior")]
        public double SaldoAnterior { get; set; }

        [Display(Name = "Entrada")]
        public double QuantityEntry { get; set; }

        [Display(Name = "Salida")]
        public double QuantityOut { get; set; }

        [Display(Name = "Saldo")]
        public double Total { get; set; }

        [Display(Name = "Entrada/Salida")]
        public Int32 TypeOperationId { get; set; }

        [Display(Name = "Entrada/Salida")]
        public string TypeOperationName { get; set; }

        [Display(Name = "Cantidad minima en existencia por producto")]
        public double MinimumExistance { get; set; }
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }

        public int TypeOfDocumentId { get; set; }
        public string TypeOfDocumentName { get; set; }
        public int DocumentId { get; set; }
    }
}
