using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.LINE
{
    public class Product
    {
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品數量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 商品圖片
        /// </summary>
        public string ImageUrl { get; set; }
    }
}