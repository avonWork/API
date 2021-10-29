using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Coffee0417.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "訂單Id")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "產品圖片")]
        public string ProductImg { get; set; }

        [Display(Name = "產品沖煮方式")]
        public string ProductBrew { get; set; }

        [Required]
        [Display(Name = "單價")]
        public int UnitPrice { get; set; }

        [Required]
        [Display(Name = "數量")]
        public int Quantity { get; set; }
    }
}