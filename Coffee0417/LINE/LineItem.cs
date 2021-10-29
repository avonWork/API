using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.LINE
{
    public class LineItem
    {
        /// <summary>
        /// 出貨單總計(付款總金額)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 貨幣
        /// </summary>
        public string Currency { get; set; }
    }
}