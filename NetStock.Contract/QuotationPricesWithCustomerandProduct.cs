using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock.Contract
{
public class QuotationPricesWithCustomerandProduct:IContract
    {
        public QuotationPricesWithCustomerandProduct()
        {

        }
        public decimal SellRate { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
    }
}
