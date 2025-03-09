using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class PaymentHistory
    {
        public Guid PaymentHistoryId { get; set; }
        public byte? Version { get; set; }
        public string Currency { get; set; }
        public string Command { get; set; }
        public string AccessCode { get; set; }
        public string Merchant { get; set; }
        public string Locale { get; set; }
        public string ReturnUrl { get; set; }
        public string MerchTxnRef { get; set; }
        public string OrderInfo { get; set; }
        public string Amount { get; set; }
        public string TicketNo { get; set; }
        public string CardList { get; set; }
        public string AgainLink { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerId { get; set; }
        public string SecureHash { get; set; }
        public string TxnResponseCode { get; set; }
        public string TransactionNo { get; set; }
        public string Message { get; set; }
        public string Card { get; set; }
        public string PayChannel { get; set; }
        public string CardUid { get; set; }
        public byte? PayType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
