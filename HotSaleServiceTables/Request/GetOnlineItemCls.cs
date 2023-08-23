using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class GetOnlineItemCls
    {
        public Token token { get; set; }
        public string whouseCode { get; set; }
        public string[] itemCodes { get; set; }
    }
}
