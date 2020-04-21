using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPMVC.Models;

namespace ERPMVC.DTO
{
    public class PurchaseOrderLineDTO:PurchaseOrderLine
    {
        public List<PurchaseOrderLine> _PurchaseOrderLine { get; set; }
        public int contadorId { get; set; }
        public int editar { get; set; } = 1;

        public string proceso { get; set; }
        public int EstadoId { get; set; }
        public string token { get; set; }
    }
}
