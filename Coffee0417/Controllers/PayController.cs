using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Coffee0417.LINE;
using Coffee0417.Utils;
using Coffee0417.Models;
using System.Web;
using static Coffee0417.Utils.Enum;

using Coffee0417.Models;

using System.Web.Http.Cors;
using System.Configuration;

namespace Coffee0417.Controllers
{
    [EnableCors("*", "*", "*")]
    public class PayController : ApiController
    {
        private Model1 db = new Model1();

        private string userid;

        private int permission;

        //有動到金額兩隻API都要改
        //ConfirmUrl = "https://vacationaroma.rocket-coding.com/pay.html",
        // GET: api/Pay/Getreserve
        //LINEPay--Reserve API 請求
        //付款按鈕--轉付款網址
        //ok
        [JwtAuthFilter]
        public IHttpActionResult Getreserve(int id)
        {
            string transaction = "";
            string web = "";
            //LINE 後台的 Channel Secret Key
            string channelSecret = "bced68ed4ad69f1fcaf8faef538997d3";
            //LINE PAY 的 Request 網址
            string requestUri = "/v3/payments/request";
            string URL = "https://sandbox-api-pay.line.me/v3/payments/request";
            string ChannelId = "1655777719";

            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            userid = JwtAuthFilter.GetTokenId(Request.Headers.Authorization.Parameter);
            //防呆
            if (permission != 1) return Ok(new
            {
                check = "no",
                message = message.尚未註冊
            });
            var userdata = db.Users.FirstOrDefault(x => x.UserId == userid);
            if (userdata.CheckAccount == 0) return Ok(new
            {
                check = "no",
                message = message.會員帳號尚未開通
            });
            var orderdata = db.Orders.FirstOrDefault(x => x.Id == id && x.UserId == userid);
            int subTotal = orderdata.SubTotal;
            //防呆 訂單狀態如果處理中
            //訂單id
            //HttpContext.Current.Session["id"] = orderdata.Id;
            string urlSetting = ConfigurationSettings.AppSettings["Url"];
            LineForm json = new LineForm
            {
                Amount = subTotal,
                Currency = "TWD",
                OrderId = "X" + DateTime.Now.ToString("MMddHHmm") + String.Format("{0:00}", orderdata.Id),
                Packages = new List<Package>
                {
                    new Package
                    {
                        Id = "1",
                        Amount = subTotal,
                        Name = "咖啡豆一組",
                        Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "咖啡豆一組",
                                //遠端圖片(linepay商品圖)
                                ImageUrl="https://cdn.pixabay.com/photo/2016/08/07/16/23/coffee-1576537_1280.jpg",
                                Quantity = 1,
                                Price =subTotal
                            }
                        }
}
                },
                RedirectUrls = new RedirectUrls
                {
                    ConfirmUrl = urlSetting + "Xsp5Cart.html",
                    CancelUrl = "https://pay-store.line.com/order/payment/cancelUrl"
                }
            };
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            var order = JsonConvert.SerializeObject(json, setting);

            string nonce = Guid.NewGuid().ToString();
            string result = LinePayHMACSHA256((channelSecret + requestUri + order + nonce), channelSecret);

            HttpContent httpContent = new StringContent(order);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.Add("X-LINE-ChannelId", ChannelId);
            httpContent.Headers.Add("X-LINE-Authorization-Nonce", nonce);
            httpContent.Headers.Add("X-LINE-Authorization", result);

            HttpClient client = new HttpClient();
            var response = client.PostAsync(URL, httpContent).Result.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(response))
            {
                var item = JsonConvert.DeserializeObject<LinePayResponse>(response);
                if (item.returnCode == "0000")
                {
                    transaction = item.info.transactionId.ToString();
                    web = item.info.paymentUrl.web;
                    return Ok(new
                    {
                        check = "ok",
                        message = message.linepay付款網址成功,
                        web,
                        transaction,
                    });
                }
            }
            return Ok(new
            {
                check = "no",
                message = message.linepay付款網址不成功
            });
        }

        // POST:api/Pay/Postconfirm?transactionId=2021040100658980610
        //LINEPay付款完成
        //ok
        [HttpPost]
        [AcceptVerbs("GET", "Post")]
        public IHttpActionResult Postconfirm(string transactionId, [FromBody] Order order)
        {
            //LINE 後台的 Channel Secret Key
            string channelSecret = "bced68ed4ad69f1fcaf8faef538997d3";
            //LINE PAY 的 Request 網址
            string url = string.Format("https://sandbox-api-pay.line.me/v2/payments/{0}/confirm",
                transactionId);
            string ChannelId = "1655777719";

            //int id = Convert.ToInt16(HttpContext.Current.Session["id"]);
            //用戶訂單資料
            var orderdata = db.Orders.FirstOrDefault(x => x.Id == order.Id);
            LineItem data = new LineItem
            {
                Amount = orderdata.SubTotal,
                Currency = "TWD",
            };
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var money = JsonConvert.SerializeObject(data, setting);

            HttpContent httpContent = new StringContent(money);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.Add("X-LINE-ChannelId", ChannelId);
            httpContent.Headers.Add("X-LINE-ChannelSecret", channelSecret);

            HttpClient client = new HttpClient();
            var response = client.PostAsync(url, httpContent).Result.Content.ReadAsStringAsync().Result;

            if (!string.IsNullOrEmpty(response))
            {
                var item = JsonConvert.DeserializeObject<LinePayResponse>(response);
                if (item.returnCode == "0000")
                {
                    return Ok(new
                    {
                        check = "ok",
                        message = message.linepay付款成功
                    });
                }
            }
            return Ok(new
            {
                check = "no",
                message = message.linepay付款失敗
            });
        }

        // PUT: api/Pay/Put
        //訂購成功修改訂單狀態=處理中(後台)
        //ok
        public IHttpActionResult Put([FromBody] Order order)
        {
            var result = db.Orders.FirstOrDefault(x => x.Id == order.Id);
            if (result == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.linepay訂單狀態修改失敗
                });
            }
            result.Status = OrderStatus.處理中;
            db.SaveChanges();
            //註銷Session訂單Id
            HttpContext.Current.Session["id"] = "";

            return Ok(new
            {
                check = "ok",
                message = message.linepay訂單狀態修改成功
            });
        }

        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Pay/5
        public void Delete(int id)
        {
        }

        public static string LinePayHMACSHA256(string message, string key)
        {
            key = key ?? "";
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}