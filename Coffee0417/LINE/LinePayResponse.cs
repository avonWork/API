using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.LINE
{
    public class LinePayResponse
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public Info info { get; set; }
    }

    public class Info
    {
        public string orderId { get; set; }
        public Paymenturl paymentUrl { get; set; }
        public long transactionId { get; set; }
        public string paymentAccessToken { get; set; }
        public List<PayInfo> payInfo { get; set; }
    }

    public class Paymenturl
    {
        public string web { get; set; }
        public string app { get; set; }
    }

    public class PayInfo
    {
        public string method { get; set; }
        public int amount { get; set; }
    }
}