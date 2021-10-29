using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static Coffee0417.Utils.Enum;

namespace Coffee0417.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "userid")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Display(Name = "收貨人")]
        public string Name { get; set; }

        [Display(Name = "收貨人地址")]
        public string Address { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "收貨人Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "收貨人電話")]
        public string Phone { get; set; }

        [Display(Name = "訂單狀態")]
        public OrderStatus Status { get; set; }

        [Display(Name = "付款方式")]
        public Payment Payment { get; set; }

        [Display(Name = "訂單通知方式")]
        public Notice Notice { get; set; }

        [Display(Name = "產品總金額")]
        public int ProTotal { get; set; }

        [Display(Name = "運費")]
        public int Shipping { get; set; }

        [Display(Name = "訂單總金額")]
        public int SubTotal { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }

        [Display(Name = "創建時間")]
        public DateTime AddTime { get; set; }

        [Display(Name = "編輯時間")]
        public DateTime? EidtTime { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}