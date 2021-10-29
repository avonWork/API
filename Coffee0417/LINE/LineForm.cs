using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.LINE
{
    public class LineForm
    {
        /// <summary>
        /// 出貨單總計(付款總金額)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 貨幣
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        ///出貨單編號
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 訂單明細內容
        /// </summary>
        public List<Package> Packages { get; set; }

        /// <summary>
        /// 網址
        /// </summary>
        public RedirectUrls RedirectUrls { get; set; }
    }
}