using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using NetStock.Contract;

namespace NetStock.Contract
{
    public class Product : IContract
    {
        // Constructor 
        public Product() { }

        // Public Members 
        [DisplayName("Material Code")]
        public string ProductCode { get; set; }

        [DisplayName("Material Name")]
        public string Description { get; set; }

        [DisplayName("Supplier 1")]
        public string Supplier1 { get; set; }

        [DisplayName("Material Type")]
        public string ProductCategory { get; set; }

        [DisplayName("Supplier 2")]
        public string Supplier2 { get; set; }

        [DisplayName("HSN Code")]
        public string HSNCode { get; set; }

        [DisplayName("UOM")]
        public string UOM { get; set; }

        [DisplayName("Supplier 3")]
        public string Supplier3 { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Count must be a natural number")]
        [DisplayName("AVG Cost")]
        public decimal BuyingPrice { get; set; }

        [DisplayName("IGST")]
        public decimal IGST { get; set; }

        [DisplayName("SGST")]
        public decimal SGST { get; set; }

        [DisplayName("CGST")]
        public decimal CGST { get; set; }

        [DisplayName("CESS")]
        public decimal CESS { get; set; }

        [DisplayName("Bar Code")]
        public string BarCode { get; set; }

        [DisplayName("Size")]
        public string Size { get; set; }

        [DisplayName("Color")]
        public string Color { get; set; }

        [DisplayName("Current Qty")]
        public float CurrentQty { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }

        [DisplayName("Modified On")]
        public DateTime ModifiedOn { get; set; }

        [DisplayName("Re-Order Qty")]
        public float ReOrderQty { get; set; }

        [DisplayName("W/H Location")]
        public string Location { get; set; }

        [DisplayName("W/H Location Description")]
        public string LocationDescription { get; set; }

        [DisplayName("ProductCategory Description")]
        public string ProductCategoryDescription { get; set; }

        public string UOMDescription { get; set; }

        [DisplayName("Selling Price")]
        public decimal SellingPrice { get; set; }



        public IEnumerable<SelectListItem> ProductCategoryList { get; set; }
        public IEnumerable<SelectListItem> UOMList { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }

        [DisplayName("Product Photo")]
        public ProductImage Photo { get; set; }

    }
}



