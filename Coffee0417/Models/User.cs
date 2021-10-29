using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Coffee0417.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //prop
        public int Id { get; set; }

        [Key]
        [Display(Name = "身分識別id")]
        public string UserId { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "是否LINE登入")]
        public bool LINE { get; set; }

        [Display(Name = "使用者帳號")]
        public string Account { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "使用者密碼")]
        public string Password { get; set; }

        [Display(Name = "密碼鹽")]
        public string PasswordSalt { get; set; }

        [Display(Name = "頭像圖片")]
        public string ImgName { get; set; }

        [Display(Name = "權限")]
        public int Authority { get; set; }

        [Display(Name = "會員開通")]
        public int CheckAccount { get; set; }

        [Display(Name = "使用者手機")]
        public string Phone { get; set; }

        [Display(Name = "使用者Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "創建時間")]
        public DateTime? AddTime { get; set; }
    }
}