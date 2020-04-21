using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class ContratoDTO  : Contrato
    {
        //public List<Contrato_Detalle> Contrato_detalle { get; set; }
        public int editar { get; set; } = 1;

        public string token { get; set; }

        public int meses { get; set; }
        public int Difmeses { get; set; }
        public string Dias { get; set; }
        public int ver { get; set; }

        public DateTime Fecha1 { get; set; } = DateTime.Now; 
        public DateTime Fecha2 { get; set; } = DateTime.Now;
        public double TotalPagado { get; set; }

        public Double cambio { get; set; }

        //public Contrato_plan_pagos Contrato_plan_pagos { get; set; }
        //public Contrato_movimientos Contrato_movimientos { get; set; }

    }


}
