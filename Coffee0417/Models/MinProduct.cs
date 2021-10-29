using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Coffee0417.Models
{
    public class MinProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //prop
        public int Id { get; set; }

        [Required]
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "產品圖片")]
        public string ProductImg { get; set; }

        [Required]
        [Display(Name = "產品價格")]
        public int ProducPrice { get; set; }

        [Display(Name = "產品描述")]
        public string ProductDescription { get; set; }

        [Display(Name = "產品庫存")]
        public int ProductStock { get; set; }

        [Display(Name = "產品內容")]
        public string ProductContent { get; set; }

        [Display(Name = "產品網址")]
        public string ProductUri { get; set; }

        [Display(Name = "產品貨號")]
        public string ProductItemNo { get; set; }

        [Display(Name = "產品分類")]
        public string ProductClass { get; set; }

        [Display(Name = "產品標籤")]
        public string Productlabel { get; set; }

        [Display(Name = "創建時間")]
        public DateTime? AddTime { get; set; }

        [Display(Name = "編輯時間")]
        public DateTime? EidtTime { get; set; }
    }
}