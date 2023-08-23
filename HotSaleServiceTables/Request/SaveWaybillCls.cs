using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class SaveWaybillCls
    {
        public Token token { get; set; }
        public WaybillM[] waybillMList { get; set; }
    }
}
