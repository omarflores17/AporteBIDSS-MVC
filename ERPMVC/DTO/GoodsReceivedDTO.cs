﻿using ERPMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMVC.DTO
{
    public class GoodsReceivedDTO : GoodsReceived
    {
        public int editar { get; set; } = 1;

        public Kardex Kardex { get; set; } = new Kardex();
    }
}
