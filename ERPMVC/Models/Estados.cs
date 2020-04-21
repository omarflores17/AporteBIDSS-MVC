﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.Models
{
    public class Estados
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 IdEstado { get; set; }
        [Required]
        public string NombreEstado { get; set; }
        public string DescripcionEstado { get; set; }
        public long IdGrupoEstado { get; set; }
        [ForeignKey("IdGrupoEstado")]
        public virtual GrupoConfiguracion GrupoConfiguracion { get; set; }

        [Required]
        public string UsuarioCreacion { get; set; }
        [Required]
        public string UsuarioModificacion { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

    }
}
