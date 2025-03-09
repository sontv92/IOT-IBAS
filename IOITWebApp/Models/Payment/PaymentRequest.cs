using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Payment
{
    public class PaymentRequest
    {
        [System.ComponentModel.DefaultValue(2)]
        public int vpc_Version { get; set; }
        [System.ComponentModel.DefaultValue("VND")]
        public string vpc_Currency { get; set; }
        [System.ComponentModel.DefaultValue("pay")]
        public string vpc_Command { get; set; }
        public string vpc_AccessCode { get; set; }
        public string vpc_Merchant { get; set; }
        [System.ComponentModel.DefaultValue("en")]
        public string vpc_Locale { get; set; }
        public string vpc_ReturnURL { get; set; }
        //
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_OrderInfo { get; set; }
        public string vpc_Amount { get; set; }
        public string vpc_TicketNo { get; set; }
        public string vpc_CardList { get; set; }
        public string AgainLink { get; set; }
        public string Title { get; set; }
        public string vpc_Customer_Phone { get; set; }
        public string vpc_Customer_Email { get; set; }
        public string vpc_Customer_Id { get; set; }
        //
        public string vpc_SecureHash { get; set; }
    }
}
