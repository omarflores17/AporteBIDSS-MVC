﻿using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class ProformaInvoiceDTO : ProformaInvoice
    {
        public int editar { get; set; } = 1;
    }


}
