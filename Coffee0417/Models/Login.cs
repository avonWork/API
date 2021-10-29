using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Coffee0417.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "帳號")]
        [Required(ErrorMessage = "{0}必填")]
        public string Account { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0}必填")]
        public string Email { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "舊密碼")]
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}