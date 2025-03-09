using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Payment
{
    public class QueryRequest
    {
        [System.ComponentModel.DefaultValue(2)]
        public int vpc_Version { get; set; }
        [System.ComponentModel.DefaultValue("queryDR")]
        public string vpc_Command { get; set; }
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_Merchant { get; set; }
        public string vpc_AccessCode { get; set; }
        public string vpc_User { get; set; }
        public string vpc_Password { get; set; }
        public string vpc_SecureHash { get; set; }
    }
}
