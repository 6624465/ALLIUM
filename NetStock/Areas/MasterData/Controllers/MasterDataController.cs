using NetStock.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetStock.ActionFilters;


namespace NetStock.Areas.MasterData.Controllers
{
    [Authorize]
    [SessionFilter]
    [RouteArea("MasterData")]
    public class MasterDataController : Controller
    {
        // GET: MasterData/MasterData
        public ActionResult Index()
        {
            return View();
        }


        #region Company




        [Route("Company")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CompanyProfile(string companyCode)
        {
            Company company;
            if (companyCode != null)
                company = new NetStock.BusinessFactory.CompanyBO().GetCompany(new Company { CompanyCode = companyCode });
            else
                company = new Company();

            company.CountryList = Utility.GetCountryList();
            //return View("CompanyProfile");
            return View(company);
        }

        [ChildActionOnly]
        [Route("GetCompanies")]
        public PartialViewResult GetCompanies()
        {
            var companyList = new NetStock.BusinessFactory.CompanyBO().GetList();
            return PartialView(companyList);
        }




        [HttpPost]
        [Route("SaveCompanyProfile")]
        public ActionResult SaveCompanyProfile(NetStock.Contract.Company company)
        {

            company.CreatedBy = Session["DEFAULTUSER"].ToString();
            company.ModifiedBy = Session["DEFAULTUSER"].ToString();
            company.IsActive = Utility.DEFAULTSTATUS;


            if (company.CompanyAddress.AddressId == 0 || company.CompanyAddress.AddressId == null)
            {
                company.CompanyAddress.AddressType = "Company";
                company.CompanyAddress.SeqNo = 1;
                company.CompanyAddress.AddressLinkID = company.CompanyCode;

            }

            var result = new NetStock.BusinessFactory.CompanyBO().SaveCompany(company);

            return RedirectToAction("CompanyProfile", "MasterData", new { companyCode = company.CompanyCode });

        }

        #endregion

        #region Branch

        [Route("DeleteBranch")]
        [HttpGet]
        public ActionResult DeleteBranch(short branchID)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.BranchBO().DeleteBranch(new Branch { BranchID = branchID });

                TempData["isSaved"] = result;

                if (result)
                    TempData["resultMessage"] = string.Format("Branch [{0}] Deleted Successfully", branchID);
                else
                    TempData["resultMessage"] = "Unable to DELETE the Record!";

            }
            catch (Exception ex)
            {
                TempData["isSaved"] = false;
                TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("BranchProfile", "MasterData");
        }


        [Route("Branch")]
        [HttpGet]
        public ActionResult BranchProfile(string companyCode, string branchCode)
        {
            var branchprofile = new NetStock.Contract.Branch();
            branchprofile.CompanyCode = companyCode;

            if (branchCode != "NEW")
            {
                branchprofile = new NetStock.BusinessFactory.BranchBO().GetBranch(new NetStock.Contract.Branch { BranchCode = branchCode });
            }

            if (branchprofile == null)
            {
                branchprofile = new NetStock.Contract.Branch();
            }

            branchprofile.CountryList = Utility.GetCountryList();

            return View(branchprofile);
        }

        [HttpPost]
        [Route("SaveBranchProfile")]
        public ActionResult SaveBranchProfile(NetStock.Contract.Branch branch)
        {

            branch.CreatedBy = Session["DEFAULTUSER"].ToString();
            branch.ModifiedBy = Session["DEFAULTUSER"].ToString();
            branch.IsActive = Utility.DEFAULTSTATUS;

            if (branch.BranchAddress.AddressId == 0 || branch.BranchAddress.AddressId == null)
            {
                branch.BranchAddress.AddressType = "Branch";
                branch.BranchAddress.SeqNo = 1;
                branch.BranchAddress.IsActive = true;
                branch.BranchAddress.AddressLinkID = branch.BranchCode;

            }
            var result = new NetStock.BusinessFactory.BranchBO().SaveBranch(branch);

            return RedirectToAction("BranchProfile", "MasterData", new { companyCode = branch.CompanyCode, branchCode = branch.BranchCode });
        }


        #endregion


        #region Product Master



        [Route("Products")]
        public ActionResult Products()
        {

            var products = new NetStock.BusinessFactory.ProductBO().GetList().Where(pr => pr.Status == true).ToList();

            return View("Products", products);
        }

        [Route("SearchProducts")]
        public ActionResult SearchProducts(string ProductDesc)
        {

            var products = new NetStock.BusinessFactory.ProductBO().GetListByDescription(ProductDesc);

            return View("Products", products);
        }

        [Route("EditProduct")]
        [HttpGet]
        public ActionResult EditProduct(string productCode, string barcode, string uom, string size, string color)
        {
            var product = new NetStock.Contract.Product();

            if (productCode != null && productCode.Length > 0)
            {
                if (productCode == "NEW")
                {
                    product = new NetStock.Contract.Product();
                }
                else
                {
                    product = new NetStock.BusinessFactory.ProductBO().GetProduct(new Contract.Product { ProductCode = productCode, BarCode = barcode, UOM = uom, Size = size, Color = color });

                    product.Photo = new NetStock.BusinessFactory.ProductImageBO().GetProductImage(product.ProductCode);
                }
            }
            product.UOMList = Utility.GetLookupItemList("UOM");
            product.ProductCategoryList = Utility.GetProductCategory();
            product.LocationList = Utility.GetLocationList();


            //return PartialView("AddProduct", product);
            return PartialView("NewProduct", product);
        }

        [Route("EditProduct1")]
        [HttpPost]
        public ActionResult EditProduct1(Product product)
        {

            if (!ModelState.IsValid)
            {
                return View("EditProduct", product);
            }

            try
            {
                foreach (string file in Request.Files)
                {
                    string fileName = file;
                }

                product.CreatedBy = Session["DEFAULTUSER"].ToString();
                product.ModifiedBy = Session["DEFAULTUSER"].ToString();
                product.Status = true;


                var result = new NetStock.BusinessFactory.ProductBO().SaveProduct(product);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }


        [Route("CheckDuplicateProduct")]
        [HttpPost]
        public JsonResult CheckDuplicateProduct(string productDescription, string barCode)
        {

            try
            {

                var product = new NetStock.BusinessFactory.ProductBO().CheckDuplicateProduct(productDescription, barCode);

                if (product != null)
                {
                    //return Json(new { success = true });
                    //return Json(new { result = true, Message = "Product Already exists", productCode = product });
                    return this.Json(new { Message = "Product Already Exists!", productCode = product }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //return Json(new { result = false,productCode = "" });
                    return this.Json(new { Message = "", productCode = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }


        [Route("AddProduct")]
        [HttpGet]
        public ActionResult AddProduct(string productCode, string uom = "")
        {

            var product = new NetStock.Contract.Product();



            if (productCode != null && productCode.Length > 0)
            {
                if (productCode == "NEW")
                {
                    product = new NetStock.Contract.Product();
                    product.Status = true;
                }
                else
                {
                    product = new NetStock.BusinessFactory.ProductBO().GetProduct(new Contract.Product { ProductCode = productCode, UOM = uom });
                    product.Photo = new NetStock.BusinessFactory.ProductImageBO().GetProductImage(product.ProductCode);
                }
            }
            product.UOMList = Utility.GetLookupItemList("UOM");
            //  product.ProductCategoryList = Utility.GetProductCategory();
            var PrductCategoryList = new NetStock.BusinessFactory.LookupBO().GetList().Where(x => x.Category == "PRODUCTCATEGORY" && x.Status == true).Select(y => new
            SelectListItem
            {
                Value = y.LookupCode,
                Text = y.Description
            });
            product.ProductCategoryList = PrductCategoryList;
            product.LocationList = Utility.GetLookupItemList("LOCATION");

            return View("AddProduct", product);
        }
        /*
        [Route("AddProduct")]
        [HttpGet]
        public ActionResult AddProduct(string productCode, string barcode = "", string uom = "", string size = "", string color = "")
        {

            var product = new NetStock.Contract.Product();



            if (productCode != null && productCode.Length > 0)
            {
                if (productCode == "NEW")
                {
                    product = new NetStock.Contract.Product();
                    product.Status = true;
                }
                else
                {
                    product = new NetStock.BusinessFactory.ProductBO().GetProduct(new Contract.Product { ProductCode = productCode, BarCode = barcode, UOM = uom, Size = size, Color = color });

                    product.Photo = new NetStock.BusinessFactory.ProductImageBO().GetProductImage(product.ProductCode);

                    //string imageBase64Data = Convert.ToBase64String(product.Photo.ProductImg);
                    //string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    //ViewBag.ImageData = imageDataURL;

                }
            }
            product.UOMList = Utility.GetLookupItemList("UOM");
            product.ProductCategoryList = Utility.GetProductCategory();
            //product.LocationList = Utility.GetLocationList();
            product.LocationList = Utility.GetLookupItemList("LOCATION");

            //var t = new FileContentResult(product.Photo.ProductImg, "image/jpeg");
            //ViewBag.Test = t;
            //return PartialView("AddProduct", product);
            return View("AddProduct", product);
        }
        */





        public FileContentResult GetImage(string productCode)
        {
            NetStock.Contract.Product product = new NetStock.Contract.Product();

            product.Photo = new NetStock.BusinessFactory.ProductImageBO().GetProductImage(productCode);

            byte[] byteArray = product.Photo.ProductImg;
            return byteArray != null
                           ? new FileContentResult(byteArray, "image/jpeg")
                           : null;


            //return File(product.Photo.ProductImg, "image/jpg");
        }


        [Route("AddProduct")]
        [HttpPost]
        public ActionResult AddProduct(Product product)
        {

            try
            {
                //product.Description = product.Description.Replace("\"", "''");
                //product.BarCode = product.BarCode.Replace("\"", "''");
                //if (product.ProductCode == "NEW" || product.ProductCode == "")
                //{
                //    var objduplicate = new NetStock.BusinessFactory.ProductBO().CheckDuplicateProduct(product.Description, product.BarCode);
                //    if (objduplicate != null)
                //    {
                //        TempData["isSaved"] = false;
                //        TempData["resultMessage"] = string.Format("Either Product Description or Barcode is duplicate", product.ProductCode);
                //        return RedirectToAction("Products", "MasterData");
                //        //return Json(new { success = false, Message = "Either Product Description or Barcode is duplicate", productCode = product.ProductCode });
                //    }
                //}

                byte[] fileData = null;
                var fileName = string.Empty;
                foreach (string file in Request.Files)
                {

                    HttpPostedFileBase currentfile = Request.Files[file];
                    fileName = currentfile.FileName;
                    using (var binaryReader = new BinaryReader(currentfile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(currentfile.ContentLength);
                    }

                }
                ProductImage pImage = new ProductImage();

                pImage.ProductImg = fileData;
                pImage.Code = product.ProductCode;
                pImage.FileName = fileName;

                product.CreatedBy = Session["DEFAULTUSER"].ToString();
                product.ModifiedBy = Session["DEFAULTUSER"].ToString();
                product.Status = true;
                product.Photo = pImage;


                var result = new NetStock.BusinessFactory.ProductBO().SaveProduct(product);

                @TempData["isSaved"] = result;


                if (result)
                {
                    @TempData["resultMessage"] = string.Format("Product [{0}] Saved Successfully", product.ProductCode);
                }
                else {
                    @TempData["resultMessage"] = "Unable to Save the Record!";
                }


                //return Json(new { success = true, Message = "Product Saved successfully.", productCode = product.ProductCode });

                //return Json(new { success = true });
            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("Products", "MasterData");

        }


        [Route("DeleteProduct")]
        [HttpPost]
        public JsonResult DeleteProduct(string productCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.ProductBO().DeleteProduct(new NetStock.Contract.Product { ProductCode = productCode, ModifiedBy = Session["DEFAULTUSER"].ToString() });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

            return Json(new { result = result, Message = "Product deleted successfully.", productCode = productCode });


        }

        #endregion


        #region Test Upload PHOTO

        [Route("TestUpload")]
        public ActionResult TestUpload()
        {
            return View();
        }

        [Route("TestPhotoUpload")]
        [HttpPost]
        public ActionResult TestPhotoUpload()
        {
            foreach (string item in Request.Files)
            {

            }
            return Json(new { Message = "" });
        }


        #endregion

        #region Supplier



        [Route("Suppliers")]
        public ActionResult Suppliers()
        {

            var suppliers = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cs => cs.CustomerType == "SUPPLIER").ToList();

            return View("Suppliers", suppliers);
            //return View(Suppliers);
        }

        [Route("EditSupplier")]
        [HttpGet]
        public ActionResult EditSupplier(string customerCode)
        {


            NetStock.Contract.Customer supplier = new Contract.Customer();

            if (customerCode == "NEW")
            {
                supplier.CustomerCode = customerCode;
                customerCode = "";
                supplier.Status = true;
            }
            if (customerCode != null && customerCode.Length > 0)
            {
                supplier = new NetStock.BusinessFactory.CustomerBO().GetCustomer(new Contract.Customer { CustomerCode = customerCode });
            }

            supplier.CountryList = NetStock.Utility.GetCountryList();
            supplier.CustomerModeList = NetStock.Utility.GetLookupItemList("SUPPLIERTYPE");
            return View("AddSupplier", supplier);
            //return View(Suppliers);
        }

        [Route("EditSupplier")]
        [HttpPost]
        public ActionResult EditSupplier(NetStock.Contract.Customer supplier)
        {


            try
            {


                supplier.CreatedBy = Session["DEFAULTUSER"].ToString();
                supplier.ModifiedBy = Session["DEFAULTUSER"].ToString();
                supplier.CustomerType = "SUPPLIER";
                supplier.Status = true;

                if (supplier.CustomerAddress.AddressId == 0 || supplier.CustomerAddress.AddressId == null)
                {
                    supplier.CustomerAddress.AddressType = "SUPPLIER";// "Customer";
                    supplier.CustomerAddress.SeqNo = 1;
                    supplier.CustomerAddress.AddressLinkID = supplier.CustomerCode;

                }
                var result = new NetStock.BusinessFactory.CustomerBO().SaveCustomer(supplier);



            }
            catch (Exception ex)
            {

                throw ex;
            }



            return RedirectToAction("Suppliers");
            //return View(Suppliers);
        }


        [Route("DeleteSupplier")]
        [HttpGet]
        public ActionResult DeleteSupplier(string supplierCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.CustomerBO().DeleteCustomer(new NetStock.Contract.Customer { CustomerCode = supplierCode, ModifiedBy = Session["DEFAULTUSER"].ToString() });

                TempData["isSaved"] = result;

                if (result)
                    TempData["resultMessage"] = string.Format("Customer [{0}] Deleted Successfully", supplierCode);
                else
                    TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                TempData["isSaved"] = false;
                TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            //return Json(new { result = result, Message = "Supplier deleted successfully.", supplierCode = supplierCode });
            return RedirectToAction("Suppliers");


        }


        /*
        [Route("SaveProd")]
        [HttpPost]
        public JsonResult SaveProd(string data)
        {
            JsonResult jr=new JsonResult();

            var lstResult = data.Split('$').ToList();

            NetStock.Contract.CustomerProduct cp = new Contract.CustomerProduct();
            cp.ProductCode = lstResult[1];
            cp.MatchProductCode = lstResult[0];
            cp.BarCode = lstResult[2];
            cp.CostPrice = Convert.ToDecimal(lstResult[3].ToString());

            // now do we need to split the string and put into the CustomerProduct object ? 
            // 

            var supplier = new NetStock.Contract.Customer();

            //return jr;

            var lstproducts = new List<NetStock.Contract.CustomerProduct>();

            lstproducts.Add(cp);

            supplier.CustomerProducts = lstproducts;

            

            //return View("SupplierProduct", supplier);

            return Json(supplier, JsonRequestBehavior.AllowGet);
        }
        */

        #endregion

        #region Customer



        [Route("Customers")]
        public ActionResult Customers()
        {
            var customers = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cs => cs.CustomerType == "CUSTOMER").ToList();

            return View("Customers", customers);

        }

        [HttpGet]
        [Route("CheckCustomerExist")]
        public ActionResult CheckCustomerExist(string CustomerCode)
        {
            var customerCount = new NetStock.BusinessFactory.CustomerBO().GetList().Where(cs => cs.CustomerType == "CUSTOMER" && cs.CustomerCode== CustomerCode).Count();
            if (customerCount != 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }
        [Route("EditCustomer")]
        [HttpGet]
        public ActionResult EditCustomer(string customerCode)
        {
            NetStock.Contract.Customer customer = new Contract.Customer();

            if (customerCode == "NEW" || customerCode == "" || customerCode == null)
            {
                //customer.CustomerCode = customerCode;
                customer.CustomerCode = "";
                customer.Status = true;
            }

            if (customerCode != null && customerCode.Length > 0)
            {
                if (customerCode != "NEW")
                {
                    customer = new NetStock.BusinessFactory.CustomerBO().GetCustomer(new Contract.Customer { CustomerCode = customerCode });
                }

            }
            customer.UOMList = Utility.GetLookupItemList("UOM");
            customer.CountryList = NetStock.Utility.GetCountryList();
            customer.PaymentModeList = Utility.GetLookupItemList("PAYMENTMODE");
            return View("AddCustomer", customer);
            //return View(Suppliers);
        }

        [Route("GetCustomer")]
        [HttpGet]
        public JsonResult GetCustomer(string customerCode)
        {

            NetStock.Contract.Customer customer = new Contract.Customer();
            customer = new NetStock.BusinessFactory.CustomerBO().GetCustomer(new Contract.Customer { CustomerCode = customerCode });
            customer.CountryList = NetStock.Utility.GetCountryList();
            return Json(customer);

        }


        [Route("EditCustomer")]
        [HttpPost]
        public ActionResult EditCustomer(NetStock.Contract.Customer customer)
        {
            try
            {
                customer.CreatedBy = Session["DEFAULTUSER"].ToString();
                customer.ModifiedBy = Session["DEFAULTUSER"].ToString();
                customer.CustomerMode = Utility.DEFAULTCUSTOMERMODE;
                customer.CustomerType = "CUSTOMER";
                customer.Status = true;

                if (customer.CustomerAddress.AddressId == 0 || customer.CustomerAddress.AddressId == null)
                {
                    customer.CustomerAddress.AddressType = "Customer";
                    customer.CustomerAddress.SeqNo = 1;
                    customer.CustomerAddress.AddressLinkID = customer.CustomerCode;

                }
                var result = new NetStock.BusinessFactory.CustomerBO().SaveCustomer(customer);
                if (result)
                {
                    TempData["isSaved"] = result;
                    TempData["resultMessage"] = string.Format("Customer Details [{0}] Saved Successfully", customer.CustomerCode);
                }
                else
                    TempData["resultMessage"] = "Unable to Save the Record!";

            }
            catch (Exception ex)
            {
                TempData["isSaved"] = false;
                TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }



            return RedirectToAction("Customers");
            //return View(Suppliers);
        }



        [Route("DeleteCustomer")]
        [HttpGet]
        public ActionResult DeleteCustomer(string customerCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.CustomerBO().DeleteCustomer(new NetStock.Contract.Customer { CustomerCode = customerCode, ModifiedBy = Session["DEFAULTUSER"].ToString() });
                TempData["isSaved"] = result;

                if (result)
                    TempData["resultMessage"] = string.Format("Customer [{0}] Deleted Successfully", customerCode);
                else
                    TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                TempData["isSaved"] = false;
                TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("Customers");


        }

        #endregion

        #region Supplier Product



        [Route("SupplierProducts")]
        [HttpGet]
        public ActionResult SupplierProducts(string supplierCode)
        {
            NetStock.Contract.Customer supplier = new Contract.Customer();




            supplier = new NetStock.BusinessFactory.CustomerBO().GetCustomer(new Contract.Customer { CustomerCode = supplierCode });

            if (supplier.CustomerType == "SUPPLIER")
            {
                supplier.CustomerProducts = new NetStock.BusinessFactory.CustomerProductBO().GetCustomerProductsList(supplier.CustomerCode);
            }

            if (supplier.CustomerProducts == null || supplier.CustomerProducts.Count == 0)
                supplier.CustomerProducts = new List<Contract.CustomerProduct>();

            return View("SupplierProduct", supplier);
        }

        [Route("SupplierProducts")]
        [HttpPost]
        public ActionResult SupplierProducts(NetStock.Contract.Customer supplier)
        {

            var items = ((NetStock.Contract.Customer)HttpContext.Session["SupplierProducts"]).CustomerProducts;



            foreach (var item in items)
            {
                item.CustomerCode = supplier.CustomerCode;
                item.CustomerName = supplier.CustomerName;

                var result = new NetStock.BusinessFactory.CustomerProductBO().SaveCustomerProduct(item);

            }

            return RedirectToAction("Suppliers");
        }




        [Route("SupplierProductList")]
        public ActionResult SupplierProductList(string supplierCode)
        {
            var productlist = new NetStock.BusinessFactory.CustomerProductBO().GetList();

            return PartialView("SupplierProductList", productlist);

        }

        [Route("EditSupplierProduct")]
        [HttpGet]
        public ActionResult EditSupplierProduct(string supplierCode, string productCode)
        {
            NetStock.Contract.CustomerProduct customerproduct = new Contract.CustomerProduct();

            customerproduct.MatchProductList = NetStock.Utility.GetProductList();

            return PartialView("AddSupplierProduct", customerproduct);

        }




        [Route("SaveSupplierProduct")]
        [HttpPost]
        public JsonResult SaveSupplierProduct(NetStock.Contract.Customer supplier)
        {
            try
            {
                supplier.Status = true;
                supplier.CreatedBy = Session["DEFAULTUSER"].ToString();
                supplier.ModifiedBy = Session["DEFAULTUSER"].ToString();

                var lstCustomerProducts = supplier.CustomerProducts;

                if (lstCustomerProducts != null || lstCustomerProducts.Count > 0)
                {
                    lstCustomerProducts.Update(p => { p.CustomerCode = supplier.CustomerCode; p.CostPrice = 0; });
                }


                var result = new NetStock.BusinessFactory.CustomerProductBO().SaveList(lstCustomerProducts);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", ex.Message);
            }



            return Json(new { success = true, Message = "Products saved successfully.", supplierData = supplier });

        }


        [Route("DeleteSupplierProducts")]
        [HttpPost]
        public JsonResult DeleteSupplierProducts(string supplierCode)
        {
            try
            {

                var result = new NetStock.BusinessFactory.CustomerProductBO().DeleteAllCustomerProducts(supplierCode);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", ex.Message);
            }



            return Json(new { success = true, Message = "Products Deleted successfully.", SupplierCode = supplierCode });

        }


        #endregion


        #region UOM
        [Route("UOMList")]
        [HttpGet]
        public ActionResult UOMList()
        {
            var lstUOM = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "UOM" && dt.Status == true).ToList();
            return View("UOMList", lstUOM);
        }




        [Route("EditUOM")]
        [HttpGet]
        public ActionResult EditUOM(string lookupCode)
        {
            var uom = new NetStock.Contract.Lookup();

            if (lookupCode != null && lookupCode.Length > 0)
                uom = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "UOM" && dt.LookupCode == lookupCode).FirstOrDefault();

            return PartialView("UOM", uom);
        }


        [Route("EditUOM")]
        [HttpPost]
        public ActionResult SaveUOM(NetStock.Contract.Lookup uomItem)
        {
            try
            {
                if (uomItem == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                uomItem.Status = true;
                uomItem.Category = "UOM";
                uomItem.Description2 = "";
                var result = new NetStock.BusinessFactory.LookupBO().SaveLookup(uomItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("UOMList");
        }


        [Route("DeleteUOM")]
        [HttpGet]
        public ActionResult DeleteUOM(string lookupCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.LookupBO().DeleteLookup(new NetStock.Contract.Lookup { LookupCode = lookupCode });

                TempData["isSaved"] = result;

                if (result)
                    TempData["resultMessage"] = string.Format("UOM [{0}] Deleted Successfully", lookupCode);
                else
                    TempData["resultMessage"] = "Unable to DELETE the Record!";

            }
            catch (Exception ex)
            {
                TempData["isSaved"] = false;
                TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("UOMList", "MasterData");
        }

        #endregion


        #region IncoTerms

        [Route("IncoTermList")]
        [HttpGet]
        public ActionResult IncoTermList()
        {
            var lstUOM = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "INCOTERM" && dt.Status == true).ToList();
            return View("IncoTermList", lstUOM);
        }


        [Route("EditIncoTerm")]
        [HttpGet]
        public ActionResult EditIncoTerm(string lookupCode)
        {
            var uom = new NetStock.Contract.Lookup();

            if (lookupCode != null && lookupCode.Length > 0)
                uom = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "INCOTERM" && dt.LookupCode == lookupCode).FirstOrDefault();

            return PartialView("IncoTerm", uom);
        }


        [Route("EditIncoTerm")]
        [HttpPost]
        public ActionResult SaveIncoTerm(NetStock.Contract.Lookup uomItem)
        {
            try
            {
                if (uomItem == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                uomItem.Status = true;
                uomItem.Category = "INCOTERM";
                uomItem.Description2 = "";
                var result = new NetStock.BusinessFactory.LookupBO().SaveLookup(uomItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("IncoTermList");
        }


        [Route("DeleteIncoTerm")]
        [HttpGet]
        public ActionResult DeleteIncoTerm(string lookupCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.LookupBO().DeleteLookup(new NetStock.Contract.Lookup { LookupCode = lookupCode });
                TempData["isSaved"] = result;

                if (result)
                    @TempData["resultMessage"] = string.Format("INCO-TERM [{0}] Deleted Successfully", lookupCode);
                else
                    @TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("IncoTermList", "MasterData");


        }



        #endregion


        #region Warehouse Location
        [Route("LocationList")]
        [HttpGet]
        public ActionResult LocationList()
        {
            var lstLookup = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "LOCATION" && dt.Status == true).ToList();
            return View("LocationList", lstLookup);
        }


        [Route("EditLocation")]
        [HttpGet]
        public ActionResult EditLocation(string lookupCode)
        {
            var location = new NetStock.Contract.Lookup();

            if (lookupCode != null && lookupCode.Length > 0)
                location = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "LOCATION" && dt.LookupCode == lookupCode).FirstOrDefault();

            return PartialView("WareHouseLocation", location);
        }


        [Route("EditLocation")]
        [HttpPost]
        public ActionResult SaveLocation(NetStock.Contract.Lookup locationItem)
        {
            try
            {
                if (locationItem == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                locationItem.Status = true;
                locationItem.Category = "LOCATION";
                locationItem.Description2 = "";
                var result = new NetStock.BusinessFactory.LookupBO().SaveLookup(locationItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("LocationList");
        }

        [Route("DeleteLocationList")]
        [HttpGet]
        public ActionResult DeleteLocationList(string locationCode)
        {
            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.LookupBO().DeleteLookup(new NetStock.Contract.Lookup { LookupCode = locationCode });
                TempData["isSaved"] = result;

                if (result)
                    @TempData["resultMessage"] = string.Format("Product Category  [{0}] Deleted Successfully", locationCode);
                else
                    @TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("LocationList", "MasterData");
        }

        #endregion


        #region Payment Type

        [Route("PaymentTypeList")]
        [HttpGet]
        public ActionResult PaymentTypeList()
        {
            var lstLookup = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "PAYMENTTYPE" && dt.Status == true).ToList();
            return View("PaymentTypeList", lstLookup);
        }


        [Route("EditPaymentType")]
        [HttpGet]
        public ActionResult EditPaymentType(string lookupCode)
        {
            var location = new NetStock.Contract.Lookup();

            if (lookupCode != null && lookupCode.Length > 0)
                location = new NetStock.BusinessFactory.LookupBO().GetList().Where(dt => dt.Category == "PAYMENTTYPE" && dt.LookupCode == lookupCode).FirstOrDefault();

            return PartialView("PaymentType", location);
        }


        [Route("EditPaymentType")]
        [HttpPost]
        public ActionResult SavePaymentType(NetStock.Contract.Lookup paymentTypeItem)
        {
            try
            {
                if (paymentTypeItem == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                paymentTypeItem.Status = true;
                paymentTypeItem.Category = "PAYMENTTYPE";
                paymentTypeItem.Description2 = "";
                var result = new NetStock.BusinessFactory.LookupBO().SaveLookup(paymentTypeItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("PaymentTypeList");
        }

        [Route("DeletePaymentType")]
        [HttpGet]
        public ActionResult DeletePaymentType(string paymentTypeCode)
        {
            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.LookupBO().DeleteLookup(new NetStock.Contract.Lookup { LookupCode = paymentTypeCode });
                TempData["isSaved"] = result;

                if (result)
                    @TempData["resultMessage"] = string.Format("Payment Type  [{0}] Deleted Successfully", paymentTypeCode);
                else
                    @TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("PaymentTypeList", "MasterData");
        }



        #endregion


        #region Product Category
        [Route("ProductCategoryList")]
        [HttpGet]
        public ActionResult ProductCategoryList()
        {
            var lstLookup = new NetStock.BusinessFactory.ProductCategoryBO().GetList();
            return View("ProductCategoryList", lstLookup);
        }




        [Route("DeleteProductCategory")]
        [HttpGet]
        public ActionResult DeleteProductCategory(string categoryCode)
        {
            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.ProductCategoryBO().DeleteProductCategory(new NetStock.Contract.ProductCategory { CategoryCode = categoryCode });
                TempData["isSaved"] = result;

                if (result)
                    @TempData["resultMessage"] = string.Format("Product Category  [{0}] Deleted Successfully", categoryCode);
                else
                    @TempData["resultMessage"] = "Unable to DELETE the Record!";
            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            return RedirectToAction("ProductCategoryList", "MasterData");
        }

        [Route("EditProductCategory")]
        [HttpGet]
        public ActionResult EditProductCategory(string categoryCode)
        {
            var productCategory = new NetStock.Contract.ProductCategory();

            if (categoryCode != null && categoryCode.Length > 0)
                productCategory = new NetStock.BusinessFactory.ProductCategoryBO().GetProductCategory(new ProductCategory { CategoryCode = categoryCode });

            return PartialView("ProductCategory", productCategory);
        }


        [Route("EditProductCategory")]
        [HttpPost]
        public ActionResult SaveProductCategory(NetStock.Contract.ProductCategory productCategory)
        {
            try
            {
                if (productCategory == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                productCategory.CreatedBy = Session["DEFAULTUSER"].ToString();
                productCategory.ModifiedBy = Session["DEFAULTUSER"].ToString();

                var result = new NetStock.BusinessFactory.ProductCategoryBO().SaveProductCategory(productCategory);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("ProductCategoryList");
        }


        #endregion



        #region Currency

        [Route("CurrencyList")]
        [HttpGet]
        public ActionResult CurrencyList()
        {
            var lstCurrency = new NetStock.BusinessFactory.CurrencyBO().GetList().ToList();
            return View("CurrencyList", lstCurrency);
        }


        [Route("EditCurrency")]
        [HttpGet]
        public ActionResult EditCurrency(string CurrencyCode)
        {
            var currency = new NetStock.Contract.Currency();

            if (CurrencyCode != null && CurrencyCode.Length > 0)
                currency = new NetStock.BusinessFactory.CurrencyBO().GetList().Where(dt => dt.CurrencyCode == CurrencyCode).FirstOrDefault();

            return PartialView("Currency", currency);
        }


        [Route("EditCurrency")]
        [HttpPost]
        public ActionResult SaveCurrency(NetStock.Contract.Currency currencyItem)
        {
            try
            {
                if (currencyItem == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);


                currencyItem.CurrencyCode = currencyItem.CurrencyCode;
                currencyItem.Description = currencyItem.Description;
                currencyItem.Description1 = currencyItem.Description1;
                var result = new NetStock.BusinessFactory.CurrencyBO().SaveCurrency(currencyItem);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            return RedirectToAction("CurrencyList");
        }


        [Route("DeleteCurrency")]
        [HttpGet]
        public ActionResult DeleteCurrency(string CurrencyCode)
        {

            var result = false;

            string message = string.Empty;
            try
            {
                result = new NetStock.BusinessFactory.CurrencyBO().DeleteCurrency(new NetStock.Contract.Currency { CurrencyCode = CurrencyCode });

                @TempData["isSaved"] = result;

                if (result)
                    @TempData["resultMessage"] = string.Format("Currency [{0}] Deleted Successfully", CurrencyCode);
                else
                    @TempData["resultMessage"] = "Unable to DELETE the Record!";

            }
            catch (Exception ex)
            {
                @TempData["isSaved"] = false;
                @TempData["resultMessage"] = string.Format("Error Occurred {0}", ex.Message.ToString());
                ModelState.AddModelError("Error", ex.Message);
            }

            //return Json(new { result = result, Message = lookupCode + " Deleted successfully.", UOMCode = lookupCode });

            return RedirectToAction("CurrencyList", "MasterData");

        }

        #endregion




        [Route("UpdateCustomerAddress")]
        [HttpPost]
        public JsonResult UpdateCustomerAddress(Customer customer)
        {

            var address = customer.CustomerAddress;
            address.AddressLinkID = customer.CustomerCode;
            address.AddressType = "Customer";
            address.ModifiedBy = Session["DEFAULTUSER"].ToString();
            address.CreatedBy = Session["DEFAULTUSER"].ToString();
            address.IsActive = true;
            address.IsBilling = true;


            var result = new NetStock.BusinessFactory.AddressBO().SaveAddress(address);

            customer = new NetStock.BusinessFactory.CustomerBO().GetCustomer(new NetStock.Contract.Customer { CustomerCode = customer.CustomerCode });

            return Json(GetFullAddress(customer.CustomerAddress.FullAddress), JsonRequestBehavior.AllowGet);
        }

        protected string GetFullAddress(string address)
        {
            if (address.IndexOf(",") > 0)
            {
                var list = address.Split(',').ToList();

                var html = "";
                for (var i = 0; i < list.Count; i++)
                {
                    html += list[i] + "<br/>";
                }

                return html;
            }
            return address;
        }



        protected List<ChartOfAccountsVm> PopulateChilds(string code, List<NetStock.Contract.ChartOfAccount> chartOfAccountList)
        {
            try
            {
                var obj = chartOfAccountList.Where(x => x.AccountGroup == code)
                                       .Select(x => new ChartOfAccountsVm()
                                       {
                                           AccountCode = x.AccountCode,
                                           Description = x.Description,
                                           Description2 = x.Description2,
                                           AccountGroup = x.AccountGroup,
                                           DebitCredit = x.DebitCredit,
                                           CurrencyCode = x.CurrencyCode,
                                           Status = x.Status,
                                           Sequence = x.Sequence,
                                           BranchID = x.BranchID,
                                           CreatedBy = x.CreatedBy,
                                           CreatedOn = x.CreatedOn,
                                           ModifiedBy = x.ModifiedBy,
                                           ModifiedOn = x.ModifiedOn,
                                           AccountGroupDescription = x.AccountGroupDescription,
                                          // COAList = PopulateChilds(x.AccountCode, chartOfAccountList)
                                       }).ToList<ChartOfAccountsVm>();

                return obj;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Route("ChartOfAccountMaster")]
        public ActionResult ChartOfAccountMaster(string branchID, string _accountCode = "")
        {
            var accountCode = Request.Form["hdnAccountCode"];
            if (string.IsNullOrWhiteSpace(accountCode))
            {
                if (!string.IsNullOrWhiteSpace(_accountCode))
                {
                    accountCode = _accountCode;
                }
            }
            ViewData["hdnAccountCode"] = accountCode;

            short branchId = -1;
            if (branchID != null)
            {
                branchId = Convert.ToInt16(branchID);
            }
            else
            {
                branchId = Convert.ToInt16(Session["BranchId"]);
            }
            //var ChartOfAccounts = new NetStock.BusinessFactory.ChartOfAccountBO().GetList();
            var accountGroupList = new NetStock.BusinessFactory.AccountGroupBO().GetList();
            var chartOfAccountsList = new NetStock.BusinessFactory.ChartOfAccountBO().GetList(branchId);

            List<AccountGroupVm> accountGroupVm = new List<AccountGroupVm>();
            for (var i = 0; i < accountGroupList.Count; i++)
            {
                var code = accountGroupList[i].Code;
                AccountGroupVm accountGroup = new AccountGroupVm();
                accountGroup.Code = accountGroupList[i].Code;
                accountGroup.AccountType = accountGroupList[i].AccountType;
                accountGroup.Description = accountGroupList[i].Description;
                accountGroup.Description1 = accountGroupList[i].Description1;
                accountGroup.Description2 = accountGroupList[i].Description2;
                accountGroup.SequenceNo = accountGroupList[i].SequenceNo;

                accountGroup.COAList = chartOfAccountsList.Where(x => x.AccountGroup == accountGroupList[i].Code)
                                                          .Select(x => new ChartOfAccountsVm()
                                                          {
                                                              AccountCode = x.AccountCode,
                                                              Description = x.Description,
                                                              Description2 = x.Description2,
                                                              AccountGroup = x.AccountGroup,
                                                              DebitCredit = x.DebitCredit,
                                                              CurrencyCode = x.CurrencyCode,
                                                              Status = x.Status,
                                                              Sequence = x.Sequence,
                                                              BranchID = x.BranchID,
                                                              CreatedBy = x.CreatedBy,
                                                              CreatedOn = x.CreatedOn,
                                                              ModifiedBy = x.ModifiedBy,
                                                              ModifiedOn = x.ModifiedOn,
                                                              AccountGroupDescription = x.AccountGroupDescription,
                                                              COAList = PopulateChilds(x.AccountCode, chartOfAccountsList)
                                                          }).ToList<ChartOfAccountsVm>();

                accountGroupVm.Add(accountGroup);
            }
            /*
            var list = ChartOfAccounts.GroupBy(x => x.AccountGroup)
                                        .Select(x => new MyItem() {
                                            Description = x.FirstOrDefault().Description,
                                            Description2 = x.FirstOrDefault().Description2,
                                            AccountGroupDescription = x.FirstOrDefault().AccountGroupDescription,
                                            Sequence = x.FirstOrDefault().Sequence,
                                            AccountCode = x.FirstOrDefault().AccountCode,
                                            ChildItems = (ChartOfAccounts.Where(y => y.AccountGroup == x.FirstOrDefault().AccountGroup)
                                                            .Select(y => new MyItem() {
                                                                Description = y.Description,
                                                                Description2 = y.Description2,
                                                                AccountGroupDescription = y.AccountGroupDescription,
                                                                Sequence = y.Sequence,
                                                                AccountCode = y.AccountCode
                                                            })).ToList()
                                        }).ToList<MyItem>();*/

            if (Request.IsAjaxRequest())
            {
                return Json(accountGroupVm, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View("ChartOfAccounts", accountGroupVm);
            }
        }

        [Route("EditChartOfAccount")]
        [ChildActionOnly]
        public ActionResult EditChartOfAccount(string accountCode)
        {
            return PartialView("ChartOfAccount", ChartOfAccountData(accountCode));
        }

        public ChartOfAccount ChartOfAccountData(string accountCode)
        {
            var coaItem = new NetStock.Contract.ChartOfAccount();

            if (accountCode != null && accountCode.Length > 0)
                coaItem = new NetStock.BusinessFactory.ChartOfAccountBO().GetChartOfAccount(new Contract.ChartOfAccount { AccountCode = accountCode, BranchID = Utility.SsnBranch });

            coaItem.CurrencyCodeList = Utility.GetCurrencyItemList();
            coaItem.AccountGroupList = Utility.GetAccountGroupItemList(Convert.ToInt16((Session["BranchId"]))).Where(x => x.Value != accountCode).ToList();
            coaItem.DebitCreditList = Utility.GetLookupItemList("DebitCredit");

            return coaItem;
        }

        [HttpGet]
        [Route("chartofaccount/{accountCode}")]
        public JsonResult ChartOfAccountByCode(string accountCode)
        {
            return Json(ChartOfAccountData(accountCode), JsonRequestBehavior.AllowGet);
        }


        [Route("EditChartOfAccount")]
        [HttpPost]
        public ActionResult UpdateChartOfAccount(NetStock.Contract.ChartOfAccount item)
        {

            if (ModelState.IsValid)
            {
                if (item.BranchID == 0)
                {
                    item.BranchID = Utility.SsnBranch;
                }
                if (item == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

                item.CreatedBy = Utility.DEFAULTUSER;
                item.ModifiedBy = Utility.DEFAULTUSER;

                var result = new NetStock.BusinessFactory.ChartOfAccountBO().SaveChartOfAccount(item);
            }
            return RedirectToAction("ChartOfAccountMaster", "MasterData");
        }

        [Route("DeleteChartOfAccount")]
        [HttpPost]
        public ActionResult DeleteChartOfAccount(NetStock.Contract.ChartOfAccount item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = new NetStock.BusinessFactory.ChartOfAccountBO().DeleteChartOfAccount(item);
                }
                catch (Exception ex)
                {
                    TempData["TempExMsg"] = ex.Message;
                }

            }
            return RedirectToAction("ChartOfAccountMaster", "MasterData", new { branchID = item.BranchID, _accountCode = "" });
        }


        [Route("ChargeCodeMaster")]
        public ActionResult ChargeCodeMaster()
        {
            var chargeCodes = new NetStock.BusinessFactory.ChargeCodeBO().GetList(Utility.SsnBranch).Where(x => x.Status == true).ToList();

            return View("ChargeCode", chargeCodes);

            //EditChargeCode
        }

        [Route("EditChargeCode")]
        [HttpGet]
        public ActionResult EditChargeCode(string chargeCode)
        {
            var chargecodeItem = new NetStock.Contract.ChargeCodeMaster();


            if (chargeCode != null && chargeCode.Length > 0)
                chargecodeItem = new NetStock.BusinessFactory.ChargeCodeBO().GetChargeCode(new Contract.ChargeCodeMaster { ChargeCode = chargeCode, BranchID = Utility.SsnBranch });

            chargecodeItem.BillingUnitList = Utility.GetLookupItemList("UOM");
            chargecodeItem.CreditTermList = Utility.GetLookupItemList("CreditTerm");
            chargecodeItem.PaymentTermList = Utility.GetLookupItemList("PaymentTerm");
            chargecodeItem.AccountCodeList = Utility.GetAccountCodeItemList();
            chargecodeItem.VATAccountCodeList = Utility.GetAccountCodeItemList();
            chargecodeItem.WHAccountCodeList = Utility.GetAccountCodeItemList();


            //return PartialView("AddChargeCode", chargecodeItem);
            return View("NewChargeCode", chargecodeItem);
        }


        [Route("EditChargeCode")]
        [HttpPost]
        public ActionResult UpdateChargeCode(NetStock.Contract.ChargeCodeMaster item)
        {

            if (ModelState.IsValid)
            {

                if (item == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

                item.CreatedBy = Utility.DEFAULTUSER;
                item.ModifiedBy = Utility.DEFAULTUSER;
                item.BranchID = Utility.SsnBranch;

                var result = new NetStock.BusinessFactory.ChargeCodeBO().SaveChargeCode(item);
            }


            return RedirectToAction("ChargeCodeMaster", "MasterData");

        }


        [Route("SaveChargeCode")]
        [HttpPost]
        public JsonResult SaveChargeCode(NetStock.Contract.ChargeCodeMaster item)
        {

            if (ModelState.IsValid)
            {

                if (item == null)
                    return Json(new { Errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);

                item.CreatedBy = Utility.DEFAULTUSER;
                item.ModifiedBy = Utility.DEFAULTUSER;
                item.Status = true;
                item.BranchID = Utility.SsnBranch;

                var result = new NetStock.BusinessFactory.ChargeCodeBO().SaveChargeCode(item);
            }

            //return RedirectToAction("ChargeCodeMaster", "MasterData");
            return Json(new { success = true, Message = string.Format("Charge Code {0} saved successfully.", item.ChargeCode), ChargeCodeMaster = item });

        }


        [Route("DeleteChargeCode")]
        [HttpPost]
        public JsonResult DeleteChargeCode(NetStock.Contract.ChargeCodeMaster item)
        {
            try
            {
                if (item != null)
                {
                    item.CreatedBy = Utility.DEFAULTUSER;
                    item.ModifiedBy = Utility.DEFAULTUSER;
                    item.Status = false;
                    item.BranchID = Utility.SsnBranch;
                    //var chargecodeItem = new NetStock.BusinessFactory.ChargeCodeBO().GetChargeCode(new Contract.ChargeCodeMaster { ChargeCode = chargeCode, BranchID = Utility.SsnBranch });
                    var result = new NetStock.BusinessFactory.ChargeCodeBO().DeleteChargeCode(item);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(new { success = true, Message = string.Format("Charge Code {0} deleted successfully.", item.ChargeCode), ChargeCodeMaster = item });
        }

        #region Asset Master

        [Route("AssetMaster")]
        public ActionResult AssetMaster()
        {
            var assets = new NetStock.BusinessFactory.AssetHeaderBO().GetList(Utility.SsnBranch).Where(x => x.Status == true).ToList();



            return View("AssetMaster", assets);

            //EditChargeCode
        }

        [Route("EditAsset")]
        [HttpGet]
        public ActionResult EditAsset(string assetCode)
        {
            var assetItem = new NetStock.Contract.AssetHeader();


            if (assetCode != null && assetCode.Length > 0)
                assetItem = new NetStock.BusinessFactory.AssetHeaderBO().GetAssetHeader(new AssetHeader { AssetCode = assetCode, BranchID = Utility.SsnBranch });
            else
                assetItem.BuyingDate = DateTime.Today.Date;

            assetItem.DepreciationTypeList = Utility.GetLookupItemList("Depreciation");

            return View("AddAsset", assetItem);
        }

        [HttpPost]
        [Route("SaveAsset")]
        public ActionResult SaveAsset(NetStock.Contract.AssetHeader assetHeader)
        {

            assetHeader.CreatedBy = Utility.DEFAULTUSER;
            assetHeader.ModifiedBy = Utility.DEFAULTUSER;
            assetHeader.Status = Utility.DEFAULTSTATUS;
            assetHeader.BranchID = Utility.SsnBranch;


            var result = new NetStock.BusinessFactory.AssetHeaderBO().SaveAssetHeader(assetHeader);

            //return RedirectToAction("AssetMaster", "MasterData");
            return Json(new { success = true, Message = string.Format("Asset Code {0} saved successfully.", assetHeader.AssetCode) });
        }

        [HttpPost]
        [Route("DeleteAsset")]
        public ActionResult DeleteAsset(NetStock.Contract.AssetHeader assetHeader)
        {
            if (assetHeader != null)
            {
                try
                {
                    assetHeader.CreatedBy = Utility.DEFAULTUSER;
                    assetHeader.ModifiedBy = Utility.DEFAULTUSER;
                    assetHeader.Status = false;
                    assetHeader.BranchID = Utility.SsnBranch;
                    var item = new NetStock.BusinessFactory.AssetHeaderBO().DeleteAssetHeader(assetHeader);
                }
                catch (Exception)
                {

                    throw;
                }

            }

            var assets = new NetStock.BusinessFactory.AssetHeaderBO().GetList(Utility.SsnBranch);

            // return View("AssetMaster", assets);
            return Json(new { success = true, Message = string.Format("Asset Code {0} deleted successfully.", assetHeader.AssetCode) });
        }

        #endregion




        //protected List<ChartOfAccountsVm> PopulateChilds(string code, List<NetStock.Contract.ChartOfAccount> chartOfAccountList)
        //{
        //    var obj = chartOfAccountList.Where(x => x.AccountGroup == code)
        //                                .Select(x => new ChartOfAccountsVm()
        //                                {
        //                                    AccountCode = x.AccountCode,
        //                                    Description = x.Description,
        //                                    Description2 = x.Description2,
        //                                    AccountGroup = x.AccountGroup,
        //                                    DebitCredit = x.DebitCredit,
        //                                    CurrencyCode = x.CurrencyCode,
        //                                    Status = x.Status,
        //                                    Sequence = x.Sequence,
        //                                    BranchID = x.BranchID,
        //                                    CreatedBy = x.CreatedBy,
        //                                    CreatedOn = x.CreatedOn,
        //                                    ModifiedBy = x.ModifiedBy,
        //                                    ModifiedOn = x.ModifiedOn,
        //                                    AccountGroupDescription = x.AccountGroupDescription,
        //                                    COAList = PopulateChilds(x.AccountCode, chartOfAccountList)
        //                                }).ToList<ChartOfAccountsVm>();

        //    return obj;
        //}


       



    }


    public class AccountGroupVm
    {
        public string Code { get; set; }
        public string AccountType { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public Int16 SequenceNo { get; set; }
        public List<ChartOfAccountsVm> COAList { get; set; }
    }


    public class ChartOfAccountsVm
    {
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string AccountGroup { get; set; }
        public string DebitCredit { get; set; }
        public string CurrencyCode { get; set; }
        public bool Status { get; set; }
        public Int16 Sequence { get; set; }
        public Int16 BranchID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string AccountGroupDescription { get; set; }
        public List<ChartOfAccountsVm> COAList { get; set; }
    }

}