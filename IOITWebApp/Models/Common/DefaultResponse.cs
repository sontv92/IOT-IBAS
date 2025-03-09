
using IOITWebApp.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOITWebApp.Models
{
    public class DefaultResponse
    {
        public Meta meta { get; set; }
        public object data { get; set; }
        public object data1 { get; set; }
        public object dataSUM { get; set; }
        public object metadata { get; set; }
        public string tongKg { get; set; }
        public string tongm3 { get; set; }

        public string tongBeTongThang { get; set; }

        public string tongBeTongNgay { get; set; }

        public string tongDonHangNgay { get; set; }

        public string tongDonHangHoanThanh { get; set; }

        public string[] lstXeTron { get; set; }

        public string[] lstPhutHoanThanh { get; set; }

        public double[] m3XeTron { get; set; }
        public List<ColumnName> ColumnNames { get; set; }
        public TongTheTichTheoNgayTron tongTheTichTheoNgayTron { get; set; }
        public VatLieuTheoNgay vatLieuTheoNgay { get; set; }

    }
    public class ColumnName
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class Meta
    {
        public int error_code { get; set; }
        public string error_message { get; set; }

        public Meta()
        { }

        public Meta(int errorCode, string errorMessage)
        {
            this.error_code = errorCode;
            this.error_message = errorMessage;
        }
    }

    public class Metadata
    {
        public int item_count { get; set; }
        public decimal total { get; set; }
        public Metadata()
        { }

        public Metadata(int item_count)
        {
            this.item_count = item_count;
        }
        public Metadata(decimal total)
        {
            this.total = total;
        }

        public Metadata(int item_count, decimal total)
        {
            this.item_count = item_count;
            this.total = total;
        }
    }
    

    public class MetadataTotal
    {
        public int Count { get; set; }
        public int TotalInit { get; set; }
        public int TotalConfirm { get; set; }
        public int TotalDelivery { get; set; }
        public int TotalDelived { get; set; }
        public int TotalCancel { get; set; }

        public MetadataTotal()
        { }

        public MetadataTotal(int Count, int TotalInit, int TotalConfirm, int TotalDelivery, int TotalDelived, int TotalCancel)
        {
            this.Count = Count;
            this.TotalInit = TotalInit;
            this.TotalConfirm = TotalConfirm;
            this.TotalDelivery = TotalDelivery;
            this.TotalDelived = TotalDelived;
            this.TotalCancel = TotalCancel;
        }

    }

    public class APIResponseData
    {
        public Meta meta { get; set; }
        public object data { get; set; }
    }


}