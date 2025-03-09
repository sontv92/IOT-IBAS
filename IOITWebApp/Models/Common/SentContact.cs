using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace IOITWebApp.Models.Common
{
    public class SentContact
    {
        [Required(ErrorMessage = " Vui lòng nhập email ")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Vui lòng nhập email hợp lệ ")]
        public string ToEmail { get; set; }

        //[Required(ErrorMessage = " Vui lòng nhập họ tên")]
        //[StringLength(100, MinimumLength = 2, ErrorMessage = " Vui lòng nhập họ tên đầy đủ ")]
        public string Subject { get; set; }

        //[Required(ErrorMessage = " Vui lòng nhập nội dung liên hệ")]
        //[StringLength(1000, MinimumLength = 10, ErrorMessage = " Bạn có thể viết nhiều hơn !")]
        public string Content { get; set; } 
    }
}