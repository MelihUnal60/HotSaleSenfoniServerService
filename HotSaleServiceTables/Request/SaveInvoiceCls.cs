using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotSaleServiceTables.Request
{
    public class SaveInvoiceCls
    {
        public Token token { get; set; }
        public InvoiceM[] invoiceMList { get; set; }
    }
}
