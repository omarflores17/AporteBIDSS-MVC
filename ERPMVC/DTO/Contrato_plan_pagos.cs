using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class Contrato_plan_pagosDTO  : Contrato_plan_pagos
    {
        public int editar { get; set; } = 1;
    }


}
