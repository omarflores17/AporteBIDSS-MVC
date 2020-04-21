﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Contrato
    {
        [Key]
        [Display(Name = "Id de Contrato")]
        public Int64 ContratoId { get; set; }


        [Display(Name = "Sucursal ")]
        public int? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }


        [Display(Name = "Id del Cliente ")]
        public Int64 CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Display(Name = "Fecha ")]
        public DateTime Fecha { get; set; }

        //internal long GetContratoByCliente(int v)
        //{
        //    throw new NotImplementedException();
        //}

        [Display(Name = "Fecha de Inicio: ")]
        public DateTime Fecha_inicio { get; set; }
        [Display(Name = "Valor de Prima ")]
        public double Valor_prima { get; set; }
        [Display(Name = "Valor de Contrato: ")]
        public double Valor_Contrato { get; set; }

        [Display(Name = "Saldo de Contrato ")]
        public double Saldo_Contrato { get; set; }
        [Display(Name = "Dias de Mora")]
        public Int32 Dias_mora { get; set; }
        [Display(Name = "Estado de Contrato")]
        public Int32 Estado { get; set; }
        [Display(Name = "Nombre Estado")]
        public string NombreEstado { get; set; }
        [Display(Name = "Valor de Cuota")]
        public double Valor_cuota { get; set; }
        [Display(Name = "Cuotas Pagadas")]
        public Int32 Cuotas_pagadas { get; set; }
        [Display(Name = "Cuotas Pendientes")]
        public Int32 Cuotas_pendiente { get; set; }
        [Display(Name = "Proxima Fecha de Pago")]
        public DateTime Proxima_fecha_de_pago { get; set; }

        [Display(Name = "Ultima Fecha de Pago")]
        public DateTime Ultima_fecha_de_pago { get; set; }
        [Display(Name = "Fecha de Vencimiento ")]
        public DateTime Fecha_de_vencimiento { get; set; }
        [Display(Name = "Plazo ")]
        public Int32 Plazo { get; set; }
        [Display(Name = "Tasa de Interes")]
        public double Tasa_de_Interes { get; set; }

        [Display(Name = "Total a Finaciar")]
        public double Finaciar { get; set; }
        [Display(Name = "Total Sin Finaciar")]
        public double SinFinaciar { get; set; }

        [Display(Name = "Total Impuesto")]
        public double Impuesto { get; set; }

        [Display(Name = "Total Prima Monto")]
        public double PrimaMonto { get; set; }

        [Display(Name = "Total Valor Prima")]
        public double ValorPrima { get; set; }

        [Display(Name = "Total Valor Contado")]
        public double ValorContado { get; set; }

        [Display(Name = "Total Saldo Credito")]
        public double SaldoCredito { get; set; }

        [Display(Name = "Total Valor Cuota")]
        public double ValorCuota { get; set; }

        [Display(Name = "Total Contrato")]
        public double TotalContrato { get; set; }

        [Display(Name = "Total Intereses Finaciar")]
        public double InteresesFinaciar { get; set; }

        [Display(Name = "Id del Empleado")]
        public Int64 IdEmpleado { get; set; }

        [Display(Name = "Nombre del Empleado")]
        public string NombreEmpleado { get; set; }

        [Display(Name = "Saldo Actual")]
        public double SaldoActual { get; set; }

        // Campos de reglas moratorias
        [Display(Name = "Valor Pagado")]
        public Double Valorpagado { get; set; }
        [Display(Name = "Intereses Mora")]
        public double InteresesMora { get; set; }
        [Display(Name = "Pago Por Mora")]
        public double PagoPorMora { get; set; }
        // Campos de Auditoria.

        [Required]
        [Display(Name = "Usuario de creacion")]
        public string UsuarioCreacion { get; set; }
        [Required]
        [Display(Name = "Usuario de modificacion")]
        public string UsuarioModificacion { get; set; }
        [Required]
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Required]
        [Display(Name = "Fecha de Modificación")]
        public DateTime FechaModificacion { get; set; }

        // Navigation
        public IEnumerable<Contrato_Detalle> Contrato_detalle { get; set; }
        //public virtual ICollection<Contrato_movimientos> Contrato_movimientos { get; set; }
        public virtual ICollection<Contrato_plan_pagos> Contrato_plan_pagos { get; set; }
    }
}
