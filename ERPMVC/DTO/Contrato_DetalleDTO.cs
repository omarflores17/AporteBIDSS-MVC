using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class Contrato_DetalleDTO : Contrato_Detalle
    {
        public List<Contrato_Detalle> _Contrato_Detalle { get; set; }
        public int editar { get; set; } = 1;

        public string token { get; set; }
    }
}
