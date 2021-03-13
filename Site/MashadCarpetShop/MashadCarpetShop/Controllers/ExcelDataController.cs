using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Helpers;
using Models;
using ViewModels;

namespace MashadCarpetShop.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ExcelDataController : Controller
    {
        public ActionResult Import()
        {
            UploadFile uploadFile = new UploadFile();
            return View(uploadFile);
        }

        [HttpPost]
        public ActionResult Import(UploadFile UploadFile)
        {
            if (ModelState.IsValid)
            {
                if (UploadFile.ExcelFile.ContentLength > 0)
                {
                    if (UploadFile.ExcelFile.FileName.EndsWith(".xlsx") || UploadFile.ExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.ExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(String.Empty, $"Check your file. {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet("Sheet1");

                        }
                        catch
                        {
                            ModelState.AddModelError(String.Empty, "sheet not found!");
                            return View();
                        }
                        WorkSheet.FirstRow().Delete();//if you want to remove ist row

                        bool colorValidate = true;
                        string invalidColorTitle = "";

                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            if (!ColorValidator(row.Cell(2).Value.ToString()))
                            {
                                colorValidate = false;
                                invalidColorTitle = row.Cell(2).Value.ToString();
                                break;
                            }
                        }

                        if (colorValidate)
                        {
                            foreach (var row in WorkSheet.RowsUsed())
                            {

                                int designNo = Convert.ToInt32(row.Cell(1).Value.ToString());
                                int tedadRang = Convert.ToInt32(row.Cell(5).Value.ToString());
                                int amount = Convert.ToInt32(row.Cell(7).Value.ToString());
                                int qty = Convert.ToInt32(row.Cell(8).Value.ToString());

                                if (designNo == 802001)
                                {
                                    int test = 1;
                                }

                                UpdateRow(designNo,
                                    row.Cell(2).Value.ToString(),
                                    row.Cell(3).Value.ToString(),
                                    row.Cell(4).Value.ToString(),
                                    tedadRang,
                                    row.Cell(6).Value.ToString(),
                                    amount, qty);

                                db.SaveChanges();


                             
                            }
                            InsertExcellHistory(UploadFile);
                            db.SaveChanges();


                            TempData["success"] = "اطلاعات فایل اکسل با موفقیت بارگزاری شد";

                        }
                        else
                        {
                            TempData["invalidColor"] = "رنگ " + invalidColorTitle +
                                                       " در جدول رنگ ها موجود نمی باشد. ابتدا از قسمت مدیریت رنگ ها این رنگ را اضافه کنید.";

                            return View();
                        }

                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Only .xlsx and .xls files are allowed");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Not a valid file");
                    return View();
                }
            }
            return View();
        }

        public void InsertExcellHistory(UploadFile UploadFile)
        {
            string newFilenameUrl = string.Empty;

            string filename = Path.GetFileName(UploadFile.ExcelFile.FileName);
            string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                 + Path.GetExtension(filename);

            newFilenameUrl = "/Uploads/product/" + newFilename;
            string physicalFilename = Server.MapPath(newFilenameUrl);
            UploadFile.ExcelFile.SaveAs(physicalFilename);
            ExcellHistory excellHistory = new ExcellHistory()
            {
                Id = Guid.NewGuid(),
                FileUrl = newFilenameUrl,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now
            };

            db.ExcellHistories.Add(excellHistory);
        }
        public void test()
        {
            decimal a = Convert.ToDecimal("");
            decimal ab = Convert.ToInt32("");
            decimal aaaDecimal = Convert.ToDecimal(null);
            decimal aaasdb = Convert.ToInt32(null);
        }

        public bool ConvertMattresVal(string hasMattres)
        {
            if (hasMattres.ToLower() == "y")
                return true;

            return false;
        }
        public decimal DecimalConvertor(string amount)
        {
            try
            {
                if (string.IsNullOrEmpty(amount))
                    return 0;

                return Convert.ToDecimal(amount);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void UpdateRow(int designNo, string colorTitle, string size, string shane, int tedadrang, string tarakom, decimal amount, int qty)
        {

            CodeGenerator codeGenerator = new CodeGenerator();
            Color color = db.Colors.FirstOrDefault(c => c.Title == colorTitle);

            string realSize = GetOrginalSize(size);
            Product product =
                db.Products.FirstOrDefault(c => c.DesignNo == designNo && c.IsDeleted == false && c.ParentId == null);

            if (product != null)
            {
                Product productWithColor =
                    db.Products.FirstOrDefault(c =>
                        c.DesignNo == designNo && c.IsDeleted == false && c.Color.Title == colorTitle);

                if (productWithColor != null)
                {
                    ProductSize productSize = db.ProductSizes.FirstOrDefault(c =>
                        c.ProductId == productWithColor.Id && c.IsDeleted == false && c.Size.Title == realSize);

                    if (productSize != null)
                    {
                        productSize.Amount = amount;
                        productSize.Stock = qty;
                        productSize.LastModifiedDate = DateTime.Now;

                        if (productWithColor.Amount == 0 || productWithColor.Amount > productSize.Amount)
                            productWithColor.Amount = amount;
                    }
                    else
                    {
                        CreateProductSize(realSize, qty, amount, productWithColor);

                    }
                }
                else
                {
                    Product oProductWithColor =
                        CreateProduct(product, color, colorTitle, designNo, tedadrang, shane, tarakom, amount);

                    CreateProductSize(realSize, qty, amount, oProductWithColor);
                }
            }
            else
            {
                Product oProductWithColor =
                    CreateProduct(null, color, colorTitle, designNo, tedadrang, shane, tarakom, amount);

                CreateProductSize(realSize, qty, amount, oProductWithColor);
            }
        }

        public Product CreateProduct(Product parentProduct, Color color, string colorTitle, int designNo, int tedadrang,
            string shane, string tarakom, decimal amount)
        {
            if (parentProduct != null)
            {
                Product oProductWithColor = new Product()
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    Title = "طرح " + parentProduct.DesignNo + " رنگ " + colorTitle,
                    PageTitle = "طرح " + parentProduct.DesignNo + " رنگ " + colorTitle,
                    ColorId = color.Id,
                    Amount = amount,
                    Code = codeGenerator.ReturnProductCode(),
                    DesignNo = designNo,
                    Frame = tedadrang,
                    ImageUrl = "",
                    Order = 0,
                    ProductGroupId = parentProduct.ProductGroupId,
                    ParentId = parentProduct.Id,
                    Visit = 0,
                    Shots = tarakom,
                    Reeds = shane,
                    IsAvailable = true,

                };

                db.Products.Add(oProductWithColor);

                return oProductWithColor;
            }
            else
            {
                string systemname = shane + "-" + tedadrang + "-" + tarakom;

                ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == systemname);
                if (productGroup != null)
                {
                    Product oParentProduct = new Product()
                    {
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        CreationDate = DateTime.Now,
                        IsActive = true,
                        Title = "طرح " + designNo + " رنگ " + colorTitle,
                        PageTitle = "طرح " + designNo + " رنگ " + colorTitle,
                        ColorId = color.Id,
                        Amount = amount,
                        Code = codeGenerator.ReturnProductCode(),
                        DesignNo = designNo,
                        Frame = tedadrang,
                        ImageUrl = "",
                        Order = 0,
                        ProductGroupId = productGroup.Id,
                        ParentId = null,
                        Visit = 0,
                        Shots = tarakom,
                        Reeds = shane,
                        IsAvailable = true
                    };

                    db.Products.Add(oParentProduct);

                    Product childProduct = new Product()
                    {
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        CreationDate = DateTime.Now,
                        IsActive = true,
                        Title = "طرح " + designNo + " رنگ " + colorTitle,
                        PageTitle = "طرح " + designNo + " رنگ " + colorTitle,
                        ColorId = color.Id,
                        Amount = amount,
                        Code = codeGenerator.ReturnProductCode(),
                        DesignNo = designNo,
                        Frame = tedadrang,
                        ImageUrl = "",
                        Order = 0,
                        ProductGroupId = productGroup.Id,
                        ParentId = oParentProduct.Id,
                        Visit = 0,
                        Shots = tarakom,
                        Reeds = shane,
                        IsAvailable = true
                    };

                    db.Products.Add(childProduct);

                    return childProduct;
                }

                return null;
            }
        }

        public void CreateProductSize(string realSize, int qty, decimal amount, Product product)
        {
            Size oSize = db.Sizes.FirstOrDefault(c => c.Title == realSize && c.IsDeleted == false);

            if (oSize == null)
            {
                oSize = CreateNewSize(realSize);
                db.SaveChanges();
            }
            ProductSize oProductSize = new ProductSize()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                IsDeleted = false,
                Stock = qty,
                SeedStock = qty,
                Amount = amount,
                SizeId = oSize.Id,
                IsActive = true,
                IsAvailable = true,
                IsInPromotion = false,
                SellNumber = 0,
                ProductId = product.Id
            };

            db.ProductSizes.Add(oProductSize);


            if (product.Amount == 0 || product.Amount > oProductSize.Amount)
                product.Amount = amount;
        }


        public bool ColorValidator(string colorTitle)
        {
            if (db.Colors.Any(c => c.Title == colorTitle && c.IsDeleted == false))
                return true;

            return false;
        }
        public Size CreateNewSize(string title)
        {
            Size size = new Size()
            {
                Id = Guid.NewGuid(),
                Title = title,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now,
            };
            db.Sizes.Add(size);

            return size;
        }

        public string GetOrginalSize(string size)
        {
            string[] newsize = size.Split('*');

            return (Convert.ToDouble(newsize[0]) / 100) + "*" + (Convert.ToDouble(newsize[1]) / 100);

        }
        private DatabaseContext db = new DatabaseContext();
        CodeGenerator codeGenerator = new CodeGenerator();

    }
}