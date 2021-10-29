using Coffee0417.Models;
using Coffee0417.LINE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using static Coffee0417.Utils.Enum;
using System.Web.Http.Cors;
using Coffee0417.Utils;
using System.Configuration;

namespace Coffee0417.Controllers
{
    [EnableCors("*", "*", "*")]
    public class LineloginController : ApiController
    {
        private Model1 db = new Model1();

        //登入跳轉
        //Callback URL =https://vacationaroma.rocket-coding.com/va/index.html && redirect_uri
        //以下自行修改從WebConfig讀取

        private string redirect_uri = ConfigurationSettings.AppSettings["Url"]+"index.html";

        //private string redirect_uri = "https://localhost:44316/lineindex.html";

        private string client_id = "1655768349"; //Channel ID
        private string client_secret = "3262667ffdd649c1f9f14d7f0bbe23fd"; //Channel secret
        private string client_nonce = "avonCoffee"; //通關密語
        private string bot_prompt = "aggressive"; //LINE 官方帳號加入好友
        private string id_token;
        private string access_token;

        // GET api/Linelogin/GetLineLoginUrl
        //line登入網址
        public IHttpActionResult GetLineLoginUrl()
        {
            //state使用隨機字串比較安全
            //每次Ajax Request都產生不同的state字串，避免駭客拿固定的state字串將網址掛載自己的釣魚網站獲取用戶的Line個資授權(CSRF攻擊)
            string state = Guid.NewGuid().ToString();
            //HttpContext.Current.Session["state"] = state; //利用TempData被取出資料後即消失的特性，來防禦CSRF攻擊
            //如果是ASP.net Form，就改成放入Session或Cookie，之後取出資料時再把Session或Cookie設為null刪除資料
            // &prompt=consent 每次登入都要同意
            //nonce="xxx" 防止重複攻擊 通關密語(驗證用)
            string LineLoginUrl = $@"https://access.line.me/oauth2/v2.1/authorize?response_type=code&prompt=consent&nonce={client_nonce}&client_id={client_id}&redirect_uri={redirect_uri}&state={state}&bot_prompt={bot_prompt}&scope={HttpUtility.UrlEncode("openid profile email")}";

            //沒綁官方帳號
            //string LineLoginUrl = $@"https://access.line.me/oauth2/v2.1/authorize?response_type=code&prompt=consent&nonce={client_nonce}&client_id={client_id}&redirect_uri={redirect_uri}&state={state}&scope={HttpUtility.UrlEncode("openid profile email")}";

            return Ok(LineLoginUrl);
        }

        // GET api/Linelogin/GetLineInfo
        public IHttpActionResult GetLineInfo(string friendship_status_changed, string code, string state)
        {
            //     if (HttpContext.Current.Session["state"].ToString() == "")
            if (state == "")
            {
                //可能使用者停留Line登入頁面太久
                return Ok(new
                {
                    check = "no",
                    message = message.line頁面逾期
                });
            }

            #region 第一支Api   id_token/access_token

            WebRequest request = WebRequest.Create("https://api.line.me/oauth2/v2.1/token");

            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("grant_type", "authorization_code");
            postParams.Add("code", code);
            postParams.Add("redirect_uri", this.redirect_uri);
            postParams.Add("client_id", this.client_id);
            postParams.Add("client_secret", this.client_secret);

            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                LineLoginToken tokenObj = JsonConvert.DeserializeObject<LineLoginToken>(responseFromServer);
                //重點 拿掉Token 可以給前端
                id_token = tokenObj.id_token;
                access_token = tokenObj.access_token;
            }
            // Close the response.
            response.Close();

            #endregion 第一支Api   id_token/access_token

            #region 第二支Api個人userId/displayName/pictureUrl/statusMessage

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.line.me/v2/profile");
            //req.Headers.Add("Authorization", "Bearer " + access_token);
            //req.Method = "GET";
            ////API回傳的字串
            //string resStr = "";
            ////發出Request
            //using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            //{
            //    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            //    {
            //        resStr = sr.ReadToEnd();
            //    }//end using
            //}
            //LineUserProfile userProfile = JsonConvert.DeserializeObject<LineUserProfile>(resStr);
            //userId = userProfile.userId;

            #endregion 第二支Api個人userId/displayName/pictureUrl/statusMessage

            #region 第四支 LINE Login API 取得好友關係

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.line.me/friendship/v1/status");
            req.Headers.Add("Authorization", "Bearer " + access_token);
            req.Method = "GET";
            //API回傳的字串
            string resStr = "";
            //發出Request
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    resStr = sr.ReadToEnd();
                }//end using
            }
            object friends = JsonConvert.DeserializeObject<object>(resStr);

            #endregion 第四支 LINE Login API 取得好友關係

            //}
            //註銷Session
            //HttpContext.Current.Session["state"] = "";
            return Ok(new
            {
                id_token,
                access_token,
                friend = friends
            });
        }

        // GET api/Linelogin/GetLineInfoError
        public IHttpActionResult GetLineInfoError()
        {
            //用戶沒授權你的LineApp
            return Ok(new
            {
                check = "no",
                message = message.line用戶沒授權你的LineApp
            });
        }

        //  POST api/Linelogin/PostLinePayload
        public IHttpActionResult PostLinePayload(string id_token)
        {
            string Name;
            string Email;
            string Userid;
            string ImgName;

            #region 第三支Api 存資料庫

            //財政部電子發票API的Url
            string url = "https://api.line.me/oauth2/v2.1/verify";

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
            request2.Method = "POST";
            request2.ContentType = "application/x-www-form-urlencoded";

            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams2 = System.Web.HttpUtility.ParseQueryString(string.Empty);
            postParams2.Add("id_token", id_token);
            postParams2.Add("client_id", client_id);

            byte[] byteArray2 = Encoding.UTF8.GetBytes(postParams2.ToString());
            // Set the ContentLength property of the WebRequest.
            request2.ContentLength = byteArray2.Length;
            // Get the request stream.
            Stream dataStream2 = request2.GetRequestStream();
            // Write the data to the request stream.
            dataStream2.Write(byteArray2, 0, byteArray2.Length);
            // Close the Stream object.
            dataStream2.Close();

            // Get the response.
            WebResponse response = request2.GetResponse();

            // The using block ensures the stream is automatically closed.
            using (dataStream2 = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream2);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                LineProfile userProfile2 = JsonConvert.DeserializeObject<LineProfile>(responseFromServer);
                Userid = userProfile2.sub;
                Email = userProfile2.email;
                Name = userProfile2.name;
                ImgName = userProfile2.picture;
            }
            // Close the response.
            response.Close();

            #endregion 第三支Api 存資料庫

            //防呆 如果LINE用戶已經有登入過並儲存資料庫直接回應

            var lineUserId = db.Users.FirstOrDefault(x => x.UserId == Userid);

            var linetoken = "";
            if (lineUserId != null)
            {
                var jwtAuth = new JwtAuthUtil();
                linetoken = jwtAuth.GenerateToken(lineUserId.UserId, lineUserId.Authority);
                return Ok(new
                {
                    check = "ok",
                    message = message.LINE登入成功,
                    Name,
                    ImgName,
                    Email,
                    linetoken
                });
            }

            //存資料庫
            var user = new User
            {
                UserId = Userid,
                Name = Name,
                ImgName = ImgName,
                Email = Email,
                Authority = 1,
                LINE = true,
                AddTime = DateTime.Now,
                CheckAccount = 1
            };
            db.Users.Add(user);
            db.SaveChanges();

            var jwtAuth2 = new JwtAuthUtil();
            linetoken = jwtAuth2.GenerateToken(user.UserId, user.Authority);
            return Ok(new
            {
                check = "ok",
                message = message.LINE登入成功,
                user.Name,
                user.ImgName,
                user.Email,
                linetoken
            });
        }

        //  POST api/Linelogin/RevokeLineLoginUrl
        //徹銷Line Login，登出
        [HttpPost]
        public IHttpActionResult RevokeLineLoginUrl(string access_token)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.line.me/oauth2/v2.1/revoke");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("access_token", access_token);
            postParams.Add("client_id", this.client_id);
            postParams.Add("client_secret", this.client_secret);

            //要發送的字串轉為byte[]
            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }//end using

            //API回傳的字串
            string responseStr = "";
            //發出Request
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                }//end using
            }

            return Ok(responseStr);
        }
    }
}