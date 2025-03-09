using System.Configuration;

namespace IOITWebApp.Models
{
    public class Const
    {


        public static readonly int COMPANYID = 1;
        public static readonly int WEBSITEID = 1;
        public static string ROOT_UPLOADS  = "uploads";
        public static string ROOT_IMAGES  = "uploads/images";
        public static string ROOT_THUMBS = "uploads/thumbs";
        public static string ROOT_THUMBS_THUMB = "uploads/thumbs/_thumb";

        public static string CATEGORY_NEWS = "danh-sach-tin";
        //public static string CATEGORY_GROUP_PRODUCT = "nhom-san-pham";
        public static string CATEGORY_IMAGE = "thu-vien-anh";
        public static string CATEGORY_VIDEO = "thu-vien-video";
        public static string CATEGORY_ATTACTMENT = "van-ban";
        public static string CATEGORY_NOTIFICATION = "thong-bao";
        public static string CATEGORY_PRODUCT = "nhom-san-pham";
        public static string CATEGORY_PARTNER = "doi-tac";
        public static string PAGE_NOMAL = "trang";

        public static string DETAIL_NEWS = "chi-tiet-tin";
        public static string DETAIL_IMAGE = "chi-tiet-thu-vien-anh";
        public static string DETAIL_VIDEO = "chi-tiet-thu-vien-video";
        public static string DETAIL_ATTACTMENT = "chi-tiet-van-ban";
        public static string DETAIL_NOTIFICATION = "chi-tiet-thong-bao";
        public static string DETAIL_PRODUCT = "chi-tiet-san-pham";
        public static string DETAIL_PARTNER = "chi-tiet-doi-tac";
        public static string DETAIL_AUCTION = "phien-dau-gia";

        public static string PAGE_AUCTION = "dau-gia";
        public static string PAGE_AUCTION_LIST = "danh-sach-koi-dau-gia";
        public static string PAGE_AUCTION_TOP = "top-tra-gia-cao-nhat";


        public static string TAG_NEWS = "tag-tin";
        public static string TAG_PRODUCT = "tag-san-pham";

        public static readonly string PRICE_CONTACT = "Giá: Liên hệ";

        public enum Status
        {
            NORMAL = 1,
            OK = 2,
            NOT_OK = 3,
            TEMP = 10,
            LOCK = 98,
            DELETED = 99,
        }

        public enum Action
        {
            VIEW = 0,
            CREATE = 1,
            UPDATE = 2,
            DELETED = 3,
            IMPORT = 4,
            EXPORT = 5,
            PRINT = 6,
            EDIT_ANOTHER_USER = 7,
            MENU = 8
        }

        public enum TypeAttribute    // Thuốc tính loại hình
        {
            THTDA = 1, // Thuộc tính dự án
            LHDA = 2,   // Loại hình dự án
            LDA = 3, // Loại dự án
            TTDA = 4, // Tình trạng dự án
            NDA = 5, // Nhóm dự án
            CDA = 6, // Cấp dự án
            VDA = 7, // Vùng dự án
            HTQLDA = 8, // Hình thức QLDA
            TTGT = 9, // Tình trạng gói thầu
            LGT = 10, // Loại gói thầu
            LHD = 11, // Loại hợp đồng
            HTHD = 12, // Hình thức hợp đồng
        }

        public enum TypeCategory    // loại danh mục
        {
            CATEGORY_NEWS_TEXT = 1, // Danh mục tin văn bản
            CATEGORY_NEWS_NOTIFICATION = 2, // danh mục tin thông báo
            CATEGORY_NEWS_IMAGE = 3, // danh mục tin hình ảnh
            CATEGORY_NEWS_VIDEO = 4, // danh mục tin video
            CATEGORY_NEWS_ATTACTMENT = 5, // danh mục tệp đính kèm
            //
            CATEGORY_PAGE_NORMAL = 6,    // danh mục trang bình thường
            CATEGORY_PAGE_LINK_DO_NORMAL = 7,    // danh mục trang liên kết dofollow on tab
            CATEGORY_PAGE_LINK_DO_BLALK = 8,    // danh mục trang liên kết dofollow new tab
            CATEGORY_PAGE_LINK_NO_NORMAL = 9,    // danh mục trang liên kết nofollow on tab
            CATEGORY_PAGE_LINK_NO_BLALK = 10,    // danh mục trang liên kết nofollow new tab
            //
            CATEGORY_PRODUCT = 11,   // danh mục sản phẩm
        }

        public enum TypeNews    // loại tin tức
        {
            NEWS_TEXT = 1, // tin văn bản
            NEWS_NOTIFICATION = 2,    // tin thông báo
            NEWS_IMAGE = 3,   // tin hình ảnh
            NEWS_VIDEO = 4, // tin video
            NEWS_ATTACTMENT = 5 // tin tệp đính kèm
        }

        public enum TypeFunction    // Phân quyền chức năng với người dùng và nhóm quyền
        {
            FUNCTION_USER = 1, // Người dùng - Chức năng
            FUNCTION_ROLE = 2,    // Nhóm quyền - Chức năng
        }

        public enum TypeFolder    // loại folder
        {
            CATEDOGY = 1, // danh mục + trang
            NEWS = 2,    // tin tức
            PRODUCT = 3,   // sản phẩm
            SLIDE = 3,   // trình chiếu
        }

        public enum TypeThumb    // loại thumb
        {
            CATEDOGY = 5, // danh mục + trang
            NEWS = 1,    // tin tức
            PRODUCT = 2,   // sản phẩm
            SLIDE = 3,   // slide
            ICON = 4,   // icon
            OTHER = 6,   // icon
        }

        public enum TypeAction    // hành động
        {
            ACTION = 1, // Thông báo
            AUTION = 1, // Thông báo
            WARNING = 2,    // Cảnh báo
        }

        public enum ActionType    // hành động
        {
            CREATE = 1, // Thêm
            UPDATE = 2,    // Sửa
            DELETE = 3,    // Xóa
            WARNING = 4,    // Cảnh báo
        }

        public enum TypeFile    // loại file
        {
            DOCUMENTS = 1, // file văn bản
            VIDEO = 2,    // file video
            AUDIO = 3,    // file âm thanh
            ELECTRONIC_BOOKS = 4,    // file sách điện tử
            IMAGES = 5,    // file hình ảnh
            ARCHIVES = 6,    // file nén
        }

        public enum PaymentStatus    // trang thái thanh toán
        {
            INIT = 1, // chưa thanh toán
            FULL = 2,    // đã thanh toán hết
            NOT_ENOUGH = 3,    // chưa thanh toán hết
            NOT_PAYMENT = 4,     // không thanh toán
            ERROR_PAYMENT = 5     // thanh toán lỗi
        }

        public enum PaymentMethod    // trang thái thanh toán
        {
            NOTPAY = 108, // ko thanh toán
            COD = 100, // ko thanh toán
            VIETELPAY = 99, // ko thanh toán
            PAYPAL = 91,    // paypal
            WECHATPAY = 90,    // wechat pay
            ALIPAY = 89,     // alipay
            ONEPAY_OUT = 88,     // onepay
            ONEPAY_IN = 87,     // onepay
            MOMO = 86,     // momo
        }

        public enum ShippingStatus // trang thái giao hàng
        {
            NOT_SHIP = 0, //chon van chuyen bang nha cung cap nhung chua tao shiporder
            INIT = 1, // da khoi tao thanh cong nhung chua tiep nhan
            DELIVERING = 4, //2 Da lay hang va dang van chuyen 
            ORDER_RETURN = 3,// khong giao duoc hang
            CANCELED = 5,//4 Huy giao hang 
            COMPLETED = 2,//5 Da hoan thanh
            LOST = 6, // khong lay duoc hang
            DELIVERER = 7, // da tiep nhan , dang dieu phoi lay hang
            DELETE = 99
        }

        public enum ShippingMethod // trang thái giao hàng
        {
            SHOP_SHIP = 1,
            GHTK = 2,
            GHN = 3,
            GHVT = 4,
            GHVN = 5,
            OTHER = 88,
        }

        public enum AcceptCash    // trang thái duyệt phiếu
        {
            INIT = 1, // chưa duyệt
            ACCEPT = 2,    // đã duyệt
            NOT_ACCEPT = 3,    // không duyệt
        }

        public enum ContractStatus    // trang thái duyệt phiếu
        {
            INIT = 49, // Chuẩn bị triển khai
            DEPLOYING = 50,    // Đang triển khai
            ACCEPTED = 51,    // Đã nghiệm thu
            PAYMENTED = 52,    // Đã quyết toán
            PAUSE = 53,    // Tạm dừng
        }

        public enum TypePlatform    // loại nền tảng
        {
            WEB = 1,    // 
            ANDROID = 2, // 
            IOS = 23, // 
        }

        public enum RoleLevel  //Loại quyền
        {
            ADMIN = 1,   //admin (chỉ có 1 tài khoản duy nhất và ng cài đặt web lắm giữ)
            MANAGER = 3,   //manager quản trị trang web
            USER = 2,   //user sử dụng trang web
        }

        //public enum LanguageNumber  //Số lượng ngôn ngữ
        //{
        //    LANGUAGE_ZEZO = -2,   //
        //    LANGUAGE_ONE = -1,   //
        //    LANGUAGE_MULTI = 0,   //
        //}

        public enum TypeTag  //Loại tag
        {
            TAG_NEWS = 1,   //
            TAG_PRODUCT = 2,   //
        }

        public enum TypeCategoryMapping  //Loại tag
        {
            CATEGORY_NEWS = 1,   //
            CATEGORY_PRODUCT = 2,   //
            CATEGORY_SESSION = 3
        }

        public enum TypeRelated  //Loại liên quan
        {
            PRODUCT_PRODUCT = 1,   //
            PRODUCT_NEWS = 2,   //
            NEWS_NEWS = 3,   //
            NEWS_PRODUCT = 4,   //
        }

        public enum TypeSlide  //Loại slide
        {
            SLIDE_HOME = 1,   //
            SLIDE_PRODUCT = 2,   //
            SLIDE_PATNER = 3,   //
            SLIDE_ADS = 4,   //
        }

        public enum TypeOrigin  //Loại bảng Manufacture
        {
            MANUFACTURER = 1,   // Nhà sản xuất(đối tác)
            TRADEMARK = 2,   // Loại cái Koi
            PARTNER = 3     // Trại cá
        }

        public enum TypeUpload  //Loại upload image
        {
            UPLOAD_IMAGE_NEWS = 1,   //
            UPLOAD_IMAGE_PRODUCT = 2,   //
            UPLOAD_IMAGE_SLIDE = 3,   //
            UPLOAD_IMAGE_ICON = 4,   //
            UPLOAD_IMAGE_CATEGORY = 5,   //
            UPLOAD_IMAGE_OTHER = 6,   //
        }

        public enum TypeProduct  //Loại sản phẩm
        {
            NORMAL = 1,   //Sản phẩm bình thường
            KOI = 2   //Cá Koi
        }

        public enum TypeKoiSex  //Giới tính koi
        {
            MALE = 1,   //Giống đực
            FEMALE = 2,   //Giống cái
            BISEXUAL = 3    //Lưỡng tính
        }

        public enum TypeAuction
        {
            AUCTION_SILENT = 1,
            AUCTION_BT = 2,
            AUCTION_ONLINE = 3
        }

        public enum TypeAttachment
        {
            NEWS_IMAGE = 1,
            NEWS_VIDEO = 2
        }

        public enum OrderStatus
        {
            INIT = 1,   // Trạng thái khởi tạo / chờ xác nhận đơn
            CONFIRM = 2,    //Đơn hàng đã được xác nhận
            DELIVERY = 3,   // Trạng thái đang giao hàng
            DELIVED = 4,    // Trạng thái đã giao hàng
            ORDER_RETURNED = 5  // Đơn hàng bị trả lại - Trạng thái HỦY
        }

        public enum TypeProductCustomer
        {
            FOLLOW = 1,   // Theo dõi
            LOVE = 2    // Thích
        }

        public enum TypeDevice
        {
            DESKTOP = 0,    // Máy tính
            MOBILE = 2   // Di động
        }

        public enum TypeOrderBy //Các loại sắp xếp
        {
            PRODUCT_IS_HOME = 10,    //Sắp xếp sản phẩm hiển thị ra trang chủ
            NEWS_IS_HOME = 11,   //Sắp xếp hiển thị tin tức ra trang chủ
            SESSION_AUCTION_IS_HOME = 12    //Sắp xếp tin đấu giá ra trang chủ
        }

        public enum TypeOrderByCategoryProduct //Các loại sắp xếp trong trang danh mục sản phẩm
        {
            DEFAULT = -1,    // Mặc định
            A_Z = 1,   // A=>Z
            Z_A = 2,    // Z=>A
            PRICE_INCREASE = 3,      // Giá từ thấp tới cao
            PRICE_REDUCTION = 4     // Giá từ cao tới thấp
        }

        public enum TypeRank
        {
            S = 1,  // Diện tích
            K_G = 2 // Khoảng giá
        }
    }
}