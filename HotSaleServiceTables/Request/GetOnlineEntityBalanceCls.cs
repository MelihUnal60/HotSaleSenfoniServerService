﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class GetOnlineEntityBalanceCls
    {
        public Token token { get; set; }
        public string entityCode { get; set; }
    }
}