﻿using System;
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
	public class SIDetail: IContract
	{
		// Constructor 
		public SIDetail() { }

		// Public Members 

		[DisplayName("DocumentNo")] 
		public string  DocumentNo { get; set; }

		[DisplayName("ProductCode")] 
		public string ProductCode { get; set; }

		[DisplayName("Quantity")]
        public float Quantity { get; set; }

        [DisplayName("UOM")]
        public string UOM { get; set; }

		[DisplayName("UnitPrice")] 
		public decimal UnitPrice { get; set; }

		[DisplayName("CreatedBy")] 
		public string  CreatedBy { get; set; }

		[DisplayName("CreatedOn")] 
		public DateTime  CreatedOn { get; set; }

		[DisplayName("ModifiedBy")] 
		public string  ModifiedBy { get; set; }

		[DisplayName("ModifiedOn")] 
		public DateTime  ModifiedOn { get; set; }

        [DisplayName("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [DisplayName("CurrencyDescription")]
        public string CurrencyDescription { get; set; }

        [DisplayName("ProductDescription")]
        public string ProductDescription { get; set; }

        public IEnumerable<SelectListItem> CurrencyCodeList { get; set; }
	}
}



