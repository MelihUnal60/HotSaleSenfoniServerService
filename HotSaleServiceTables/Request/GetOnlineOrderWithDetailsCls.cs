using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class GetOnlineOrderWithDetailsCls
    {
        public Token token { get; set; }
        public int orderMId { get; set; }
    }
}
