using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("product", "product");
        private static string functionCode = "QLSP";

        // GET: api/Product
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Product> data = db.Product.Where(c => c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("ProductId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("ProductId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                        //def.data = data.ToList();
                        def.data = data.Select(e => new
                        {
                            e.ProductId,
                            e.Code,
                            e.Name,
                            e.Description,
                            e.Contents,
                            e.Image,
                            //Image = db.ProductImage.Where(pi => pi.ProductId == e.ProductId && pi.IsImageMain == true && pi.Status != (int)Const.Status.DELETED).FirstOrDefault() != null ? db.ProductImage.Where(pi => pi.ProductId == e.ProductId && pi.IsImageMain == true && pi.Status != (int)Const.Status.DELETED).FirstOrDefault().Image : null,
                            e.Url,
                            e.DateStartActive,
                            e.DateStartOn,
                            e.DateEndOn,
                            e.IsHome,
                            e.IsHot,
                            e.IsSale,
                            e.StockQuantity,
                            e.PriceSale,
                            e.PriceImport,
                            e.PriceSpecial,
                            e.PriceOther,
                            e.ManufacturerId,
                            e.ProductAttributes,
                            e.ProductNote,
                            e.NoteTech,
                            e.NotePromotion,
                            e.ViewNumber,
                            e.LikeNumber,
                            e.CommentNumber,
                            e.CompanyId,
                            e.WebsiteId,
                            e.MetaTitle,
                            e.MetaKeyword,
                            e.MetaDescription,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.UserId,
                            e.Status,
                            e.ImageLeft,
                            e.ImageRight,
                            e.TypeProduct,
                            e.ProductAge,
                            e.ProductSex,
                            e.LinkYoutube,
                            e.Width,
                            e.Height,
                            e.Discount,
                            listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.ProductId && cp.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT && cp.Status != (int)Const.Status.DELETED).Select(p => new {
                                p.CategoryId,
                                Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                Check = true
                            }).ToList(),
                            listTag = db.Tag.Where(t => t.TargetId == e.ProductId && t.Status != (int)Const.Status.DELETED).Select(p => new {
                                p.TagId,
                                p.Name,
                                Check = true
                            }).ToList(),
                            listImage = db.ProductImage.Where(pi => pi.ProductId == e.ProductId && pi.Status != (int)Const.Status.DELETED).Select(pi => new {
                                pi.ProductImageId,
                                pi.Name,
                                pi.Image,
                                pi.Location,
                                pi.IsImageMain,
                                pi.Status
                            }).ToList(),
                            listAttribute = db.ProductAttribuite.Where(pa => pa.ProductId == e.ProductId && pa.Status != (int)Const.Status.DELETED).Select(pa => new {
                                pa.ProductAttributesId,
                                pa.AttribuiteId,
                                pa.Value,
                                pa.Location,
                                Name = db.Attribuite.Where(ai => ai.AttribuiteId == pa.AttribuiteId && ai.Status != (int)Const.Status.DELETED).FirstOrDefault().Name,
                                pa.Status
                            }).ToList(),
                            trademark = db.Manufacturer.Where(c => c.ManufacturerId == e.TrademarkId && c.Status != (int)Const.Status.DELETED).Select(c => new
                            {
                                c.ManufacturerId,
                                c.Name
                            }).FirstOrDefault(),
                            TrademarkId = db.Manufacturer.Where(c => c.ManufacturerId == e.TrademarkId && c.Status != (int)Const.Status.DELETED).FirstOrDefault() != null ? e.TrademarkId : null,
                            listRelated = db.Related.Where(r => r.TargetId == e.ProductId && r.TargetType == (int)Const.TypeRelated.PRODUCT_PRODUCT && r.Status != (int)Const.Status.DELETED).Select(r => new {
                                r.TargetRelatedId
                            }).ToList(),
                            SumProductReviewInit = db.ProductReview.Where(pr => pr.ProductId == e.ProductId && pr.Status == (int)Const.Status.NORMAL).Count(),
                            SumProductReview = db.ProductReview.Where(pr => pr.ProductId == e.ProductId && pr.Status != (int)Const.Status.DELETED).Count()

                        }).ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Product data = await db.Product.FindAsync(id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id,[FromBody] ProductDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    if(data.TypeProduct == (int)Const.TypeProduct.KOI)
                    {
                        var CheckCode = db.Product.Where(p => p.Code == data.Code && p.TypeProduct == data.TypeProduct && p.ProductId != data.ProductId && p.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (CheckCode != null)
                        {
                            def.meta = new Meta(212, "Code Exist!");
                            return Ok(def);
                        }
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Product product = await db.Product.FindAsync(id);
                        product.Code = data.Code;
                        product.Name = data.Name;
                        product.Description = data.Description;
                        product.Contents = data.Contents;
                        product.IsHome = data.IsHome;
                        product.IsHot = data.IsHot;
                        product.IsSale = data.IsSale;
                        product.StockQuantity = data.StockQuantity;
                        product.PriceSale = data.PriceSale;
                        product.PriceImport = data.PriceImport;
                        product.Discount = data.Discount;
                        if(product.PriceSale != null && product.Discount != null)
                        {
                            product.PriceSpecial = product.PriceSale * (100 - product.Discount) / 100;
                        }
                        else
                        {
                            product.PriceSpecial = product.PriceSale;
                        }
                        product.PriceOther = data.PriceOther;
                        product.Image = null;
                        product.ImageLeft = data.ImageLeft;
                        product.ImageRight = data.ImageRight;
                        product.TypeProduct = data.TypeProduct;
                        product.LinkYoutube = data.LinkYoutube;
                        product.Width = data.Width;
                        product.Height = data.Height;
                        product.ProductAge = data.ProductAge;
                        product.ProductSex = data.ProductSex;
                        product.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        product.DateStartActive = data.DateStartActive != null ? data.DateStartActive : DateTime.Now;
                        product.DateStartOn = data.DateStartOn != null ? data.DateStartOn : DateTime.Now;
                        product.DateEndOn = data.DateEndOn != null ? data.DateEndOn : DateTime.Now;
                        product.ProductAttributes = data.ProductAttributes;
                        product.ProductNote = data.ProductNote;
                        product.NoteTech = data.NoteTech;
                        product.NotePromotion = data.NotePromotion;
                        product.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 0;
                        product.LikeNumber = data.LikeNumber != null ? data.LikeNumber : 0;
                        product.CommentNumber = data.CommentNumber != null ? data.CommentNumber : 0;
                        product.MetaTitle = data.MetaTitle;
                        product.MetaKeyword = data.MetaKeyword;
                        product.MetaDescription = data.MetaDescription;
                        product.TypeImagePromotionId = data.TypeImagePromotionId;
                        product.ManufacturerId = data.ManufacturerId;
                        product.TrademarkId = data.TrademarkId;

                        product.WebsiteId = data.WebsiteId;

                        product.UserId = userId;
                        product.UpdatedAt = DateTime.Now;
                        product.Status = data.Status;
                        db.Entry(product).State = EntityState.Modified;

                        //Category mapping
                        if (data.listCategory != null)
                        {
                            foreach (var item in data.listCategory)
                            {
                                CategoryMapping exist = db.CategoryMapping.Where(cm => cm.CategoryId == item.CategoryId && cm.TargetId == id && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (exist == null)
                                {
                                    if (item.Check == true)
                                    {
                                        CategoryMapping categoryNewsMapping = new CategoryMapping();
                                        categoryNewsMapping.CategoryId = item.CategoryId;
                                        categoryNewsMapping.TargetId = product.ProductId;
                                        categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT;
                                        categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                        categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                        categoryNewsMapping.CreatedAt = DateTime.Now;
                                        db.CategoryMapping.Add(categoryNewsMapping);
                                    }
                                }
                                else
                                {
                                    if (item.Check != true)
                                    {
                                        exist.Status = (int)Const.Status.DELETED;
                                        db.Entry(exist).State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        //Tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                if (item.TagId == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TargetId = product.ProductId;
                                    tag.TargetType = (int)Const.TypeTag.TAG_PRODUCT;
                                    tag.Url = Utils.NonUnicode(item.Name);
                                    tag.UserId = data.UserId;
                                    tag.CreatedAt = DateTime.Now;
                                    tag.Status = (int)Const.Status.NORMAL;
                                    db.Tag.Add(tag);
                                }
                                else
                                {
                                    Tag exist = db.Tag.Where(t => t.TagId == item.TagId && t.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        if (item.Check == false)
                                        {
                                            exist.Status = (int)Const.Status.DELETED;
                                            db.Entry(exist).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }
                        }

                        //add list Image Product
                        if (data.listImage != null)
                        {
                            int? k = data.listImage.Max(r => r.Location);
                            foreach (var item in data.listImage)
                            {
                                if(item.ProductImageId != null)
                                {
                                    var imageExist = db.ProductImage.Find(item.ProductImageId);
                                    if(imageExist != null)
                                    {
                                        if (item.Status == (int)Const.Status.DELETED)
                                        {
                                            imageExist.Status = (int)Const.Status.DELETED;
                                        }
                                        else
                                        {
                                            imageExist.IsImageMain = item.IsImageMain;
                                        }
                                    }
                                    db.Entry(imageExist).State = EntityState.Modified;
                                }
                                else
                                {
                                    ProductImage productImage = new ProductImage();
                                    productImage.Name = product.Name + "-" + k;
                                    productImage.Image = item.Image;
                                    productImage.ProductId = product.ProductId;
                                    productImage.IsImageMain = item.IsImageMain;
                                    productImage.Location = k;
                                    productImage.UserId = data.UserId;
                                    productImage.CreatedAt = DateTime.Now;
                                    productImage.UserId = userId;
                                    productImage.Status = (int)Const.Status.NORMAL;
                                    db.ProductImage.Add(productImage);
                                    k++;
                                }
                                if(item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                {
                                    product.Image = item.Image;
                                    db.Entry(product).State = EntityState.Modified;
                                }
                            }
                        }

                        //Attribute
                        if (data.listAttribute != null)
                        {
                            foreach (var item in data.listAttribute)
                            {
                                ProductAttribuite exist = db.ProductAttribuite.Where(t => t.ProductId == product.ProductId && t.AttribuiteId == item.AttribuiteId && t.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (exist != null)
                                {
                                    exist.Value = item.Value;
                                    db.Entry(exist).State = EntityState.Modified;
                                }
                                else
                                {
                                    ProductAttribuite productAttribuite = new ProductAttribuite();
                                    productAttribuite.ProductId = product.ProductId;
                                    productAttribuite.Value = item.Value;
                                    productAttribuite.AttribuiteId = item.AttribuiteId;
                                    productAttribuite.Location = item.Location;
                                    productAttribuite.CreatedAt = DateTime.Now;
                                    productAttribuite.UserId = userId;
                                    productAttribuite.Status = (int)Const.Status.NORMAL;
                                    db.ProductAttribuite.Add(productAttribuite);
                                }

                                //if (item.ProductAttributesId == null)
                                //{
                                //    if(item.Status == (int)Const.Status.NORMAL)
                                //    {
                                //        ProductAttribuite productAttribuite = new ProductAttribuite();
                                //        productAttribuite.ProductId = product.ProductId;
                                //        productAttribuite.Value = item.Value;
                                //        productAttribuite.AttribuiteId = item.AttribuiteId;
                                //        productAttribuite.Location = item.Location;

                                //        productAttribuite.CreatedAt = DateTime.Now;
                                //        productAttribuite.UserId = userId;
                                //        productAttribuite.Status = (int)Const.Status.NORMAL;
                                //        db.ProductAttribuite.Add(productAttribuite);
                                //    }
                                //}
                                //else
                                //{

                                //    ProductAttribuite exist = db.ProductAttribuite.Where(t => t.ProductAttributesId == item.ProductAttributesId && t.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                //    if (exist != null && item.Status == (int)Const.Status.DELETED)
                                //    {
                                //        exist.Status = (int)Const.Status.DELETED;
                                //        db.Entry(exist).State = EntityState.Modified;
                                //    }
                                //}
                            }
                        }

                        //Sản phẩm gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == product.ProductId && r.TargetType == (int)Const.TypeRelated.PRODUCT_PRODUCT && r.Status != (int)Const.Status.DELETED).ToList();
                        if(listRelated != null)
                        {
                            listRelated.ForEach( lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = product.ProductId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.PRODUCT_PRODUCT;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = userId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        CategoryMapping categoryMapping = db.CategoryMapping.Where(cm => cm.CategoryId == -1 && cm.TargetId == id && cm.TargetType == (int)Const.TypeOrderBy.PRODUCT_IS_HOME && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if(product.IsHome == true)
                        {
                            if(categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = data.ProductId;
                                cm.TargetType = (int)Const.TypeOrderBy.PRODUCT_IS_HOME;
                                cm.Location = 99;
                                cm.CreatedAt = DateTime.Now;
                                cm.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(cm);
                            }
                        }
                        else
                        {
                            if(categoryMapping != null)
                            {
                                categoryMapping.Status = (int)Const.Status.DELETED;
                                db.Update(categoryMapping);
                            }
                        }
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ProductId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!ProductExists(data.ProductId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    if(data.TypeProduct == (int)Const.TypeProduct.KOI)
                    {
                        var CheckCode = db.Product.Where(p => p.Code == data.Code && p.TypeProduct == data.TypeProduct && p.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (CheckCode != null)
                        {
                            def.meta = new Meta(212, "Code Exist!");
                            return Ok(def);
                        }
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Product product = new Product();
                        product.Code = data.Code;
                        product.Name = data.Name;
                        product.Description = data.Description==null ? "" : data.Description;
                        product.Contents = data.Contents;
                        product.IsHome = data.IsHome;
                        product.IsHot = data.IsHot;
                        product.IsSale = data.IsSale;
                        product.StockQuantity = data.StockQuantity;
                        product.PriceSale = data.PriceSale;
                        product.PriceImport = data.PriceImport;
                        product.Discount = data.Discount;
                        if (product.PriceSale != null && product.Discount != null)
                        {
                            product.PriceSpecial = product.PriceSale * (100 - product.Discount) / 100;
                        }
                        else
                        {
                            product.PriceSpecial = product.PriceSale;
                        }
                        product.PriceSpecial = data.PriceSpecial;
                        product.PriceOther = data.PriceOther;
                        product.Image = data.Image;
                        product.ImageLeft = data.ImageLeft;
                        product.ImageRight = data.ImageRight;
                        product.TypeProduct = data.TypeProduct;
                        product.LinkYoutube = data.LinkYoutube;
                        product.Width = data.Width;
                        product.Height = data.Height;
                        product.ProductAge = data.ProductAge;
                        product.ProductSex = data.ProductSex;
                        product.Url = data.Url != null ? data.Url : Utils.NonUnicode(data.Name);
                        product.DateStartActive = data.DateStartActive != null ? data.DateStartActive : DateTime.Now;
                        product.DateStartOn = data.DateStartOn != null ? data.DateStartOn : DateTime.Now;
                        product.DateEndOn = data.DateEndOn != null ? data.DateEndOn : DateTime.Now;
                        product.ProductAttributes = data.ProductAttributes;
                        product.ProductNote = data.ProductNote;
                        product.NoteTech = data.NoteTech;
                        product.NotePromotion = data.NotePromotion;
                        product.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 0;
                        product.LikeNumber = data.LikeNumber != null ? data.LikeNumber : 0;
                        product.CommentNumber = data.CommentNumber != null ? data.CommentNumber : 0;
                        product.MetaTitle = data.MetaTitle;
                        product.MetaKeyword = data.MetaKeyword;
                        product.MetaDescription = data.MetaDescription;
                        product.TypeImagePromotionId = data.TypeImagePromotionId;
                        product.ManufacturerId = data.ManufacturerId;
                        product.TrademarkId = data.TrademarkId;

                        //Nếu ko truyền vào website thì chọn website mạc định
                        if (data.WebsiteId == null)
                        {
                            //Nếu website mạc định = 0 thì cảnh báo tạo website
                            if (websiteId == 0)
                            {
                                def.meta = new Meta(210, "Website default is null");
                                return Ok(def);
                            }
                            else
                                data.WebsiteId = websiteId;
                        }
                        else
                        product.WebsiteId = data.WebsiteId;
                        product.CompanyId = companyId;
                        product.UserId = userId;
                        product.CreatedAt = DateTime.Now;
                        product.UpdatedAt = DateTime.Now;
                        product.Status = (int)Const.Status.NORMAL;

                        db.Product.Add(product);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.ProductId = product.ProductId;

                            if(product.IsHome == true)
                            {
                                CategoryMapping categoryMapping = new CategoryMapping();
                                categoryMapping.CategoryId = -1;
                                categoryMapping.TargetId = data.ProductId;
                                categoryMapping.TargetType = (int)Const.TypeOrderBy.PRODUCT_IS_HOME;
                                categoryMapping.Location = 99;
                                categoryMapping.CreatedAt = DateTime.Now;
                                categoryMapping.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(categoryMapping);
                            }

                            //add category mapping
                            if (data.listCategory != null)
                            {
                                foreach (var item in data.listCategory)
                                {
                                    CategoryMapping categoryNewsMapping = new CategoryMapping();
                                    categoryNewsMapping.CategoryId = item.CategoryId;
                                    categoryNewsMapping.TargetId = data.ProductId;
                                    categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT;
                                    categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                    categoryNewsMapping.CreatedAt = DateTime.Now;
                                    categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                    db.CategoryMapping.Add(categoryNewsMapping);
                                }
                            }

                            //add tag
                            if (data.listTag != null)
                            {
                                foreach (var item in data.listTag)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.Url = Utils.NonUnicode(item.Name);
                                    tag.TargetId = data.ProductId;
                                    tag.TargetType = (int)Const.TypeTag.TAG_PRODUCT;
                                    tag.WebsiteId = product.WebsiteId;
                                    tag.CompanyId = companyId;
                                    tag.UserId = data.UserId;
                                    tag.CreatedAt = DateTime.Now;
                                    tag.Status = (int)Const.Status.NORMAL;
                                    db.Tag.Add(tag);
                                }
                            }

                            //add list Image Product
                            if (data.listImage != null)
                            {
                                int k = 1;
                                foreach (var item in data.listImage)
                                {
                                    ProductImage productImage = new ProductImage();
                                    productImage.Name = product.Name + "-" + k;
                                    productImage.Image = item.Image;
                                    productImage.ProductId = product.ProductId;
                                    productImage.IsImageMain = item.IsImageMain;
                                    productImage.Location = k;
                                    productImage.UserId = data.UserId;
                                    productImage.CreatedAt = DateTime.Now;
                                    productImage.UserId = userId;
                                    productImage.Status = (int)Const.Status.NORMAL;
                                    db.ProductImage.Add(productImage);
                                    k++;

                                    if(item.IsImageMain == true)
                                    {
                                        product.Image = item.Image;
                                        db.Entry(product).State = EntityState.Modified;
                                    }
                                }
                            }

                            //add product attribuite
                            if (data.listAttribute != null)
                            {
                                //int k = 1;
                                foreach (var item in data.listAttribute)
                                {
                                    ProductAttribuite productAttribuite = new ProductAttribuite();
                                    productAttribuite.ProductId = product.ProductId;
                                    productAttribuite.Value = item.Value;
                                    productAttribuite.AttribuiteId = item.AttribuiteId;
                                    productAttribuite.Location = item.Location;
                                    productAttribuite.CreatedAt = DateTime.Now;
                                    productAttribuite.UserId = userId;
                                    productAttribuite.Status = (int)Const.Status.NORMAL;
                                    db.ProductAttribuite.Add(productAttribuite);
                                }
                            }

                            //add product related
                            if (data.listRelated != null)
                            {
                                foreach (var item in data.listRelated)
                                {
                                    Related related = new Related();
                                    related.TargetId = product.ProductId;
                                    related.TargetRelatedId = item.TargetRelatedId;
                                    related.TargetType = (int)Const.TypeRelated.PRODUCT_PRODUCT;
                                    related.Location = item.Location;
                                    related.CreatedAt = DateTime.Now;
                                    related.UserId = userId;
                                    related.Status = (int)Const.Status.NORMAL;
                                    db.Related.Add(related);
                                }
                            }

                            await db.SaveChangesAsync();
                            if (data.ProductId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);


                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (ProductExists(data.ProductId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Product data = await db.Product.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    if ((userId != data.UserId) || (companyId != data.CompanyId))
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();
                            //Xóa tag
                            var tag = db.Tag.Where(e => e.TargetId == data.ProductId
                            && e.TargetType == (int)Const.TypeTag.TAG_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in tag)
                            {
                                item.UserId = userId;
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa ảnh slide
                            var slide = db.Slide.Where(e => e.TargetId == data.ProductId
                            && e.TypeSlideId == (int)Const.TypeSlide.SLIDE_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in slide)
                            {
                                item.UserId = userId;
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa danh mục
                            var categoryMapping = db.CategoryMapping.Where(e => e.TargetId == data.ProductId
                            && e.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in categoryMapping)
                            {
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa thuộc tính
                            var productAttribute = db.ProductAttribuite.Where(e => e.ProductId == data.ProductId
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in productAttribute)
                            {
                                item.Status = (int)Const.Status.DELETED;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            //Xóa file đính kèm

                            if (data.ProductId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!ProductExists(data.ProductId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpPut("ShowHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Product data = await db.Product.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Comment.Remove(data);
                        data.Status = (byte)stt;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ProductId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!ProductExists(data.ProductId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        //API get danh sách đánh giá của sản phẩm
        [HttpGet("ProductReview/GetByPage")]
        public IActionResult GetByPageProductReview([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    //IQueryable<ProductReview> data = db.ProductReview.Where(c => c.Status != (int)Const.Status.DELETED);
                    IQueryable<ProductReviewDT> data = (from p in db.Product
                                                        join pr in db.ProductReview on p.ProductId equals pr.ProductId
                                                        where p.Status != (int)Const.Status.DELETED
                                                        && pr.Status != (int)Const.Status.DELETED
                                                        select new ProductReviewDT
                                                        {
                                                            ProductReviewId = pr.ProductReviewId,
                                                            CustomerId = pr.CustomerId,
                                                            ProductId = pr.ProductId,
                                                            ProductName = p.Name,
                                                            Contents = pr.Contents,
                                                            NumberStar = pr.NumberStar,
                                                            Status = pr.Status,
                                                            Name = pr.Name,
                                                            Email = pr.Email
                                                        }).AsQueryable();

                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    MetaDataDT metaDataDT = new MetaDataDT();
                    metaDataDT.Sum = data.Count();
                    metaDataDT.Approved = data.Where(e => e.Status == 2).Count();
                    metaDataDT.NotApproved = data.Where(e => e.Status == 3).Count();

                    def.metadata = metaDataDT;

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("ProductReviewId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("ProductReviewId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select).ToDynamicList();
                    }
                    else
                        def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        //API đổi trạng thái đánh giá sản phẩm khởi tạo - duyệt - không duyệt
        [HttpPut("ChangeStatusProductReview/{ProductReviewId}/{Stt}")]
        public async Task<ActionResult> ChangeStatusProductReview(int ProductReviewId, int Stt)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    ProductReview data = await db.ProductReview.FindAsync(ProductReviewId);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var product = await db.Product.Where(e => e.ProductId == data.ProductId).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            //db.Comment.Remove(data);
                            data.Status = (byte)Stt;
                            db.Entry(data).State = EntityState.Modified;
                            try
                            {
                                await db.SaveChangesAsync();
                                //Tính toán lại số điểm star
                                var dataReviews = db.ProductReview.Where(pr => pr.ProductId == data.ProductId && pr.Status == (int)Const.Status.OK).ToList();
                                if (dataReviews.Count() > 0)
                                {
                                    var PointStar = Math.Round((decimal)(dataReviews.Sum(e => e.NumberStar) / dataReviews.Count()), 0, MidpointRounding.AwayFromZero);
                                    product.PointStar = (int)PointStar;
                                }
                                else
                                {
                                    product.PointStar = 0;
                                }

                                db.Product.Update(product);
                                await db.SaveChangesAsync();

                                if (data.ProductReviewId > 0)
                                    transaction.Commit();
                                else
                                    transaction.Rollback();

                                def.meta = new Meta(200, "Success");
                                def.data = data;
                                return Ok(def);
                            }
                            catch (DbUpdateException e)
                            {
                                transaction.Rollback();
                                log.Error("DbUpdateException:" + e);
                                if (!ProductReviewExists(data.ProductReviewId))
                                {
                                    def.meta = new Meta(404, "Not Found");
                                    return Ok(def);
                                }
                                else
                                {
                                    def.meta = new Meta(500, "Internal Server Error");
                                    return Ok(def);
                                }
                            }
                        }
                        else
                        {
                            def.meta = new Meta(404, "Not Found");
                            return Ok(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }


        private bool ProductExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Product.Count(e => e.ProductId == id) > 0;
            }
        }

        private bool ProductReviewExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.ProductReview.Count(e => e.ProductReviewId == id) > 0;
            }
        }
    }
}


