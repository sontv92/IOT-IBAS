using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOITWebApp.Models
{
    public class FilteredPagination : BasePagination
    {
        [System.ComponentModel.DefaultValue("")]
        public string query { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string select { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string search { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int companyid { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int sort { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string Branchlist { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string TENKHACHHANG { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string BIENSO { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string TENMACBETONG { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string TENHANGMUC { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string CHEDO { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int status { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public DateTime tungay { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public DateTime denngay { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string timetungay { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string timedenngay { get; set; }
        [System.ComponentModel.DefaultValue(true)]
        public bool ckbKhachHang { get; set; }
        [System.ComponentModel.DefaultValue(true)]
        public bool ckbXeTron { get; set; }
        [System.ComponentModel.DefaultValue(true)]
        public bool ckbMacBeTong { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string TENNV { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string CVL { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string KDL { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MAPHIEU { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string TENDUAN { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string TAIXE { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string GroupBy { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int CUAVL { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MALIENKETMAC { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MALIENKETKH { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MADVCS { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MAGD { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string QUYEN { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string MAKHO { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string TKNO { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string TKCO { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int? TypeTram { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int? Module { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string VATLIEU { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string TenKH { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string NGUOICAN { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string KIEUCAN { get; set; }

    }
}