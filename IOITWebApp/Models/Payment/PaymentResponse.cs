using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Payment
{
    public class PaymentResponse
    {
        public string vpc_Command { get; set; }
        public string vpc_Locale { get; set; }
        public string vpc_CurrencyCode { get; set; }
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_Merchant { get; set; }
        public string vpc_OrderInfo { get; set; }
        public string vpc_Amount { get; set; }
        public string vpc_TxnResponseCode { get; set; }
        public string vpc_TransactionNo { get; set; }
        public string vpc_Message { get; set; }
        public string vpc_Card { get; set; }
        public string vpc_PayChannel { get; set; }
        public string vpc_CardUid { get; set; }
        //
        public string vpc_SecureHash { get; set; }
    }
}
