using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class SaveCashPaymentCls
    {
        public Token token { get; set; }
        public Payment[] paymentList { get; set; }
    }
}
