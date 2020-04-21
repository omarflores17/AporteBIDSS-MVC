using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class Contrato_movimientosDTO  : Contrato_movimientos
    {
        public List<Contrato> _Contrato { get; set; }
        public int editar { get; set; } = 1;
    }


}
