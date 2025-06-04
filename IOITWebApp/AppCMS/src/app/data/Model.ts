export class Action {
  ActionId: number;
  ActionName: string;
  ActionType: string;
  TargetId: number;
  TargetType: string;
  Logs: string;
  CreatedAt: Date;
  IPAddress: string;
  Time: number;
  Type: number;
  CompanyId: number;
  UserPushId: number;
  UserId: number;
  Status: number;
}

export class Attactment {
  AttactmentId: number;
  Name: string;
  TargetId: number;
  TargetType: number;
  Url: string;
  Thumb: string;
  CreatedAt: Date;
  UserId: number;
  Status: number;
  IsImageMain: boolean;
}

export class Bank {
  BankId: number;
  Name: string;
  AccountId: string;
  AccountName: string;
  BranchName: string;
  Note: string;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Block {
  BlockId: number;
  Code: string;
  Name: string;
  Contents: string;
  Icon: string;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  IconText: any;
}

export class Category {
  CategoryId: number;
  Name: string;
  Code: string;
  CategoryParentId: number;
  Description: string;
  Contents: string;
  Url: string;
  Image: string;
  Icon: string;
  IconFa: string;
  IconText: boolean;
  Location: number;
  TypeCategoryId: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  NumberDisplayMobile: number;
}

export class CategoryMapping {
  CategoryMappingId: number;
  CategoryId: number;
  TargetId: number;
  TargetType: number;
  Location: number;
  CreatedAt: Date;
  Status: number;
}

export class CategoryRank {
  CategoryRankId: number;
  Name: string;
  Description: string;
  RankStart: number;
  RankEnd: number;
  TypeRankId: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Comment {
  CommentId: number;
  CustomerId: number;
  TargetId: number;
  TargetType: number;
  Contents: string;
  CommentParentId: number;
  CreatedAt: Date;
  UpdateAt: Date;
  Status: number;
}

export class Company {
  CompanyId: number;
  Code: string;
  Name: string;
  Email: string;
  Logo: string;
  Phone: string;
  Address: string;
  Fax: string;
  Representative: string;
  ContactName: string;
  ContactEmail: string;
  ContactPhone: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  Active: number;
  CountUser: number;

  Note: string;
}

export class Config {
  ConfigId: number;
  IsLog: number;
  EmailHost: string;
  EmailSender: string;
  EmailEnableSsl: number;
  EmailUserName: string;
  EmailDisplayName: string;
  EmailPasswordHash: string;
  EmailColorBody: string;
  EmailColorHeader: string;
  EmailColorFooter: string;
  EmailLogo: string;
  Website: string;
  Phone: string;
  EmailPort: number;
  ConpanyId: number;
  CreatedAt: Date;
  Status: number;
  IsOnePay: any;
}

export class ConfigTableItem {
  ConfigTableItemId: number;
  ConfigTableId: number;
  Code: string;
  Name: string;
  DataType: string;
  IsNull: number;
  RankMin: number;
  RankMax: number;
  Note: string;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class ConfigTable {
  ConfigTableId: number;
  Name: string;
  Code: string;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listConfigTableItem: Array<ConfigTableItem>;
}

export class ConfigThumb {
  ConfigThumbId: number;
  Width: number;
  Height: number;
  Type: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class Contact {
  ContactId: number;
  CustomerId: number;
  FullName: string;
  Title: string;
  Email: string;
  Phone: string;
  Note: string;
  TypeContactId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  TypeContact: any;
}

export class Customer {
  CustomerId: number;
  Username: string;
  Password: string;
  ConfirmPassword: string;
  FullName: string;
  Email: string;
  PhomeNumber: string;
  Avata: string;
  Sex: string;
  Birthday: any;
  Address: string;
  Note: string;
  KeyRandom: string;
  IsEmailConfirm: number;
  IsPhoneConfirm: number;
  Type: number;
  WebsiteId: number;
  CompanyId: number;
  TypeThirdId: number;
  LastLoginAt: Date;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class Department {
  DepartmentId: number;
  Code: string;
  Name: string;
  Phone: string;
  Email: string;
  CompanyId: number;
  Location: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class FunctionRole {
  FunctionRoleId: number;
  TargetId: number;
  FunctionId: number;
  ActiveKey: string;
  Type: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Function {
  FunctionId: number;
  Name: string;
  Code: string;
  FunctionParentId: number;
  Url: string;
  Note: string;
  Location: number;
  Icon: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Language {
  LanguageId: number;
  Name: string;
  Code: string;
  Flag: string;
  IsMain: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class Log {
  LogId: string;
  Action: string;
  Contents: string;
  UserId: number;
  CretatedAt: Date;
}

export class Manufacturer {
  ManufacturerId: number;
  Name: string;
  Code: string;
  TypeOriginId: number;
  Description: string;
  Logo: string;
  Address: string;
  Mobile: string;
  Phone: string;
  Fax: string;
  Website: string;
  Url: string;
  Location: number;
  MetaTitle: string;
  MetaDescription: string;
  MetaKeywords: string;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Owner: string;
  AvatarOwner: string;
  NickName: string;
  Country: string;
  Contents: string;
}

export class MenuItem {
  MenuItemId: number;
  CategoryId: number;
  MenuId: number;
  MenuParentId: number;
  Location: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Menu {
  MenuId: number;
  Name: string;
  Code: string;
  Note: string;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class News {
  NewsId: number;
  Title: string;
  Description: string;
  Contents: string;
  Image: string;
  Url: string;
  DateStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  ViewNumber: number;
  Location: number;
  IsHome: boolean;
  IsHot: boolean;
  TypeNewsId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  IsService: boolean;
  LinkVideo: string;
  Author: string;
  listCategory: Array<{ CategoryId: number, Name: string, Check: boolean }>;
  listTag: Array<{ TagId: number, Name: string, Check: boolean }>;
  listAttachment: Array<Attactment>;
  listRelated: Array<{ TargetRelatedId: number }>;
}

export class OrderItem {
  OrderItemId: number;
  OrderId: number;
  ProductId: number;
  Quantity: number;
  Price: number;
  PriceTax: number;
  PriceDiscount: number;
  PriceTotal: number;
  Status: number;
}

export class Order {
  OrderId: number;
  Code: number;
  CustomerId: number;
  PaymentStatusId: number;
  PaymentMethodId: number;
  BillingAddress: string;
  ShippingMethodId: number;
  ShippingAddress: string;
  OrderStatusId: number;
  OrderTax: number;
  OrderDiscount: number;
  OrderTotal: number;
  CustomerNote: string;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listOrderItems: Array<OrderItem>;
  ReceiverEmail: string;
  ReceiverName: string;
  ReceiverPhone: string;
  customerName?: CustomerName;
  customerAddress?: CustomerAddress;
}

export class CustomerName {
  CustomerId: number;
  FullName: string;
  PhomeNumber: string;
}

export class CustomerAddress {
  CustomerId: number;
  Name: string;
  Phone: string;
  Address: string;
}

export class Position {
  PositionId: number;
  Name: string;
  Code: string;
  LevelId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class ProductAttribuite {
  ProductAttributesId: number;
  ProductId: number;
  Name: string;
  Value: string;
  Price: number;
  AttribuiteParentId: number;
  CreatedAt: Date;
  UserId: number;
  Status: number;
}

export class ProductCustomer {
  ProductCustomerId: number;
  TargetId: number;
  TargetType: number;
  CustomerId: number;
  Location: number;
  CreatedAt: Date;
  Status: number;
}

export class ProductReview {
  ProductReviewId: number;
  CustomerId: number;
  ProductId: number;
  Contents: string;
  NumberStar: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class Product {
  ProductId: number;
  Code: string;
  Name: string;
  Description: string;
  Contents: string;
  IsHome: boolean;
  IsHot: boolean;
  IsSale: boolean;
  StockQuantity: number;
  PriceSale: number;
  Discount: number;
  PriceImport: number;
  PriceSpecial: number;
  PriceOther: number;
  Image: string;
  ImageLeft: string;
  ImageRight: string;
  TypeProduct: number;
  Width: number;
  Height: number;
  ProductAge: number;
  ProductSex: number;
  LinkYoutube: string;
  Url: string;
  DateStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  ProductAttributes: string;
  ProductNote: string;
  NoteTech: string;
  NotePromotion: string;
  ViewNumber: number;
  LikeNumber: number;
  CommentNumber: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  TypeImagePromotionId: number;
  ManufacturerId: number;
  TrademarkId: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listCategory: Array<{ CategoryId: number, Name: string, Check: boolean }>;
  listTag: Array<{ TagId: number, Name: string, Check: boolean }>;
  listAttribute: Array<Attribute>;
  listRelated: Array<{ TargetRelatedId: number }>;
  listImage: Array<ImageProduct>;
}

export class ImageProduct {
  ProductImageId: number;
  Name: string;
  Image: string;
  Location: number;
  IsImageMain: boolean;
  Status: number
}

export class Attribute {
  ProductAttributesId: number;
  AttribuiteId: number;
  Location: number;
  Name: string;
  Value: string;
  Status: number
}

export class Related {
  RelatedId: number;
  TargetId: number;
  TargetRelatedId: number;
  TargetType: number;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class Role {
  RoleId: number;
  Code: string;
  Name: string;
  Note: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserEditId: number;
  UserId: number;
  LevelRole: number;
  Status: number;
  listFunction: Array<FuncRole>;
}

export class FuncRole {
  FunctionRoleId: number;
  TargetId: number;
  FunctionId: number;
  ActiveKey: string;
}

export class Slide {
  SlideId: number;
  Name: string;
  Description: string;
  TargetId: number;
  Image: string;
  Url: string;
  TypeSlideId: number;
  IsImageMain: boolean;
  IsLinkNewTab: boolean;
  Location: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

// export interface sysdiagram {
//     diagram_id: number;
//     name: string;
//     principal_id: number;
//     version: number;
//     definition: byte[];
// }

export class Tag {
  TagId: number;
  Name: string;
  Url: string;
  TargetId: number;
  TargetType: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class TypeAttributeItem {
  TypeAttributeItemId: number;
  Code: string;
  Name: string;
  TypeAttributeId: number;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class TypeAttribute {
  TypeAttributeId: number;
  Name: string;
  IsUpdate: number;
  IsDelete: number;
  TypeAttribuiteParentId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listAttributeItem: Array<TypeAttributeItem>;
}
export class TypeAttributeItemE {
  TypeAttributeItemId: number;
  Code: string;
  Name: string;
  TypeAttributeId: number;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class UserMapping {
  UserMappingId: number;
  UserId: number;
  TargetId: number;
  TargetType: number;
  UserIdCreatedId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class UserRole {
  UserRoleId: number;
  UserId: number;
  RoleId: number;
  CreatedAt: Date;
  Status: number;
}

export class User {
  UserId: number;
  FullName: string;
  UserName: string;
  Password: string;
  Email: string;
  Code: string;
  Avata: string;
  UnitId: number;
  PositionId: number;
  DepartmentId: number;
  CompanyId: number;
  Address: string;
  Phone: string;
  KeyLock: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  TokenSince: Date;
  RegEmail: string;
  RoleMax: number;
  RoleLevel: number;
  IsRoleGroup: boolean;
  UserCreateId: number;
  UserEditId: number;
  Status: number;
  BranchId: string;
  listBranchId: [];
  Roleid: number;
}

export class Website {
  WebsiteId: number;
  Name: string;
  Url: string;
  LanguageId: number;
  CompanyId: number;
  Address: string;
  WebsiteParentId: number;
  LogoHeader: string;
  LogoFooter: string;
  Banner: string;
  Hotline: string;
  Hotmail: string;
  LinkGooglePlus: string;
  LinkFacebookPage: string;
  LinkTwitter: string;
  LinkYoutube: string;
  LinkInstagram: string;
  LinkLinkedIn: string;
  LinkOther1: string;
  LinkOther2: string;
  LinkOther3: string;
  GoogleAnalitics: string;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  HighlightsNewsId: number;
  TechNiQuePhone: string;
  GuaRanTeePhone: string;
}

export class UserChangePass {
  UserId: number;
  PasswordOld: string;
  PasswordOldE: string;
  PasswordNew: string;
  PasswordNewE: string;
  UserName: string;
  Avatar: string;
  Logo: string;
  FullName: string;
  ConfirmPassword: string;
}

export class Material {
  MaterialId: number;
  CategoryMaterialId: number;
  Levels: number;
  UnitCalculateId: number;
  StandardId: number;
  LocaltionProviderId: number;
  Location: number;
  UnitPrice: number;
  ProvinceId: number;
  Month: number;
  Year: number;
  Note: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  listMaterialImportExcelChild: Array<{
    STT: string,
    Levels: number
    Name: string
    UnitCalculate: string
    LocaltionName: string
    Standard: string,
    UnitPrice: number,
    Location: number
  }>;
}

export class SessionAution {
  SessionAutionId: number;
  Name: string;
  Code: string;
  Url: string;
  Image: string;
  IsHome: boolean;
  Description: string;
  Contents: string;
  DateStart: any;
  DateEnd: any;
  TimeStart: any;
  TimeEnd: any;
  Type: number;
  CreatedAt: string;
  UpdatedAt: string;
  UserId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  ListSessionProduct: Array<SessionPoduct>
}

export class SessionPoduct {
  SessionProductId: number;
  SessionAutionId: number;
  ProductId: number;
  CustomerWinId: number;
  PriceStart: number;
  DateTimeWin: number;
  IsHome: boolean;
  CreatedAt: string;
  UpdatedAt: string;
  UserId: number;
  Status: number;
  TypeAuction: number;
  BidPriceDistance: number;
}

export class AutionHistory {
  AutionHistoryId: number;
  SessionAutionId: number;
  CustomerId: number;
  ProductId: number;
  PriceOld: number;
  PriceNew: number;
  CreatedAt: string;
  Status: number;
}


export class Branch {
  BranchId: number;
  Code: string;
  Name: string;
  Avatar: string;
  Email: string;
  Phone: string;
  Address: string;
  Contents: string;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
  Location: number;
  Lat: string;
  Long: string;
  Username: string;
  Password: string;
  CompanyId: number;
  PMQLXe: string;
  QLCamera: string;
  TypeTram: number;
}

export class ResetPasswordCustomerDTO {
  FullName: string;
  PasswordInit: string;
  Password: string;
  ConfirmPassword: string;
  CustomerId: number;
}

export class Attribuite {
  AttribuiteId: number;
  Name: string;
  IsCustom: boolean;
  Location: number;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
}

export class ResetUser {
  id: number;
  Pass: string;
}
export class NhapKho {
  id: number;
  Pass: string;
  CAT1: number;
  CAT2: number;
  CAT3: number;
  DA1: number;
  DA2: number;
  DA3: number;
  XIMANG1: number;
  XIMANG2: number;
  XIMANG3: number;
  PHUGIA1: number;
  PHUGIA2: number;
  PHUGIA3: number;

}
export class KhachHang {
  Ma:string;
  TENKHACHHANG: string;
  SDT: string;
  DIACHI: string;
  ID:any;
  BranchId:number;
  Disable: boolean;
  listBranchId:[];CompanyId:number;
}
export class NHANVIEN {
  Ma:string;
  MATHENV: string;
  TENNV: string;
  SDT: string;
  GHICHU: string;
  ID:any;
  BranchId:number;
  Disable: boolean;
  listBranchId:[];CompanyId:number;
}
export class PHUGIA {
  Ma:string;
  TENPG: string;
  SDT: string;
  NHACUNGCAP: string;
  ID:any;
  BranchId:number;
  Disable: boolean;
  listBranchId:[];CompanyId:number;
}
export class Xe {
  Ma:string;
  BIENSO: string;
  TENLAIXE: string;
  ID:any;
  BranchId:number;
  CompanyId:number;
  listBranchId:[];
}
export class DUAN {
  Ma:string;
  TENDUAN: string;
  DIADIEMXD: string;
  TENHANGMUC: string;
  ID:any;
  BranchId:number;
  CompanyId:number;
  listBranchId:[];
}
export class DATHANG {
  ID:any;
  Ma:string;
  KHACHHANGID:number;
  NHANVIENID:number;
  DUANID:number;
  MACBETONGID:number;
  METKHOIDATHANG:any;
  NGAYDATHANG:any;
  TONGSOPHIEU:number;
  METKHOITICHLUY:any;
  TENKHACHHANG:string;
  TENNV:string;
  TENMACBETONG: string;
  TENDUAN: string;
  BranchId:number;
  CompanyId:number;
  listBranchId:[];
}
export class CAPPHOI {
  ID:any;
  MACBETONGID:any;
  TENMACBETONG:string;
  CUONGDO: string;
  COTLIEUMAX: number;
  DOSUT: string;
  CAT1:Number;
  CAT2:Number;
  CAT3:Number;
  DA1:Number;
  DA2:Number;
  DA3:Number;
  XM1:Number;
  XM2:Number;
  XM3:Number;
  XM4:Number;
  NUOC1:Number;
  NUOC2:Number;
  PHUGIA1:Number;
  PHUGIA2:Number;
  PHUGIA3:Number;
  PHUGIA4:Number;
  PHUGIA5:Number;
  PHUGIA6:Number;
  BranchId:number;
  CompanyId:number;
  listBranchId:[];
  VatLieus:any[]
}


