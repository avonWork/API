using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Coffee0417.Models;
using Coffee0417.Utils;
using static Coffee0417.Utils.Enum;
using static Coffee0417.Utils.Myall;

namespace Coffee0417.Controllers
{
    [EnableCors("*", "*", "*")]
    public class OrdersController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/BkOrders
        //後台全部訂單(後台首頁)
        //訂單狀態不等於0
        //ok
        [Route("api/BkOrders")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            if (permission != 2) return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var item = db.Orders.Where(y => y.Status != 0).
                Select(x => new
                {
                    AddTime = x.AddTime.Year + "/" + x.AddTime.Month + "/" + x.AddTime.Day,
                    x.Id,
                    x.Status,
                    x.Payment,
                    x.User.Name,
                    x.User.Email
                }).ToList();

            if ((item != null) && (!item.Any()))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.尚未有訂單
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得全部訂單,
                item
            });
        }

        // GET: api/BkOrders
        //後台條件訂單(點選後觸發)
        //ok
        //https://localhost:44304/api/BkOrders/?status=1&payment=1
        [JwtAuthFilter]
        [Route("api/BkOrders")]
        public IHttpActionResult GetOrder(OrderStatus status, Payment payment)
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            if (permission != 2) return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var orderlList = db.Orders.Where(x => x.Status == status && x.Payment == payment).Select(x => new
            {
                AddTime = x.AddTime.Year + "/" + x.AddTime.Month + "/" + x.AddTime.Day,
                x.Id,
                x.Status,
                x.Payment,
                x.User.Name,
                x.User.Email
            }).ToList();
            if ((orderlList != null) && (!orderlList.Any()))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.無符合訂單
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得符合訂單,
                orderlList
            });
        }

        // PUT: api/BkOrders/status
        //後台商品狀態修改
        //ok
        [HttpPut]
        [JwtAuthFilter]
        [Route("api/BkOrders/status")]
        public IHttpActionResult PutOrderStatus(int id, [FromBody] Order order)
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            if (permission != 2) return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var orderlist = db.Orders.FirstOrDefault(x => x.Id == id);
            orderlist.Status = order.Status;
            orderlist.EidtTime = DateTime.Now;
            db.SaveChanges();
            return Ok(new
            {
                check = "ok",
                message = message.商品狀態修改成功
            });
        }

        // GET:  api/Orders/Consignee/1
        //確定收件人資料--依訂單秀收件人資料
        [HttpGet]
        [JwtAuthFilter]
        public IHttpActionResult Consignee(int id)
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            var userid = JwtAuthFilter.GetTokenId(Request.Headers.Authorization.Parameter);
            //var all = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
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
            //用戶訂單資料
            var orderdata = db.Orders.FirstOrDefault(x => x.Id == id && x.UserId == userid);
            if (orderdata == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.無符合訂單
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得訂單收件人資料,
                orderdata.Name,
                orderdata.Phone,
                orderdata.Address
            });
        }

        // GET: api/Orders/GetOrderList/9
        //前台訂購完成(本人訂單資料)
        //通知訂單結果
        //ok
        [HttpGet]
        [JwtAuthFilter]
        public IHttpActionResult GetOrderList(int id)
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            var userid = JwtAuthFilter.GetTokenId(Request.Headers.Authorization.Parameter);
            //var all = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
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
            //用戶訂單資料
            var orderdata = db.Orders.FirstOrDefault(x => x.Id == id && x.UserId == userid);
            if (orderdata == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.無符合訂單
                });
            }

            //用戶訂單資料
            var detaildata = orderdata.OrderDetails.Select(x => new
            {
                x.ProductImg,
                x.ProductName,
                x.ProductBrew,
                x.UnitPrice,
                x.Quantity,
                Price = (x.Quantity * x.UnitPrice)
            }).ToList();

            string urlSetting = ConfigurationSettings.AppSettings["Url"];
            string ChannelAccessToken = ConfigurationManager.AppSettings["ChannelAccessToken"];

            //LINE通知訂單結果 //lline有加好友1  linepay付款成功
            if (orderdata.Notice == Notice.LINE通知 && orderdata.Payment == Payment.LINEPay)
            {
                string bodycontent = $"vacationaroma訂單付款成功通知: \n您的訂單已LINEPay付款完成! \n您可以至會員訂單查詢瞭解訂單詳情與處理進度 \n" + urlSetting + "login.html";
                var AdminUserId = orderdata.UserId; //LINE會員
                isRock.LineBot.Bot bot = new isRock.LineBot.Bot(ChannelAccessToken);
                bot.PushMessage(AdminUserId, bodycontent);
            }

            //LINE通知訂單結果 //lline有加好友1 門市取貨
            if (orderdata.Notice == Notice.LINE通知 && orderdata.Payment == Payment.門市取貨)
            {
                string bodycontent = $"vacationaroma門市取貨並付款通知: \n請於3日內至門市取貨並完成付款,報姓名電話資料以方便核對!謝謝您~ \n付款金額為: " + orderdata.SubTotal + " 元\n" + urlSetting;
                var AdminUserId = orderdata.UserId; //LINE會員
                isRock.LineBot.Bot bot = new isRock.LineBot.Bot(ChannelAccessToken);
                bot.PushMessage(AdminUserId, bodycontent);
            }

            //寄信通知訂單結果 //lline沒加好友2  linepay付款成功
            if (orderdata.Notice == Notice.Email通知 && orderdata.Payment == Payment.LINEPay)
            {
                string bodycontent = $"<p>您的訂單已LINEPay付款完成！<p><p>您可以至會員訂單查詢瞭解訂單詳情與處理進度<p><br/><a href='" + urlSetting + "login.html'>登入</a>";
                string title = "vacationaroma訂單付款成功通知:";
                SendEmail(userdata.Email, bodycontent, title);
            }

            //寄信通知訂單結果 //門市取貨
            if (orderdata.Notice == Notice.Email通知 && orderdata.Payment == Payment.門市取貨)
            {
                string bodycontent = $"<p>您的訂單已成立!<p><p>請於3日內至門市取貨並完成付款,報姓名電話資料以方便核對!謝謝您~<p><p>付款金額為: " + orderdata.SubTotal + " 元<p><br/><a href='" + urlSetting + "'>vacationaroma</a>";
                string title = "vacationarom門市取貨並付款通知:";
                SendEmail(userdata.Email, bodycontent, title);
            }
            //回傳前端
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderdata.ProTotal,
                orderdata.SubTotal,
                detaildata
            });
        }

        // GET: api/Orders/GetOrderList
        //前台會員系統訂單資料 (歷史訂單)
        //ok
        [HttpGet]
        [JwtAuthFilter]
        public IHttpActionResult GetOrderList()
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            var userid = JwtAuthFilter.GetTokenId(Request.Headers.Authorization.Parameter);
            //var all = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
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
            //用戶訂單資料
            var orderdata = db.Orders.Where(x => x.UserId == userid && x.Status!=0).Join(
                db.OrderDetails,
                a => a.Id,
                b => b.OrderId,
                (a, b) => new
                {
                    AddTime = a.AddTime.Year + "/" + a.AddTime.Month + "/" + a.AddTime.Day,
                    a.Id,
                    a.Name,
                    a.Status,
                    a.SubTotal,
                    a.Payment,
                    b.ProductName,
                    b.ProductBrew,
                    b.Quantity,
                    b.UnitPrice,
                    Price = (b.Quantity * b.UnitPrice),
                }
            ).ToList();
            if ((orderdata != null) && (!orderdata.Any()))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.無符合訂單
                });
            }

            return Ok(new
            {
                check = "ok",
                message = message.取得全部訂單,
                orderdata
            });
        }

        // GET: api/BkOrders/OrderList/3
        //後台詳細訂單
        //ok
        [HttpGet]
        [JwtAuthFilter]
        [Route("api/BkOrders/OrderList/{id}")]
        public IHttpActionResult OrderList(int id)
        {
            if (Request.Headers.Authorization.Parameter == "") return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            var permission = JwtAuthFilter.GetTokenPermission(Request.Headers.Authorization.Parameter);
            var userid = JwtAuthFilter.GetTokenId(Request.Headers.Authorization.Parameter);
            //var all = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            if (permission != 2) return Ok(new
            {
                check = "no",
                message = message.權限驗證不符
            });
            //用戶訂單資料
            var orderdata = db.Orders.FirstOrDefault(x => x.Id == id);
            if (orderdata == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.無符合訂單
                });
            }
            //用戶訂單資料
            var detaildata = orderdata.OrderDetails.Select(x => new
            {
                x.ProductName,
                x.ProductBrew,
                x.Quantity,
                Price = (x.Quantity * x.UnitPrice)
            }).ToList();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderdata.Name,
                orderdata.Phone,
                orderdata.Address,
                orderdata.ProTotal,
                orderdata.SubTotal,
                detaildata
            });
        }

        // POST: api/Orders/PostOrder
        //確認訂單--接收前端資料JSON
        //訂單資料 填寫收件人資料按鈕 --存資料庫
        //ok
        [HttpPost]
        public object PostOrder(dynamic obj)
        {
            string token = Convert.ToString(obj.mytoken);
            var permission = JwtAuthFilter.GetTokenPermission(token);
            var userid = JwtAuthFilter.GetTokenId(token);
            //防呆--權限
            if (permission != 1) return Ok(new
            {
                check = "no",
                message = message.尚未註冊
            });
            var user = db.Users.FirstOrDefault(x => x.UserId == userid);
            if (user.CheckAccount == 0) return Ok(new
            {
                check = "no",
                message = message.會員帳號尚未開通
            });
            var ods = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(Convert.ToString(obj.order));

            if (getphone(ods.Phone) == false)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.手機格式不正確
                });
            }
            int prototal = ods.ProTotal;
            int subtotal = ods.SubTotal;

            //防呆--空值
            if (ods.Payment != Payment.LINEPay && ods.Payment != Payment.門市取貨)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.付款方式未選擇
                });
            }

            //防呆--空值
            if (string.IsNullOrEmpty(ods.Name))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.收件人姓名為空值
                });
            }
            if (string.IsNullOrEmpty(ods.Phone)) return Ok(new
            {
                check = "no",
                message = message.收件人手機為空值
            });

            if (string.IsNullOrEmpty(ods.Address)) return Ok(new
            {
                check = "no",
                message = message.收件人地址為空值
            });

            //訂單(收貨人)  前端根據登入拿到true傳值Notice==1 (line通知)
            var order = new Order
            {
                UserId = userid,
                Name = ods.Name,
                Phone = ods.Phone,
                Address = ods.Address,
                Payment = ods.Payment,
                ProTotal = 0,
                Shipping = ods.Shipping,
                SubTotal = 0,
                Notice = ods.Notice,
                AddTime = DateTime.Now
            };
            var tempDetails = ods.OrderDetails;
            //訂單明細(產品)
            foreach (var pro in tempDetails)
            {
                var newDerail = new OrderDetail
                {
                    ProductName = pro.ProductName,
                    UnitPrice = pro.UnitPrice,
                    Quantity = pro.Quantity,
                    ProductBrew = pro.ProductBrew,
                    ProductImg = pro.ProductImg
                };
                db.OrderDetails.Add(newDerail);
                //產品合計
                order.ProTotal += (newDerail.UnitPrice * newDerail.Quantity);
            }
            //訂單合計(含運費)
            order.SubTotal = order.ProTotal + order.Shipping;

            //門市取貨  //轉型別
            if (ods.Payment.Equals(Payment.門市取貨))
            {
                order.Status = OrderStatus.處理中;
            }
            db.Orders.Add(order);

            //防呆--訂單金額
            if (prototal != order.ProTotal) return Ok(new
            {
                check = "no",
                message = message.前台後台ProTotal不符
            });
            if (subtotal != order.SubTotal) return Ok(new
            {
                check = "no",
                message = message.前台後台SubTotal不符
            });

            db.SaveChanges();
            return Ok(new
            {
                check = "ok",
                message = message.訂單資料存進資料庫,
                order.Id
            });
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}