using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class GetBeginOfDayCls
    {
        public Token token { get; set; }
        public string loadingCardCode { get; set; }
        public string stokTarihi { get; set; }
        public string siparisSevkTarihi { get; set; }
    }
}
