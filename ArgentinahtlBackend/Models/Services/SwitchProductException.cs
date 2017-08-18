using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArgentinahtlMVC.Models.Services
{
    public class ProductSwitchException : Exception
    {
        public string ProductCode { get; private set; }
        public string SupplierCode { get; private set; }

        public ProductSwitchException(string productCode, string supplierCode, string message):base(message)
        {
            ProductCode = productCode;
            SupplierCode = supplierCode;
        }

        
    }
}