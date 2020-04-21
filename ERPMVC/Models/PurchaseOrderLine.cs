using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class PurchaseOrderLine
    {
        public int Id { get; set; }

        public int? LineNumber { get; set; }

        public int PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder PurchaseOrder { get; set; }
        
        public int? ProductId { get; set; }

        public string ProductDescription { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public double? QtyOrdered { get; set; }
        
        public double? QtyAuthorized { get; set; }

        public double? QtyReceived { get; set; }

        public double? QtyReceivedToDate { get; set; }

        public double Price { get; set; }

        public string TaxName { get; set; }

        public decimal TaxRate { get; set; }

        public int? TaxId { get; set; }
        public double TaxPercentage { get; set; }
        public double Amount { get; set; }
        public double TaxAmount { get; set; }
        public int DiscountAmount { get; set; }
        public int DiscountPercentage { get; set; }
        public double SubTotal { get; set; }
        public double Total { get; set; }

        [ForeignKey("TaxId")]
        public Tax Tax { get; set; }
        public int UnitOfMeasureId { get; set; }

        public string UnitOfMeasureName { get; set; }

        [ForeignKey("UnitOfMeasureId")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

    }
}
