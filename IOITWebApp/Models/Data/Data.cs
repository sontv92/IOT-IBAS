using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOITWebApp.Models.Data
{
    public class Data
    {
        
    }

    public partial class UserInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public partial class ActionPush
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string FullName { get; set; }
        public string Avata { get; set; }
        public string TargetName { get; set; }
    }


    public partial class DownloadFile
    {
        public string Link { get; set; }

    }

    public partial class MenuItems
    {
        public int CategoryId { get; set; }
        public int MenuItemId { get; set; }
        public int MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Location { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? TypeCategoryId { get; set; }
    }

    public partial class Search
    {
        [System.ComponentModel.DefaultValue("")]
        public string sName { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int sType { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public int sCategory { get; set; }
    }

    public partial class Register
    {
        public string sName { get; set; }
        public int sType { get; set; }
    }


    public partial class CustomerLogin
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Avata { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string PhomeNumber { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public string access_token { get; set; }
        public bool? IsEmailConfirm { get; set; }
    }

    public partial class ProductDT
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceSpecial { get; set; }
        public byte? TypeProduct { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public double? Discount { get; set; }
        public double? PointStar { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    //public partial class BidKoiDT
    //{
    //    public Guid AutionHistoryId { get; set; }
    //    public string NickName { get; set; }
    //    public int SessionAutionId { get; set; }
    //    public int CustomerId { get; set; }
    //    public int ProductId { get; set; }
    //    public string ProductCode { get; set; }
    //    public byte? TypeBid { get; set; }
    //    public decimal PriceOld { get; set; }
    //    public decimal? PriceNew { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public byte? Status { get; set; }
    //}

    //public partial class NickNameDT
    //{
    //    public Guid NickNameId { get; set; }
    //    public string NickName { get; set; }
    //    public int? CustomerId { get; set; }
    //    public int? SessionAutionId { get; set; }
    //    public int? ProductId { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public byte? Status { get; set; }
    //}

    public partial class DetailRatingStar
    {
        public int? item_count { get; set; }
        public float? star { get; set; }
        public int? star1 { get; set; }
        public int? star2 { get; set; }
        public int? star3 { get; set; }
        public int? star4 { get; set; }
        public int? star5 { get; set; }
        public int? countStar1 { get; set; }
        public int? countStar2 { get; set; }
        public int? countStar3 { get; set; }
        public int? countStar4 { get; set; }
        public int? countStar5 { get; set; }
    }

    public partial class CategoryDT
    {
        public int CategoryMappingId { get; set; }
        public int? CategoryId { get; set; }
        public int? TargetId { get; set; }
        public string TargetName { get; set; }
        public int? TargetType { get; set; }
        public int? Location { get; set; }
        public int? Status { get; set; }
    }

    //public partial class SessionProductDT
    //{
    //    public int SessionProductId { get; set; }
    //    public int? SessionAutionId { get; set; }
    //    public int? ProductId { get; set; }
    //    public int? CustomerWinId { get; set; }
    //    public decimal? PriceStart { get; set; }
    //    public decimal? PriceWin { get; set; }
    //    public string NickName { get; set; }
    //    public DateTime? DateTimeWin { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public int? UserId { get; set; }
    //    public byte? Status { get; set; }
    //    public bool? IsHome { get; set; }
    //    public int? TypeAuction { get; set; }
    //    public decimal? BidPriceDistance { get; set; }
    //}

    //public partial class ResultAuctionDT
    //{
    //    public int? SessionAuctionId { get; set; }
    //    public string AuctionName { get; set; }
    //    public int? ProductId { get; set; }
    //    public string KoiName { get; set; }
    //    public string KoiImage { get; set; }
    //    public int? Typebid { get; set; }
    //    public List<TopCustomerAuctionDT> listTop { get; set; }
    //}

    //public partial class TopCustomerAuctionDT
    //{
    //    public Guid? AutionHistoryId { get; set; }
    //    public int? CustomerId { get; set; }
    //    public string CustomerName { get; set; }
    //    public string NickName { get; set; }
    //    public DateTime? Time { get; set; }
    //    public decimal? PriceBid { get; set; }
    //}

    //public partial class FollowKoiAutionDT
    //{
    //    public int SessionAutionId { get; set; }
    //    public double TimeEnd { get; set; }
    //    public List<ProductDT> ListProductKoi { get; set; }
    //}

    public partial class tableBC
    {
        public string header { get; set; }
        public string[] row { get; set; }
    }

    public partial class ProductAttribuiteDT
    {
        public int ProductAttributesId { get; set; }
        public int? ProductId { get; set; }
        public int? AttribuiteId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductReviewDT
    {
        public int ProductReviewId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Contents { get; set; }
        public int? NumberStar { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public partial class MetaDataDT
    {
        public int? Sum { get; set; }
        public int? Online { get; set; }
        public int? Offline { get; set; }
        public int? Approved { get; set; }
        public int? NotApproved { get; set; }
    }
}