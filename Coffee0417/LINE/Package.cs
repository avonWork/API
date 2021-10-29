using Coffee0417.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coffee0417.Models;

namespace Coffee0417.LINE
{
    public class Package
    {
        /// <summary>
        /// 訂單id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 訂單名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品清單
        /// </summary>
        public List<Product> Products { get; set; }
    }
}