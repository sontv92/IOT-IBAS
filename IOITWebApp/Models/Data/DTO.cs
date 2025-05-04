using IOITWebApp.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOITWebApp.Models.Data
{
    public class DTO
    {
    }


    public partial class UserLogin
    {
        public Nullable<int> userId { get; set; }
        public Nullable<int> companyId { get; set; }
        public Nullable<int> languageId { get; set; }
        public Nullable<int> websiteId { get; set; }
        public string logoWebsite { get; set; }
        public string fullName { get; set; }
        public string avata { get; set; }
        public string Logo { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> roleMax { get; set; }
        public Nullable<byte> roleLevel { get; set; }
        public bool isRoleGroup { get; set; }
        public string access_token { get; set; }
        public string access_key { get; set; }
        public string BranchId { get; set; }
        public string PMQLXe { get; set; }
        public string QLCamera { get; set; }
        public List<MenuDTO> listMenus { get; set; }
        public List<Role> listRoles { get; set; }
    }

    public partial class UserPartnerLogin
    {
        public Nullable<int> userId { get; set; }
        public string fullName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public bool isRoleGroup { get; set; }
        public Nullable<int> status { get; set; }
        public string access_token { get; set; }
        public string access_key { get; set; }
    }

    //public partial class UserDTO
    //{
    //    public Nullable<int> UserId { get; set; }
    //    public Nullable<int> BranchId { get; set; }
    //    public string Code { get; set; }
    //    public string FullName { get; set; }
    //    public string Password { get; set; }
    //    public string Address { get; set; }
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public Nullable<int> Status { get; set; }
    //    public Nullable<int> GroupId { get; set; }
    //    public string Group { get; set; }
    //    public Nullable<DateTime> CreatedAt { get; set; }
    //    public Nullable<DateTime> UpdatedAt { get; set; }
    //    public Nullable<DateTime> TokenSince { get; set; }
    //    public string FacebookUserId { get; set; }
    //    public string RegEmail { get; set; }
    //}

    public partial class MenuDTO
    {
        public int MenuId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MenuParent { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string ActiveKey { get; set; }
        public int? Status { get; set; }
        public List<MenuDTO> listMenus { get; set; }
    }
    public partial class DatHangDTO
    {
        public string Ma { get; set; }
        public Guid ID { get; set; }
        public Guid KHACHHANGID { get; set; }
        public Guid NHANVIENID { get; set; }
        public Guid DUANID { get; set; }
        public Guid MACBETONGID { get; set; }
        public int TONGSOPHIEU { get; set; }

        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string TENMACBETONG { get; set; }
        public double METKHOIDATHANG { get; set; }
        public double METKHOITICHLUY { get; set; }
        public DateTime NGAYDATHANG { get; set; }

        public string NGAYDATHANGTITLE { get; set; }
        public string TENNV { get; set; }
        public int BranchId { get; set; }

        public string MAKHACHHANG { get; set; }
        public string MADUAN { get; set; }
        public double METKHOITICHLUY_TEMP { get; set; }

        public string MAMACBETONG { get; set; }
        public DateTime LASTUPDATE { get; set; }


    }
    public partial class BienSoDTO
    {
        public string Name { get; set; }


    }

    public partial class XeDTO
    {
        public string BIENSO { get; set; }
        public string TENLAIXE { get; set; }
        public string Ma { get; set; }
        public Guid ID { get; set; }

        public int BranchId { get; set; }

        public bool ISSYNC { get; set; }

        public bool SYSCCHENGE { get; set; }
        public DateTime LASTUPDATE { get; set; }


    }
    public partial class HangMucDTO
    {
        public string TENHANGMUC { get; set; }
    }

    public partial class DuAnDTO
    {
        public Guid ID { get; set; }
        public string Ma { get; set; }
        public string MALIENKET { get; set; }
        public string TENDUAN { get; set; }
        public string DIADIEMXD { get; set; }

        public string TENHANGMUC { get; set; }

        public bool ISSYNC { get; set; }

        public bool SYSCCHENGE { get; set; }

        public int BranchId { get; set; }
        public Guid DUANID { get; set; }
        public DateTime LASTUPDATE { get; set; }
    }
    public partial class NhanVienDTO
    {
        public Guid ID { get; set; }
        public string Ma { get; set; }
        public string MALIENKET { get; set; }
        public string MATHENV { get; set; }
        public string TENNV { get; set; }

        public string SDT { get; set; }
        public string GHICHU { get; set; }

        public int BranchId { get; set; }

        public Guid NHANVIENID { get; set; }
        public DateTime LASTUPDATE { get; set; }
    }
    public partial class PhuGiaDTO
    {
        public Guid ID { get; set; }
        public string Ma { get; set; }

        public string TENPG { get; set; }
        public string NHACUNGCAP { get; set; }

        public bool ISSYNC { get; set; }

        public bool SYSCCHENGE { get; set; }

        public int BranchId { get; set; }
        public DateTime LASTUPDATE { get; set; }
    }
    public partial class KhachHangDTO
    {
        public string MALIENKET { get; set; }
        public string TENKHACHHANG { get; set; }
        public string SDT { get; set; }
        public string DIACHI { get; set; }
        public string STT { get; set; }

        public string Ma { get; set; }
        public Guid ID { get; set; }
        public Guid KHACHHANGID { get; set; }
        public int BranchId { get; set; }

        public bool ISSYNC { get; set; }

        public bool SYSCCHENGE { get; set; }
        public DateTime LASTUPDATE { get; set; }


    }
    public partial class CapPhoiDTO
    {
        public Guid ID { get; set; }

        public Guid MACBETONGID { get; set; }
        public string Ma { get; set; }
        public string MALIENKET { get; set; }
        public string TENMACBETONG { get; set; }
        public bool ISSYNC { get; set; }
        public List<VatLieuDTO> VatLieus { get; set; }
        public Double CAT1 { get; set; }
        public Double CAT2 { get; set; }
        public Double CAT3 { get; set; }
        public Double DA1 { get; set; }
        public Double DA2 { get; set; }
        public Double DA3 { get; set; }
        public Double XM1 { get; set; }
        public Double XM2 { get; set; }
        public Double XM3 { get; set; }
        public Double XM4 { get; set; }
        public Double NUOC1 { get; set; }
        public Double NUOC2 { get; set; }
        public Double PHUGIA1 { get; set; }
        public Double PHUGIA2 { get; set; }
        public Double PHUGIA3 { get; set; }
        public Double PHUGIA4 { get; set; }
        public Double PHUGIA5 { get; set; }
        public Double PHUGIA6 { get; set; }
        public string CUONGDO { get; set; }
        public int COTLIEUMAX { get; set; }
        public string DOSUT { get; set; }

        public int BranchId { get; set; }
        public DateTime LASTUPDATE { get; set; }

    }

    public partial class ThongKeDonHangTongHopDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }
    public partial class ThongKeXeDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }
    public partial class ThongKeDonHangChiTietDTO
    {
        public string MAPHIEU { get; set; }
        public int STTMETRON { get; set; }
        public string NGAYTRON { get; set; }
        public string GIOBATDAU { get; set; }
        public string GIOKETTHUC { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string NHANVIENKD { get; set; }
        public string MACBT { get; set; }
        public string CHEDO { get; set; }
        public double THETICH { get; set; }
        public double CP_CAT1 { get; set; }
        public double TD_CAT1 { get; set; }
        public double T_CAT1 { get; set; }
        public double PHANTRAM_CAT1 { get; set; }

        public double SS_CAT1 { get; set; }
        public double SS_CAT2 { get; set; }
        public double SS_CAT3 { get; set; }
        public double SS_DA1 { get; set; }
        public double SS_DA2 { get; set; }
        public double SS_DA3 { get; set; }

        public double SS_XM1 { get; set; }
        public double SS_XM2 { get; set; }
        public double SS_XM3 { get; set; }
        public double SS_XM4 { get; set; }

        public double SS_NC1 { get; set; }
        public double SS_NC2 { get; set; }
        public double SS_PG1 { get; set; }
        public double SS_PG2 { get; set; }
        public double SS_PG3 { get; set; }
        public double SS_PG4 { get; set; }
        public double SS_PG5 { get; set; }
        public double SS_PG6 { get; set; }


        public double CP_CAT2 { get; set; }
        public double TD_CAT2 { get; set; }
        public double T_CAT2 { get; set; }
        public double PHANTRAM_CAT2 { get; set; }

        public double CP_CAT3 { get; set; }
        public double TD_CAT3 { get; set; }
        public double T_CAT3 { get; set; }
        public double PHANTRAM_CAT3 { get; set; }

        public double CP_DA1 { get; set; }
        public double TD_DA1 { get; set; }
        public double T_DA1 { get; set; }
        public double PHANTRAM_DA1 { get; set; }

        public double CP_DA2 { get; set; }
        public double TD_DA2 { get; set; }
        public double T_DA2 { get; set; }
        public double PHANTRAM_DA2 { get; set; }

        public double CP_DA3 { get; set; }
        public double TD_DA3 { get; set; }
        public double T_DA3 { get; set; }
        public double PHANTRAM_DA3 { get; set; }

        public double CP_XM1 { get; set; }
        public double TD_XM1 { get; set; }
        public double T_XM1 { get; set; }
        public double PHANTRAM_XM1 { get; set; }

        public double CP_XM2 { get; set; }
        public double TD_XM2 { get; set; }
        public double T_XM2 { get; set; }
        public double PHANTRAM_XM2 { get; set; }

        public double CP_XM3 { get; set; }
        public double TD_XM3 { get; set; }
        public double T_XM3 { get; set; }
        public double PHANTRAM_XM3 { get; set; }

        public double CP_XM4 { get; set; }
        public double TD_XM4 { get; set; }
        public double T_XM4 { get; set; }
        public double PHANTRAM_XM4 { get; set; }

        public double CP_NC1 { get; set; }
        public double TD_NC1 { get; set; }
        public double T_NC1 { get; set; }
        public double PHANTRAM_NC1 { get; set; }

        public double CP_NC2 { get; set; }
        public double TD_NC2 { get; set; }
        public double T_NC2 { get; set; }
        public double PHANTRAM_NC2 { get; set; }

        public double CP_PG1 { get; set; }
        public double TD_PG1 { get; set; }
        public double T_PG1 { get; set; }
        public double PHANTRAM_PG1 { get; set; }

        public double CP_PG2 { get; set; }
        public double TD_PG2 { get; set; }
        public double T_PG2 { get; set; }
        public double PHANTRAM_PG2 { get; set; }

        public double CP_PG3 { get; set; }
        public double TD_PG3 { get; set; }
        public double T_PG3 { get; set; }
        public double PHANTRAM_PG3 { get; set; }

        public double CP_PG4 { get; set; }
        public double TD_PG4 { get; set; }
        public double T_PG4 { get; set; }
        public double PHANTRAM_PG4 { get; set; }


        public double CP_PG5 { get; set; }
        public double TD_PG5 { get; set; }
        public double T_PG5 { get; set; }
        public double PHANTRAM_PG5 { get; set; }

        public double CP_PG6 { get; set; }
        public double TD_PG6 { get; set; }
        public double T_PG6 { get; set; }
        public double PHANTRAM_PG6 { get; set; }

    }

    public partial class ThongKeChiTietVatTuDTO
    {
        public int STT { get; set; }
        public string MAPHIEU { get; set; }
        public DateTime NGAYTRON { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENMACBETONG { get; set; }
        //public string CHEDO { get; set; }
        public double M3METRON { get; set; }

        public double ColumnVL1 { get; set; }
        public double ColumnVL2 { get; set; }
        public double ColumnVL3 { get; set; }
        public double ColumnVL4 { get; set; }
        public double ColumnVL5 { get; set; }
        public double ColumnVL6 { get; set; }
        public double ColumnVL7 { get; set; }
        public double ColumnVL8 { get; set; }
        public double ColumnVL9 { get; set; }
        public double ColumnVL10 { get; set; }
        public double ColumnVL11 { get; set; }
        public double ColumnVL12 { get; set; }
        public double ColumnVL13 { get; set; }
        public double ColumnVL14 { get; set; }
        public double ColumnVL15 { get; set; }
        public double ColumnVL16 { get; set; }
        public double ColumnVL17 { get; set; }
        public double ColumnVL18 { get; set; }

    }

    public partial class ThongKeChiTietVatTuGridDTO
    {
        public int STT { get; set; }
        public string MAPHIEU { get; set; }
        public DateTime NGAYTRON { get; set; }
        public string TENKHACHHANG { get; set; }
        public string DIACHI { get; set; }
        public string HANGMUC { get; set; }
        public string SOXE { get; set; }
        public string TAIXE { get; set; }
        public string TENMACBETONG { get; set; }
        //public string CHEDO { get; set; }
        public double M3METRON { get; set; }

        public List<double> CoulumnVL { get; set; }

        //public double ColumnVL1 { get; set; }
        //public double ColumnVL2 { get; set; }
        //public double ColumnVL3 { get; set; }
        //public double ColumnVL4 { get; set; }
        //public double ColumnVL5 { get; set; }
        //public double ColumnVL6 { get; set; }
        //public double ColumnVL7 { get; set; }
        //public double ColumnVL8 { get; set; }
        //public double ColumnVL9 { get; set; }
        //public double ColumnVL10 { get; set; }
        //public double ColumnVL11 { get; set; }
        //public double ColumnVL12 { get; set; }
        //public double ColumnVL13 { get; set; }
        //public double ColumnVL14 { get; set; }
        //public double ColumnVL15 { get; set; }
        //public double ColumnVL16 { get; set; }
        //public double ColumnVL17 { get; set; }
        //public double ColumnVL18 { get; set; }

    }


    public partial class ThongKeChiTietVatTuTongHopDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }



    public partial class MacBeTongDTO
    {

        public Guid ID { get; set; }
        public string Ma { get; set; }
        public string TENMACBETONG { get; set; }
        public int CUONGDO { get; set; }
        public int COTLIEUMAX { get; set; }
        public string DOSUT { get; set; }
        public int MAMACBETONGSYNC { get; set; }

        public bool ISSYNC { get; set; }

        public bool SYSCCHENGE { get; set; }

        public int THOIGIANMAC { get; set; }
        public Guid MACBETONGID { get; set; }

        public DateTime LASTUPDATE { get; set; }
        public string MaLK { get; set; }
        public string GhiChu { get; set; }
        public string DonViQuyDoi { get; set; }
    }
    public partial class SoLuongVLDTO
    {

        public Guid MACBETONGID { get; set; }
        public Guid ID { get; set; }
        public float SOLUONG { get; set; }
        public string Ma { get; set; }
        public int MACUAVL { get; set; }
        public string MAMAC { get; set; }

        public DateTime LASTUPDATE { get; set; }


    }



    public partial class VatLieuDTO
    {
        public long STT { get; set; }
        public string TENCUAVL { get; set; }
        public Boolean COPHAIPHUGIA { get; set; }
        public string TENLOAIVL { get; set; }
        public int MACUAVL { get; set; }
        public decimal? VALUE { get; set; }
        public Boolean TRANGTHAI { get; set; }


    }

    public partial class MaLienKetDTO
    {
        public string MALIENKET { get; set; }
    }



    public partial class FunctionDTO
    {
        public int FunctionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Note { get; set; }
        public int FunctionParent { get; set; }
        public int? Location { get; set; }
        public string Icon { get; set; }
        public bool? IsMenu { get; set; }
        public int? Status { get; set; }
        public List<FunctionDTO> functionParent { get; set; }
    }

    public partial class FunctionDT
    {
        public int id { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public int? location { get; set; }
        public bool? selected { get; set; }
        public bool? is_max { get; set; }
        public List<FunctionDT> children { get; set; }
    }

    public partial class FunctionRoleDTO
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
        public int? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }
        public bool? Selected { get; set; }
        public List<FunctionRoleDTO> functionRole { get; set; }
    }

    public partial class FunctionRoleDT
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
    }
    public partial class UserResetDT
    {
        public int id { get; set; }
        public string Pass { get; set; }
    }

    public partial class UserRoleDT
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Avata { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public int? UnitId { get; set; }
        public int? CompanyId { get; set; }
        public string BranchId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? UserCreateId { get; set; }
        public int? ProjectId { get; set; }
        public int? Roleid { get; set; }
        public bool? IsRoleGroup { get; set; }
        public List<RoleDT> listRole { get; set; }
        public List<FunctionRoleDT> listFunction { get; set; }
        //public List<UserProjectDTO> listUnit { get; set; }
        //public List<UserProjectDTO> listProject { get; set; }
    }

    public partial class UserChangePass
    {
        public int UserId { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
        public string UserName { get; set; }
    }

    public partial class RoleDT
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public partial class RoleDTO
    {
        public int RoleId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UserId { get; set; }

        public int? UserEditId { get; set; }

        public int? Status { get; set; }

        public List<FunctionRoleDT> listFunction { get; set; }
    }


    public partial class SmallFunctionDTO
    {
        public int FunctionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Level { get; set; }
    }

    public partial class SmallCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Genealogy { get; set; }    //Gia phả của danh mục này
        public int CategoryParentId { get; set; }
        public byte? Status { get; set; }
        public int? Level { get; set; }
        public bool? Check { get; set; }
    }

    public partial class AttactmentDTO
    {
        public int? AttactmentId { get; set; }
        public string Name { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Url { get; set; }
        public string Thumb { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public bool? IsImageMain { get; set; }
    }

    public partial class BankDTO
    {
        public int BankId { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string BranchName { get; set; }
        public string Note { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class BlockDTO
    {
        public int BlockId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Icon { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? CategoryParentId { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string IconFa { get; set; }

        public bool? IconText { get; set; }
        public int? Location { get; set; }
        public int? TypeCategoryId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public int? NumberDisplayMobile { get; set; }
    }

    public partial class CategoryMappingDTO
    {
        public int CategoryMappingId { get; set; }
        public int? CategoryId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetType { get; set; }
        public int? Location { get; set; }
        public int? LanguageId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CategoryRankDTO
    {
        public int CategoryRankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? RankStart { get; set; }
        public int? RankEnd { get; set; }
        public int? TypeRankId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CommentDTO
    {
        public int CommentId { get; set; }
        public int? CustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Contents { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public string Representative { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigDTO
    {
        public int ConfigId { get; set; }
        public bool? IsLog { get; set; }
        public string EmailHost { get; set; }
        public string EmailSender { get; set; }
        public bool? EmailEnableSSL { get; set; }
        public string EmailUserName { get; set; }
        public string EmailDisplayName { get; set; }
        public string EmailPasswordHash { get; set; }
        public int? EmailPort { get; set; }
        public int? ConpanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigTableItemDTO
    {
        public int ConfigTableItemId { get; set; }
        public int? ConfigTableId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool? IsNull { get; set; }
        public int? RankMin { get; set; }
        public int? RankMax { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigTableDTO
    {
        public int ConfigTableId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public List<ConfigTableItemDTO> listConfigTableItem { get; set; }

    }

    public partial class ConfigThumbDTO
    {
        public int ConfigThumbId { get; set; }
        public int? Width { get; set; }
        public int? Hieght { get; set; }
        public byte? Type { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ContactDTO
    {
        public int ContactId { get; set; }
        public int? CustomerId { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public int? TypeContactId { get; set; }
        public int? TypeContact { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhomeNumber { get; set; }
        public string Avata { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string KeyRandom { get; set; }
        public bool? IsEmailConfirm { get; set; }
        public bool? IsSentEmailConfirm { get; set; }
        public bool? IsPhoneConfirm { get; set; }
        public int? Type { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? TypeThirdId { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? CompanyId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }


    public partial class LanguageDTO
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
        public bool? IsMain { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ManufacturerDTO
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Url { get; set; }
        public int? Location { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuItemDTO
    {
        public int MenuItemId { get; set; }
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuOutDTO
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class NewsDTO
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public int? ViewNumber { get; set; }
        public int? Location { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public int? TypeNewsId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public bool? IsService { get; set; }
        public string LinkVideo { get; set; }
        public List<CateOfNews> listCategory { get; set; }
        public List<TagOfNews> listTag { get; set; }
        public List<AttactmentDTO> listAttachment { get; set; }
        public List<RelatedDTO> listRelated { get; set; }
    }

    public partial class CateOfNews
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public bool? Check { get; set; }
    }

    public partial class TagOfNews
    {
        public int? TagId { get; set; }
        public string Name { get; set; }
        public bool? Check { get; set; }
    }

    public partial class OrderItemDTO
    {
        public int? OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductUrl { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceTax { get; set; }
        public decimal? PriceDiscount { get; set; }
        public decimal? PriceTotal { get; set; }
        public byte? Status { get; set; }
    }

    public partial class OrderDTO
    {
        public int OrderId { get; set; }
        public int? NumberOrder { get; set; }
        public int CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string BillingAddress { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public int OrderStatusId { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderTotal { get; set; }
        public string CustomerNote { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class PositionDTO
    {
        public int PositionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? LevelId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductAttribuiteDTO
    {
        public int? ProductAttributesId { get; set; }
        public int? ProductId { get; set; }
        public int? AttribuiteId { get; set; }
        public int? Location { get; set; }
        //public string Name { get; set; }
        public string Value { get; set; }
        //public decimal? Price { get; set; }
        //public int AttribuiteParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        //public List<ProductAttribuiteDTO> listChild { get; set; }
    }

    public partial class ProductCustomerDTO
    {
        public int ProductCustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? CustomerId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductReviewDTO
    {
        public int? ProductReviewId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public string Contents { get; set; }
        public int? NumberStar { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

    }

    public partial class ProductDTO
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
        public int? Discount { get; set; }
        public string Image { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
        public string LinkYoutube { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public byte? TypeProduct { get; set; }
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
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public List<CateOfNews> listCategory { get; set; }
        public List<TagOfNews> listTag { get; set; }
        public List<ImageProductDTO> listImage { get; set; }
        public List<ProductAttribuiteDTO> listAttribute { get; set; }
        public List<RelatedDTO> listRelated { get; set; }
    }

    public partial class ImageProductDTO
    {
        public int? ProductImageId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public byte? Status { get; set; }
    }

    public partial class RelatedDTO
    {
        public int RelatedId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetRelatedId { get; set; }
        public byte? TargetType { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class SlideDTO
    {
        public int SlideId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int? TypeSlideId { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class TagDTO
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class TypeAttributeItemDTO
    {
        public int? TypeAttributeItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int TypeAttributeId { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class TypeAttributeDTO
    {
        public int TypeAttributeId { get; set; }
        public string Name { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public int? TypeAttribuiteParentId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public List<TypeAttributeItemDTO> listAttributeItem { get; set; }
    }

    public partial class UserMappingDTO
    {
        public int UserMappingId { get; set; }
        public int? UserId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? UserIdCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class UserRoleDTO
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public int? UnitId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string KeyLock { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? TokenSince { get; set; }
        public string RegEmail { get; set; }
        public int? RoleMax { get; set; }
        public byte? RoleLevel { get; set; }
        public bool? IsRoleGroup { get; set; }
        public int? UserCreateId { get; set; }
        public int? UserEditId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class WebsiteDTO
    {
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public int? WebsiteParentId { get; set; }
        public string LogoHeader { get; set; }
        public string LogoFooter { get; set; }
        public string Hotline { get; set; }
        public string Hotmail { get; set; }
        public string LinkGooglePlus { get; set; }
        public string LinkFacebookPage { get; set; }
        public string LinkTwitter { get; set; }
        public string LinkYoutube { get; set; }
        public string LinkInstagram { get; set; }
        public string LinkLinkedIn { get; set; }
        public string LinkOther1 { get; set; }
        public string LinkOther2 { get; set; }
        public string LinkOther3 { get; set; }
        public string GoogleAnalitics { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuDT
    {
        public int MenuId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
        public List<MenuItemDT> listMenuItem { get; set; }
    }

    public partial class MenuItemDT
    {
        public int? MenuItemId { get; set; }
        public int? id { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Status { get; set; }
        public List<MenuItemDT> children { get; set; }
    }

    public partial class MenuChildren
    {
        public int? MenuItemId { get; set; }
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public List<MenuChildren> children { get; set; }
    }

    public partial class CategoryMenu
    {
        public int CategoryId { get; set; }
        public int CategoryParentId { get; set; }
        public string Name { get; set; }
        public int MenuItemId { get; set; }
        public bool? IsParent { get; set; }
        public List<CategoryMenu> Children { get; set; }
    }

    public partial class SessionAutionDTO
    {
        public int? SessionAutionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public bool? IsHome { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public List<SessionProductDTO> ListSessionProduct { get; set; }
    }

    public partial class SessionProductDTO
    {
        public int? SessionProductId { get; set; }
        public int? SessionAutionId { get; set; }
        public int? ProductId { get; set; }
        public int? CustomerWinId { get; set; }
        public decimal? PriceStart { get; set; }
        public decimal? PriceWin { get; set; }
        public bool? IsHome { get; set; }
        public DateTime? DateTimeWin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? TypeAuction { get; set; }
        public decimal? BidPriceDistance { get; set; }
    }

    public partial class AutionHistoryDTO
    {
        public int? AutionHistoryId { get; set; }
        public int? SessionAutionId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? PriceNew { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class KoiDTO
    {
        public int? ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceStart { get; set; }
        public string Image { get; set; }
        public bool? Check { get; set; }
        public bool? IsHome { get; set; }
        public int? TypeAuction { get; set; }
        public decimal? BidPriceDistance { get; set; }
    }

    public partial class SessionAutionMVC
    {
        public int? SessionAutionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public List<KoiAuction> ListSessionProduct { get; set; }
    }

    public partial class KoiAuction
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceStart { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string ProductNote { get; set; }
        public string TrademarkName { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public int? Width { get; set; }
        public string LinkYoutube { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
    }

    public partial class OrderWebDTO
    {
        public int OrderId { get; set; }
        public string Code { get; set; }
        public int? CustomerId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? ShippingStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal? OrderDelivery { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderPaid { get; set; }
        public decimal? OrderTotal { get; set; }
        public string CustomerNote { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        //Khách hàng
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string Address { get; set; }
        //Thanh toán
        public string IpAdress { get; set; }
        public string Locale { get; set; }
        public string ReturnUrl { get; set; }
        public string CardList { get; set; }
        public string AgainLink { get; set; }
        public string HashKey { get; set; }
        public string PaymentHistoryId { get; set; }
        public string PaymentRequest { get; set; }
        //
        public string ProductName { get; set; }
        public CustomerAddressDTO customerAddress { get; set; }
        public List<OrderItemDTO> listOrderItem { get; set; }
    }

    public partial class BranchDTO
    {
        public int? BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Contents { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PMQLXe { get; set; }
        public string QLCamera { get; set; }

    }

    public partial class ProductOutput
    {
        public int ProductId { get; set; }
        public int SessionAutionId { get; set; }
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
        public int? Discount { get; set; }
        public string Image { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
        public string LinkYoutube { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public byte? TypeProduct { get; set; }
        public int? TypeBid { get; set; }
        public string TypeBidStr { get; set; }
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
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string ManufacturerName { get; set; }
        public string TrademarkName { get; set; }
        public int? IsAuction { get; set; }
        public ProductReviewDTO productReviews { get; set; }
        public double? PointStar { get; set; }
        public double? PointStar5 { get; set; }
        public double? PointStar4 { get; set; }
        public double? PointStar3 { get; set; }
        public double? PointStar2 { get; set; }
        public double? PointStar1 { get; set; }
        public int? TotalReviews { get; set; }


    }

    public partial class GeneralProductReview
    {
        public int? TotalStar { get; set; }
        public List<GeneralProductReviewItem> ListGeneralProductReviewItem { get; set; }
        public List<ProductReview> ListProductReview { get; set; }
    }


    public partial class GeneralProductReviewItem
    {
        public int? TypeStar { get; set; }
        public int? CountReview { get; set; }
    }

    public partial class Producer
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Logo { get; set; }
    }

    public partial class HighlightNews
    {
        public int SessionAutionId { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public DateTime? DateStart { get; set; }
        public int? Location { get; set; }
    }

    public partial class NewsAndEventDTO
    {
        public int? NewsId { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Type { get; set; }
        public int? Location { get; set; }
    }

    //DT để sắp sếp kéo thả
    public partial class CategorySort
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int? CategoryParentId { get; set; }
        public string CategoryParentName { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Descriptions { get; set; }
        public int? Location { get; set; }
        public int? Level { get; set; }
        public List<CategorySort> categorySorts { get; set; }
    }

    //Sắp xếp kéo thả có thêm tổng số bản ghi
    public partial class FullCategorySort
    {
        public int? Sum { get; set; }
        public int? SumOnline { get; set; }
        public int? SumOffline { get; set; }
        public List<CategorySort> categorySorts { get; set; }
    }

    //DT đăng ký nhận bản tin
    public partial class ReceiveNews
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }
    }

    public partial class CustomerAddressDTO
    {
        public Guid CustomerAddressId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public bool? IsMain { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    //DTO đổi mật khẩu của khách hàng
    public partial class ResetPasswordCustomerDTO
    {
        public string PasswordInit { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public partial class rpdonhangDTO
    {
        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string TENMACBETONG { get; set; }
        public double METKHOIDATHANG { get; set; }
        public double METKHOITICHLUY { get; set; }
        public DateTime NGAYDATHANG { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NGAYDATHANGTITLE { get; set; }
        public string TENNV { get; set; }

    }

    public partial class rpNhapKhoDTO
    {
        public Guid ID { get; set; }
        public DateTime NGAY { get; set; }

        public decimal CAT1 { get; set; }
        public decimal CAT2 { get; set; }
        public decimal CAT3 { get; set; }
        public decimal DA1 { get; set; }
        public decimal DA2 { get; set; }
        public decimal DA3 { get; set; }
        public decimal XIMANG1 { get; set; }
        public decimal XIMANG2 { get; set; }
        public decimal XIMANG3 { get; set; }
        public decimal PHUGIA1 { get; set; }
        public decimal PHUGIA2 { get; set; }
        public decimal PHUGIA3 { get; set; }
        public string TRANGTHAI { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime Lastupdated { get; set; }


    }

    public partial class rpNhapKhoSrtDTO
    {
        public Guid ID { get; set; }
        public DateTime NGAY { get; set; }

        public string CAT1 { get; set; }
        public string CAT2 { get; set; }
        public string CAT3 { get; set; }
        public string DA1 { get; set; }
        public string DA2 { get; set; }
        public string DA3 { get; set; }
        public string XIMANG1 { get; set; }
        public string XIMANG2 { get; set; }
        public string XIMANG3 { get; set; }
        public string PHUGIA1 { get; set; }
        public string PHUGIA2 { get; set; }
        public string PHUGIA3 { get; set; }
        public string TRANGTHAI { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime Lastupdated { get; set; }


    }

    public partial class CustomerbodyDTO
    {
        public int MAMACBETONG { get; set; }
        public string TENMACBETONG { get; set; }
        public int CUONGDO { get; set; }
        public Single VL_1 { get; set; }
        public Single VL_2 { get; set; }
        public Single VL_3 { get; set; }
        public Single VL_4 { get; set; }
        public Single VL_5 { get; set; }
        public Single VL_6 { get; set; }
        public Single VL_7 { get; set; }
        public Single VL_8 { get; set; }
        public Single VL_9 { get; set; }
        public int COTLIEUMAX { get; set; }
        public string DOSUT { get; set; }
        public string name { get; set; }

    }


    public partial class VatLieuCusDTO
    {
        public long STT { get; set; }
        public string TENCUAVL { get; set; }
        public Boolean COPHAIPHUGIA { get; set; }
        public string TENLOAIVL { get; set; }
        public int MACUAVL { get; set; }


    }
    public partial class DashboardDTO
    {
        public int donhang { get; set; }
        public string Name { get; set; }
        public string TENNV { get; set; }
        public Double METKHOITICHLUY { get; set; }

        public int tongdonhang { get; set; }
        public int tongnhanvien { get; set; }

    }
    public partial class doughnutDTO
    {
        public Double DATRON { get; set; }
        public Double CHUATRON { get; set; }

    }
    public partial class DashboardChartDTO
    {
        public Double METKHOITICHLUY { get; set; }
        public String NGAYDATHANG { get; set; }

    }
    public partial class rpdonhangBranchDTO
    {

        public Double METKHOIDATHANG { get; set; }
        public Double METKHOITICHLUY { get; set; }
        public string Name { get; set; }

    }
    public partial class rpthongkeDTO
    {

        public long MALSTRON { get; set; }
        public long MACHITIETMETRON { get; set; }
        public DateTime NGAYTRON { get; set; }
        public DateTime GIOBATDAU { get; set; }
        public DateTime GIOXONG { get; set; }
        public string BIENSO { get; set; }
        public string TENMACBETONG { get; set; }
        public Single M3METRON { get; set; }
        public Single SOLUONG { get; set; }
        public Single SOLUONGT { get; set; }
        public bool COPHAIPHUGIA { get; set; }
        public string TENCUAVL { get; set; }
        public string TENLOAIVL { get; set; }
        public string USERNAME { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string DIADIEMXD { get; set; }
        public string TENNV { get; set; }
        public string TENHANGMUC { get; set; }
        public string name { get; set; }
    }
    public class DULIEUTHONGKE
    {
        public string STT { get; set; }
        public string NGAYTRON { get; set; }
        public string NGAYGIOTRON { get; set; }
        public string GIOBATDAU { get; set; }
        public string GIOXONG { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENHANGMUC { get; set; }
        public string TENDUAN { get; set; }
        public string TENDIADIEMXD { get; set; }
        public string TENNV { get; set; }
        public string BIENSO { get; set; }
        public string TENMACBETONG { get; set; }
        public string DIADIEMXD { get; set; }
        public float M3METRON { get; set; }
        public string TAIKHOAN { get; set; }
        public List<float> listcats { get; set; }
        public List<float> listdas { get; set; }
        public List<float> listximangs { get; set; }
        public List<float> listnuocs { get; set; }
        public List<float> listphugias { get; set; }
        public List<string> tenphugias { get; set; }
        public string name { get; set; }

    }
    public class DATAEXPORT
    {
        public int STT { get; set; }
        public string Name { get; set; }
        public bool issum { get; set; }
        public string namecell { get; set; }

        public bool is2phan { get; set; }
    }
    public class ThongKeTongVatTuDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }
    public class ThongKeChiTietChuyenXeDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }
    public class ThongKeChiTietKhoiLuongBeTongDTO
    {
        public int SOPHIEU { get; set; }
        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string TENMACBETONG { get; set; }
        public string TENNV { get; set; }
        public string TENLAIXE { get; set; }
        public string BIENSO { get; set; }
        public double M3METRON { get; set; }
        public string NGAYTRON { get; set; }
        public double M3TRENMETRON { get; set; }
        public int SOTTMETRON { get; set; }
        public double TONGVATTU { get; set; }
    }
    public class ThongKeTongKhoiLuongBeTongDTO
    {
        public string TENKHACHHANG { get; set; }
        public string TENDUAN { get; set; }
        public string TENMACBETONG { get; set; }
        public double SumM3METRON { get; set; }
        public DateTime NGAYTRON { get; set; }
        public int SOTTMETRON { get; set; }
    }
    public class ThongKeTongKhoiLuongBeTongGroupDTO
    {
        public object Key { get; set; }
        public List<ThongKeTongKhoiLuongBeTongDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalSumM3METRON { get; set; }
        public double TotalSOTTMETRON { get; set; }
    }
    public class ThongKeChiTietKhoiLuongBeTongGroupDTO
    {
        public object Key { get; set; }
        public List<ThongKeChiTietKhoiLuongBeTongDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalM3METRON { get; set; }
        public double TotalM3TRENMETRON { get; set; }
        public double TotalSOTTMETRON { get; set; }
        public double ToTalTONGVATTU { get; set; }
    }
    public class ThongKeChiTietVatTuGroupDTO
    {
        public object Key { get; set; }
        public List<ThongKeChiTietVatTuDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalM3METRON { get; set; }
        public double TotalColumnVL1 { get; set; }
        public double TotalColumnVL2 { get; set; }
        public double TotalColumnVL3 { get; set; }
        public double TotalColumnVL4 { get; set; }
        public double TotalColumnVL5 { get; set; }
        public double TotalColumnVL6 { get; set; }
        public double TotalColumnVL7 { get; set; }
        public double TotalColumnVL8 { get; set; }
        public double TotalColumnVL9 { get; set; }
        public double TotalColumnVL10 { get; set; }
        public double TotalColumnVL11 { get; set; }
        public double TotalColumnVL12 { get; set; }
        public double TotalColumnVL13 { get; set; }
        public double TotalColumnVL14 { get; set; }
        public double TotalColumnVL15 { get; set; }
        public double TotalColumnVL16 { get; set; }
        public double TotalColumnVL17 { get; set; }
        public double TotalColumnVL18 { get; set; }
    }
    public class ThongKeChiTietVatTuGroupGridDTO
    {
        public object Key { get; set; }
        public List<ThongKeChiTietVatTuGroupGridChildDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalM3METRON { get; set; }
        public List<double> TotalColumnVL { get; set; }
    }
    public class ThongKeChiTietVatTuGroupGridChildDTO
    {
        public object Key { get; set; }
        public List<ThongKeChiTietVatTuGridDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalM3METRON { get; set; }
        public List<double> TotalColumnVL { get; set; }
    }
    public class MaLichSuTronDTO
    {
        public long MALSTRON { get; set; }
    }
    public class ListCuaVLDeactive
    {
        public string STTCUAVL { get; set; }
        public string TENCUAVL { get; set; }
    }

    public class BaoCaoXuatKhoDTO
    {
        public string header { get; set; }
        public List<string> rows { get; set; }
    }
    public partial class BaoCaoXuatKhoNewDTO
    {
        public string MADVCS { get; set; }
        public string MAGD { get; set; }
        public string MAKHACHHANG { get; set; }
        public string MANGUOINHAN { get; set; }
        public string DIENGIAI { get; set; }
        public string NGAYTRON { get; set; }
        public string MAQUYENSO { get; set; }
        public string SOCHUNGTU { get; set; }
        public string MAVATTU { get; set; }
        public string MAKHO { get; set; }
        public double SOLUONG { get; set; }
        public string GIANGOAITE { get; set; }
        public string TIENNGOAITE { get; set; }
        public string MANGOAITE { get; set; }
        public string TYGIA { get; set; }
        public string GIA { get; set; }
        public string TIEN { get; set; }
        public string TKNO { get; set; }
        public string TKCO { get; set; }
        public string MADUAN { get; set; }
        public string MAPHI { get; set; }
        public string MASANPHAM { get; set; }
        public string MABPHT { get; set; }
        public string SOLSX { get; set; }
        public string MATRAM { get; set; }
        public string KLTRON { get; set; }
        public string KLGIAO { get; set; }
    }
    public class BaoCaoXuatKhoGroupDTO
    {
        public object Key { get; set; }
        public List<BaoCaoXuatKhoNewDTO> Data { get; set; }
        public bool Expanded { get; set; }
        public double TotalSOLUONG { get; set; }
    }

    public class TongTheTichTheoNgayTron
    {
        public List<string> Day { get; set; }
        public List<double> SumM3 { get; set; }
        public double Total { get; set; }
        public string Time { get; set; }
    }

    public class VatLieuTheoNgay
    {
        public List<string> LOAICUAVL { get; set; }
        public List<double> SUMSOLUONG { get; set; }
        public List<DataPoint> dataPoints { get; set; }
        public List<VatLieuTheoNgayDetail> VatLieuTheoNgayDetails { get; set; }
        public List<VatLieuTheoNgayData> VatLieuTheoNgays { get; set; }
    }

    public class DataPoint
    {
        public double y { get; set; }
        public string x { get; set; }
    }

    public class VatLieuTheoNgayDetail
    {
        public string CuaVL { get; set; }
        public string Color { get; set; }
    }

    public class DataCheckColor
    {
        public string LoaiCuaVL { get; set; }
        public string Color { get; set;}
    }

    public class VatLieuTheoNgayData
    {
        public string TenCuaVL { get; set; }
        public string LoaiCuaVL { get; set; }
        public double KhoiLuong { get; set; }
    }
}