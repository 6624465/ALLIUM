using NetStock.Contract;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Configuration;


namespace NetStock
{

    public enum DefaultAccountCodes
    {
        BANKCHARGESACCOUNTCODE = 1,
        ARVATACCOUNTCODE = 2,
        ARWHACCOUNTCODE = 3,
        APVATACCOUNTCODE = 4,
        APWHACCOUNTCODE = 5,
        ARADVANCEVATACCOUNTCODE = 6

    };
    public static class Utility
    {
        public static string NEWRECORD = "NEW";
        public static string DEFAULTUSER = "SYSTEM";
        public static string DEFAULTCURRENCYCODE = "THB";
        //public static string DEFAULTUSER = "SYSTEM";
        //public static string DEFAULTUSERNAME = "SYSTEM";
        public static bool DEFAULTSTATUS = true;

        public static string BANKCHARGESACCOUNTCODE = "5310-04";

        //public static short DEFAULTBRANCH = 10;

        public static string CUSTOMERQUOTATION = "CUSTOMER";
        public static string SUPPLIERQUOTATION = "SUPPLIER";

        public static string DEFAULTCUSTOMERMODE = "NONE";

        //public static string REPORTSUBFOLDER = "/ragsarma-001";
        public static string REPORTSUBFOLDER = WebConfigurationManager.AppSettings["ReportSubFolder"].ToString();



        //public static short SsnBranch = -1;
        //public static string SsnBranchText = "";
        //public static string REPORTSUBFOLDER = "";

        /*
        public static short SsnBranch
        {
            get
            {
                return Convert.ToInt16(HttpContext.Current.Session["BranchId"]);
            }
        }

        public static short BRANCHID = -1;

        */


        public static DateTime ThaiTime(this DateTime value)
        {
            string nzTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            return TimeZoneInfo.ConvertTimeFromUtc(value, nzTimeZone);
        }



        public static IEnumerable<SelectListItem> GetCurrencyItemList()
        {
            var lstcurrency = new NetStock.BusinessFactory.CurrencyBO().GetList();

            if (lstcurrency == null)
                return null;

            var selectList = lstcurrency.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.CurrencyCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");


        }


        public static IEnumerable<SelectListItem> GetCountryList()
        {
            var countryList = new NetStock.BusinessFactory.CountryBO().GetList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.CountryCode,
                                    Text = c.CountryName
                                });

            return new SelectList(selectList, "Value", "Text");

        }


        public static IEnumerable<SelectListItem> GetProductList()
        {
            var productList = new NetStock.BusinessFactory.ProductBO().GetList().Where(p => p.Status == true).ToList();

            var selectList = productList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ProductCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");

        }


        public static IEnumerable<SelectListItem> GetSupplierProductList(string supplierCode)
        {
            var productList = new NetStock.BusinessFactory.CustomerProductBO().GetCustomerProductsList(supplierCode);

            var selectList = productList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ProductCode,
                                    Text = c.MatchProductName
                                });

            return new SelectList(selectList, "Value", "Text");

        }


        public static IEnumerable<SelectListItem> GetRoleList()
        {


            List<string> lstRole = new List<string>();

            lstRole.Add("ADMINISTRATOR");
            lstRole.Add("SUPER-USER");
            lstRole.Add("USER");


            var selectList = lstRole.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ToString(),
                                    Text = c.ToString()
                                });


            return new SelectList(selectList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetDiscountList()
        {


            List<string> lstDiscount = new List<string>();

            lstDiscount.Add("NONE");
            lstDiscount.Add("AMOUNT");
            lstDiscount.Add("PERCENTAGE");


            var selectList = lstDiscount.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ToString(),
                                    Text = c.ToString()
                                });


            return new SelectList(selectList, "Value", "Text");

        }



        public static IEnumerable<SelectListItem> GetCustomerList(bool isSupplier)
        {


            var customerList = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cu => cu.Status == true && cu.CustomerType == (isSupplier ? "SUPPLIER" : "CUSTOMER")).ToList();



            var selectList = customerList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.CustomerCode,
                                    Text = c.CustomerName
                                });

            //selectList.Where(dt => dt.Value == "เงินสด").Update(dt => dt.Selected = true);

            return new SelectList(selectList, "Value", "Text");

        }
        public static IEnumerable<SelectListItem> GetContactPersonList()
        {


            var customerList = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cu => cu.Status == true && cu.CustomerType == "CUSTOMER").ToList();



            var selectList = customerList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ContactPerson,
                                    Text = c.ContactPerson
                                });
            return new SelectList(selectList, "Value", "Text");
        }
        public static IEnumerable<SelectListItem> GetSalesLeadList()
        {


            var salesLeadList = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cu => cu.Status == true && cu.CustomerType == "CUSTOMER").ToList();



            var selectList = salesLeadList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.SalesLead,
                                    Text = c.SalesLead
                                });
            return new SelectList(selectList, "Value", "Text");

        }
        public static IEnumerable<SelectListItem> GetDeliveryLocationList()
        {


            var DeliveryLocationList = new NetStock.BusinessFactory.AddressBO().GetList().Where(x=> x.AddressType == "CUSTOMER").Select(x=> new SelectListItem { Value=x.Address1,Text=x.Address1}).ToList();

            return new SelectList(DeliveryLocationList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetCityList()
        {


            var CityList = new NetStock.BusinessFactory.AddressBO().GetList().Where(x => x.AddressType == "CUSTOMER").Select(x => new SelectListItem { Value = x.CityName, Text = x.CityName }).ToList();

            return new SelectList(CityList, "Value", "Text");

        }


        public static IEnumerable<SelectListItem> GetBranchList()
        {


            var branchList = new NetStock.BusinessFactory.BranchBO().GetList();



            var selectList = branchList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.BranchID.ToString(),
                                    Text = c.BranchCode
                                });

            return new SelectList(selectList, "Value", "Text");

        }



        public static IEnumerable<SelectListItem> GetLookupItemList(string lookupCategory)
        {
            var lstLookUp = new NetStock.BusinessFactory.LookupBO().GetList().Where(lk => lk.Status == true && lk.Category == lookupCategory).ToList();

            if (lstLookUp == null)
                return null;

            var selectList = lstLookUp.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.LookupCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text", "PAYMENT-CASH");


        }

        public static IEnumerable<SelectListItem> GetProductCategory()
        {
            var lstLookUp = new NetStock.BusinessFactory.ProductCategoryBO().GetList();

            if (lstLookUp == null)
                return null;

            var selectList = lstLookUp.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.CategoryCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");


        }


        public static IEnumerable<SelectListItem> GetLocationList()
        {
            var lstLookUp = new NetStock.BusinessFactory.LocationBO().GetList();

            if (lstLookUp == null)
                return null;

            var selectList = lstLookUp.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.LocationCode,
                                    Text = c.LocationDescription
                                });

            return new SelectList(selectList, "Value", "Text");


        }

        public static void Update<T>(this IEnumerable<T> source, params Action<T>[] updateValues)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (updateValues == null)
            {
                throw new ArgumentNullException("updateValues");
            }

            foreach (T item in source)
            {
                foreach (Action<T> update in updateValues)
                {
                    update(item);
                }
            }
        }

        public static IEnumerable<SelectListItem> GetPOList()
        {


            List<string> lstPO = new List<string>();

            lstPO.Add("NONE");


            var selectList = lstPO.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ToString(),
                                    Text = c.ToString()
                                });


            return new SelectList(selectList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetAccountGroupItemList(short branchID)
        {
            var lstAccountGroup = new NetStock.BusinessFactory.AccountGroupBO().GetList();

            var lstCoa = new NetStock.BusinessFactory.ChartOfAccountBO().GetList(branchID).Select(x => new SelectListItem { Value = x.AccountCode, Text = x.Description }).ToList();




            if (lstAccountGroup == null)
                return null;

            var selectList = lstAccountGroup.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.Code,
                                    Text = c.Description
                                }).ToList();

            selectList.AddRange(lstCoa);

            return new SelectList(selectList, "Value", "Text");


        }


        public static IEnumerable<SelectListItem> GetAccountCodeItemList()
        {
            var lstaccountcode = new NetStock.BusinessFactory.ChartOfAccountBO().GetList(SsnBranch);

            if (lstaccountcode == null)
                return null;

            var selectList = lstaccountcode.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.AccountCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");


        }

        public static short SsnBranch
        {
            get
            {
                return Convert.ToInt16(HttpContext.Current.Session["BranchId"]);
            }
        }


        public static IEnumerable<SelectListItem> GetCreditorAccountList()
        {
            var countryList = new NetStock.BusinessFactory.ChartOfAccountBO().GetCreditorAccountList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.AccountCode,
                                    Text = string.Format("{0} ({1})", c.AccountCode, c.Description)
                                });

            return new SelectList(selectList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetCreditorList()
        {
            var countryList = new NetStock.BusinessFactory.CreditorBO().GetList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.CreditorCode,
                                    Text = string.Format("{0} ({1})", c.CreditorName, c.CreditorCode)
                                });

            return new SelectList(selectList, "Value", "Text");

        }

        public static ChartOfAccount GetAccountType(string accountCode)
        {
            ChartOfAccount chartOfAccount = new ChartOfAccount();
            chartOfAccount.AccountCode = accountCode;
            chartOfAccount.BranchID = Utility.SsnBranch;
            var accountType = new NetStock.BusinessFactory.ChartOfAccountBO().GetChartOfAccount(chartOfAccount);
            return accountType;

        }


        public static string GetDefaultAccountCodes(DefaultAccountCodes code)
        {

            var lstSystemWideConfiguration = new NetStock.BusinessFactory.SystemWideConfigurationBO().GetList();

            var value = "";
            var config = "";

            switch (code)
            {
                case DefaultAccountCodes.BANKCHARGESACCOUNTCODE:
                    config = "Bank Fee Account Code";
                    break;
                case DefaultAccountCodes.ARVATACCOUNTCODE:
                    config = "AR VAT Account Code";
                    break;
                case DefaultAccountCodes.ARWHACCOUNTCODE:
                    config = "AR W/H Account Code";
                    break;
                case DefaultAccountCodes.APVATACCOUNTCODE:
                    config = "AP VAT Account Code";
                    break;
                case DefaultAccountCodes.APWHACCOUNTCODE:
                    config = "AP W/H Account Code";
                    break;

                case DefaultAccountCodes.ARADVANCEVATACCOUNTCODE:
                    config = "AR Adv. VAT Account Code";
                    break;

                default:
                    break;
            }
            value = lstSystemWideConfiguration.Where(x => x.DisplayName == config).FirstOrDefault().ConfigurationValue;

            return value;
        }

        public static IEnumerable<SelectListItem> GetPaymentTypeList(string cbCategory)
        {
            var lstLookUp = new NetStock.BusinessFactory.LookupBO().GetList().Where(lk => lk.Category == cbCategory).ToList();

            if (lstLookUp == null)
                return null;

            var selectList = lstLookUp.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.LookupCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetBankCodeList()
        {
            var countryList = new NetStock.BusinessFactory.BankBO().GetList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.BankCode,
                                    Text = string.Format("{0} - {1}", c.BankCode, c.Name)
                                });

            return new SelectList(selectList, "Value", "Text");

        }
        public static IEnumerable<SelectListItem> GetChargeCodeItemList()
        {
            var lstchargecode = new NetStock.BusinessFactory.ChargeCodeBO().GetList(Utility.SsnBranch);

            if (lstchargecode == null)
                return null;

            var selectList = lstchargecode.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.ChargeCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");


        }

        public static IEnumerable<SelectListItem> GetReceiptTypeList(string cbCategory)
        {
            var lstLookUp = new NetStock.BusinessFactory.LookupBO().GetList().Where(lk => lk.Category == cbCategory).ToList();

            if (lstLookUp == null)
                return null;

            var selectList = lstLookUp.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.LookupCode,
                                    Text = c.Description
                                });

            return new SelectList(selectList, "Value", "Text");

        }

        public static IEnumerable<SelectListItem> GetDebtorList()
        {
            var countryList = new NetStock.BusinessFactory.DebtorBO().GetList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.DebtorCode,
                                    Text = string.Format("{0} ({1})", c.DebtorName, c.DebtorCode)
                                });

            return new SelectList(selectList, "Value", "Text");

        }


        public static IEnumerable<SelectListItem> GetDebtorAccountList()
        {
            var countryList = new NetStock.BusinessFactory.ChartOfAccountBO().GetDebtorAccountList();

            var selectList = countryList.Select(c =>
                                new SelectListItem
                                {
                                    Value = c.AccountCode,
                                    Text = string.Format("{0} ({1})", c.AccountCode, c.Description)
                                });

            return new SelectList(selectList, "Value", "Text");

        }


    }


    public static class ControllerExtensions
    {
        #region Json
        public static int MaxJsonLength { get; set; }

        static ControllerExtensions()
        {
            MaxJsonLength = 2147483644;
        }

        public static System.Web.Mvc.JsonResult LargeJson(this System.Web.Mvc.Controller controlador, object data)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                MaxJsonLength = MaxJsonLength,
            };
        }
        public static System.Web.Mvc.JsonResult LargeJson(this System.Web.Mvc.Controller controlador, object data, System.Web.Mvc.JsonRequestBehavior behavior)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                JsonRequestBehavior = behavior,
                MaxJsonLength = MaxJsonLength
            };
        }
        //TODO: You can add more overloads, the controller class has 6 overloads
        #endregion
    }

    public enum DocumentType
    {
        DOMESTIC = 1,
        OVERSEAS = 2
    }


    public static class ModelStateHelper
    {
        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key,
                    kvp => kvp.Value.Errors
                                    .Select(e => e.ErrorMessage).ToArray())
                                    .Where(m => m.Value.Count() > 0);
            }
            return null;
        }
    }


    
}