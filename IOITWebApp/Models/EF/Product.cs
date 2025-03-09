using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public bool? IsSale { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceImport { get; set; }
        public decimal? PriceSpecial { get; set; }
        public decimal? PriceOther { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public string ProductAttributes { get; set; }
        public string ProductNote { get; set; }
        public string NoteTech { get; set; }
        public string NotePromotion { get; set; }
        public int? ViewNumber { get; set; }
        public int? LikeNumber { get; set; }
        public int? CommentNumber { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? TypeImagePromotionId { get; set; }
        public int? TrademarkId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public byte? ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string LinkYoutube { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
        public byte? TypeProduct { get; set; }
        public int? Discount { get; set; }
        public double? PointStar { get; set; }
    }
}
