using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPMVC.Models;

namespace ERPMVC.DTO
{
    public class PurchaseOrderDTO:PurchaseOrder
    {
        public List<PurchaseOrder> _PurchaseOrder { get; set; }
        public int editar { get; set; } = 1;

        public string proceso { get; set; }



        public string token { get; set; }
    }
}
