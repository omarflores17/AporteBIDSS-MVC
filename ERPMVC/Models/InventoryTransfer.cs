using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class InventoryTransfer
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public int SourceBranchId { get; set; }
        [ForeignKey("SourceBranchId")]
        public Branch SourceBranch { get; set; }
        public int TargetBranchId { get; set; }
        [ForeignKey("TargetBranchId")]
        public Branch TargetBranch { get; set; }
        public DateTime DateGenerated { get; set; }

        public DateTime DepartureDate { get; set; }

        public DateTime DateReceived { get; set; }

        public long GeneratedbyEmployeeId { get; set; }

        public long ReceivedByEmployeeId { get; set; }

        public long CarriedByEmployeeId { get; set; }

        public int? EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public Estados Estados { get; set; }

        public int ReasonId { get; set; }

        public string Reason { get; set; }

        public string CAI { get; set; }

        public string NumeroSAR { get; set; }

        public string Rango { get; set; }

        public string DocumentId { get; set; }

        public long TipoDocumentoId { get; set; }
        public List<InventoryTransferLine> InventoryTransferLines { get; set; }

        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }


        





    }

   
}
