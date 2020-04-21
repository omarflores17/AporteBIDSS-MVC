﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;




namespace ERPMVC.Models
{
    public class Puesto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdPuesto { get; set; }
        [Display(Name = "Nombre de puesto")]
        public string NombrePuesto { get; set; }
        [Display(Name = "Departamento")]
        public long? IdDepartamento { get; set; }
        [Display(Name = "Departamento")]
        public string NombreDepartamento { get; set; }
        [Display(Name = "Usuario de creación")]
        public string Usuariocreacion { get; set; }
        [Display(Name = "Usuario de modificación")]
        public string Usuariomodificacion { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime? FechaCreacion { get; set; }
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacion { get; set; }
    }
}