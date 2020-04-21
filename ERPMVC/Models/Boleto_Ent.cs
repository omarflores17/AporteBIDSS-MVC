﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Boleto_Ent
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Int64 Boleto_EntId { get; set; }
        [Display(Name = "Clave de entrada(Consecutivo)")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 clave_e { get; set; }
        [Display(Name = "Clave de la compañia(cliente o proveedor)")]
        public string clave_C { get; set; }
        [Display(Name = "Clave del operador")]
        public string clave_o { get; set; }
        [Display(Name = "Clave del producto")]
        public string clave_p { get; set; }
        [Display(Name = "Salida")]
        public bool completo { get; set; }
        [Display(Name = "Fecha de captura del peso de entrada")]
        public DateTime fecha_e { get; set; }
        [Display(Name = "Hora de captura del peso de entrada")]
        public string hora_e { get; set; }
        [Display(Name = "Identificador del vehículo")]
        public string placas { get; set; }
        [Display(Name = "Nombre del conductor del vehiculo")]
        public string conductor { get; set; }
        [Display(Name = "Valor del peso de entrada")]
        public Int32 peso_e { get; set; }
        [Display(Name = "Observaciones del proceso de captura")]
        public string observa_e { get; set; }
        [Display(Name = "Nombre del operador de entrada")]
        public string nombre_oe { get; set; }
        [Display(Name = "Turno del operador de entrada")]
        public string turno_oe { get; set; }
        [Display(Name = "Unidades de la pesada de entrada")]
        public string unidad_e { get; set; }
        [Display(Name = "Bascula de entrada")]
        public string bascula_e { get; set; }
        [Display(Name = "Tipo de entrada(1.Automatico , 2.Manual, 3. Predeterminado")]
        public int t_entrada { get; set; }
        [Display(Name = "Clave del usuario")]
        public string clave_u { get; set; }

        public virtual Boleto_Sal Boleto_Sal { get; set; }
    }
}
