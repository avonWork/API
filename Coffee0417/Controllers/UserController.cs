using Coffee0417.Models;

using Coffee0417.Models;

using Coffee0417.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using static Coffee0417.Utils.Enum;
using static Coffee0417.Utils.Myall;

namespace Coffee0417.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UserController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/User/Getuserdata
        //拿token取得會員資料(是否有權限)
        [HttpGet]
        [JwtAuthFilter]
        public IHttpActionResult Getuserdata()

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
            return Ok(new
            {
                check = "ok",
                message = message.權限驗證成功,
                userdata.Email,
                userdata.Name,
                userdata.Phone,
                userdata.ImgName
            });
        }

        // POST: api/User/PostUser
        //會員註冊
        //防呆機制:Email格式/手機格式/會員帳號有沒有人使用
        //ok
        [HttpPost]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (getemail(user.Email) == false)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.Email格式不正確
                });
            }

            if (getphone(user.Phone) == false)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.手機格式不正確
                });
            }

            var result = db.Users.FirstOrDefault(x => x.Email == user.Email);
            if (result == null)
            {
                user.PasswordSalt = Salt.CreateSalt();
                user.Password = Salt.GenerateHashWithSalt(user.Password, user.PasswordSalt);
                //使用者權限(結帳按鈕後面所有頁面)
                string urlSetting = ConfigurationSettings.AppSettings["Url"];
                user.ImgName = urlSetting+"/img/clipart4058783.png";
                user.Authority = 1;
                user.UserId = Guid.NewGuid().ToString();
                user.AddTime = DateTime.Now;
                db.Users.Add(user);
                //寄信--會員帳號開通成功 更新資料庫
                //string bodycontent = $"<p>謝謝你的註冊,請點擊以下連結:<p><br/><a href='https://localhost:44304/api/PutAccount?userid={user.UserId}'>開通你的帳號</a>";

                string bodycontent = $"<p>謝謝你的註冊,請點擊以下連結:<p><br/><a href='https://localhost:8080/api/PutAccount?userid=" + user.UserId + "'>開通你的帳號</a>";
                string title = "vacationaroma會員帳號開通通知:";
                SendEmail(user.Email, bodycontent, title);

                db.SaveChanges();
                return Ok(new
                {
                    check = "ok",
                    message = message.會員帳號註冊成功
                });
            }

            return Ok(new
            {
                check = "no",
                message = message.會員帳號已經有人使用
            });
        }

        // POST: api/Login
        //前台登入
        //ok
        [HttpPost]
        [Route("api/Login")]
        public IHttpActionResult Login([FromBody] Login login)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == login.Email);

            if (user == null) return Ok(new
            {
                check = "no",
                message = message.會員帳號錯誤
            });
            if (user.CheckAccount == 0) return Ok(new
            {
                check = "no",
                message = message.會員帳號尚未開通
            });

            //防呆--如果沒有會員註冊(LINE登入)
            if (user.LINE) return Ok(new
            {
                check = "no",
                message = message.請改用LINE重新登入
            });

            string pwd = Salt.GenerateHashWithSalt(login.Password, user.PasswordSalt);
            if (user.Password != pwd) return Ok(new
            {
                check = "no",
                message = message.會員密碼錯誤
            });

            var jwtAuth = new JwtAuthUtil();
            var mytoken = jwtAuth.GenerateToken(user.UserId, user.Authority);

            return Ok(new
            {
                check = "ok",
                message = message.會員登入成功,
                mytoken
            });
        }

        //會員忘記密碼
        //寄信-新密碼
        [HttpPut]
        [Route("api/ForgotPassword")]
        public IHttpActionResult ForgotPassword([FromBody] User user)
        {
            var result = db.Users.FirstOrDefault(x => x.Email == user.Email);

            if (result == null) return Ok(new
            {
                check = "no",
                message = message.尚未註冊
            });

            if (result.CheckAccount == 0) return Ok(new
            {
                check = "no",
                message = message.會員帳號尚未開通
            });

            //防呆--如果沒有會員註冊(LINE登入)
            if (user.LINE) return Ok(new
            {
                check = "no",
                message = message.尚未註冊
            });
            //暫時密碼
            result.PasswordSalt = Salt.CreateSalt();
            string newPassword = Guid.NewGuid().ToString();
            result.Password = Salt.GenerateHashWithSalt(newPassword, result.PasswordSalt);
            db.SaveChanges();
            //網址
            string urlSetting = ConfigurationSettings.AppSettings["Url"];
            string bodycontent = $"<p>新密碼:" + newPassword + "<p><p>請回到登入重新登入新密碼<p><br/><a href='" + urlSetting + "/login.html'>登入</a>";
            string title = "vacationaroma會員成功申請新密碼通知:";
            SendEmail(user.Email, bodycontent, title);

            return Ok(new
            {
                check = "ok",
                message = message.新密碼寄信成功
            });
        }

        //前台-會員修改密碼
        [HttpPut]
        [JwtAuthFilter]
        [Route("api/ResetPassword")]
        public IHttpActionResult ResetPassword([FromBody] Login login)
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
                message = message.權限驗證不符
            });
            var result = db.Users.FirstOrDefault(x => x.UserId == userid);

            if (result.CheckAccount == 0) return Ok(new
            {
                check = "no",
                message = message.會員帳號尚未開通
            });

            //防呆--如果沒有會員註冊(LINE登入)
            if (result.LINE) return Ok(new
            {
                check = "no",
                message = message.尚未註冊
            });

            //舊密碼驗證
            string pwd = Salt.GenerateHashWithSalt(login.OldPassword, result.PasswordSalt);
            var meUser = db.Users.FirstOrDefault(x => x.Password == pwd);
            if (meUser == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.會員密碼錯誤
                });
            }
            //新密碼
            meUser.PasswordSalt = Salt.CreateSalt();
            meUser.Password = Salt.GenerateHashWithSalt(login.Password, meUser.PasswordSalt);
            db.SaveChanges();
            //網址
            string urlSetting = ConfigurationSettings.AppSettings["Url"];
            string bodycontent = $"<p>請回到登入重新登入新密碼<p><br/><a href='" + urlSetting + "/login.html'>登入</a>";
            string title = "vacationaroma會員密碼修改成功通知:";
            SendEmail(meUser.Email, bodycontent, title);

            return Ok(new
            {
                check = "ok",
                message = message.會員密碼修改成功
            });
        }

        // POST: api/User/PostAdmin
        //管理者註冊
        //ok
        [HttpPost]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostAdmin([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = db.Users.FirstOrDefault(x => x.Account == user.Account);
            if (result == null)
            {
                user.PasswordSalt = Salt.CreateSalt();
                user.Password = Salt.GenerateHashWithSalt(user.Password, user.PasswordSalt);
                //使用者權限(結帳按鈕後面所有頁面)
                string urlSetting = ConfigurationSettings.AppSettings["Url"];
                user.ImgName = urlSetting+"/img/全莊園豆.jpg";
                user.Name = "管理者";
                user.Authority = 2;
                user.UserId = Guid.NewGuid().ToString();
                user.AddTime = DateTime.Now;
                db.Users.Add(user);

                db.SaveChanges();
                return Ok(new
                {
                    check = "ok",
                    message = message.管理者註冊成功
                });
            }

            return Ok(new
            {
                check = "no",
                message = message.管理者帳號已經有人使用
            });
        }

        // POST: api/BkLogin
        //後台登入
        //ok
        [HttpPost]
        [Route("api/BkLogin")]
        public IHttpActionResult BkLogin([FromBody] Login login)
        {
            var user = db.Users.FirstOrDefault(x => x.Account == login.Account);
            if (user == null) return Ok(new
            {
                check = "no",
                message = message.管理者帳號錯誤
            });

            string pwd = Salt.GenerateHashWithSalt(login.Password, user.PasswordSalt);
            if (user.Password != pwd) return Ok(new
            {
                check = "no",
                message = message.管理者密碼錯誤
            });

            var jwtAuth = new JwtAuthUtil();
            var mytoken = jwtAuth.GenerateToken(user.UserId, user.Authority);

            return Ok(new
            {
                check = "ok",
                message = message.管理者登入成功,
                mytoken
            });
        }

        // PUT: api/PutAccount?userid=x
        //會員帳號開通
        [HttpPut]
        [AllowAnonymous]
        [AcceptVerbs("GET", "Put")]
        [Route("api/PutAccount")]
        public HttpResponseMessage PutAccount(string userid)
        {
            var userdata = db.Users.FirstOrDefault(x => x.UserId == userid);
            userdata.CheckAccount = 1;
            db.SaveChanges();
            //網址
            string urlSetting = ConfigurationSettings.AppSettings["Url"];
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(urlSetting + "/login.html");
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}