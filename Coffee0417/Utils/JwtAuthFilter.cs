using Coffee0417.Models;
using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Coffee0417.Utils
{
    public class JwtAuthFilter : ActionFilterAttribute
    {
        //解密
        private const string secret = "coffeeDemo";//加解密的key,如果不一樣會無法成功解密

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            //if (!WithoutVerifyToken(request.RequestUri.ToString()))
            //{
            if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
            {
                var errorMessage = new HttpResponseMessage()
                {
                    ReasonPhrase = "Lost Token",
                    Content = new StringContent(" code=5566"),
                };
                throw new HttpResponseException(errorMessage);
            }
            else
            {
                try
                {
                    //解密=====會回傳Json格式的物件(即加密前的資料) token 解密成payload
                    var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
                        request.Headers.Authorization.Parameter,
                        Encoding.UTF8.GetBytes(secret),
                        JwsAlgorithm.HS512);
                    if (IsTokenExpired(jwtObject["Exp"].ToString()))
                    {
                        //固定寫法
                        var errorMessage = new HttpResponseMessage()
                        {
                            ReasonPhrase = "Token Expired",
                            Content = new StringContent(" code=5566"),
                        };
                        throw new HttpResponseException(errorMessage);
                    }
                }
                catch (Exception e)
                {
                    var errorMessage = new HttpResponseMessage()
                    {
                        ReasonPhrase = "Lost Token",
                        Content = new StringContent($" code=5566 發生錯誤:{e}"),
                    };
                    throw new HttpResponseException(errorMessage);
                }
            }
            base.OnActionExecuting(actionContext);
        }

        //Login不需要驗證因為還沒有token 電商不適用              適合後台 全部驗證
        //public bool WithoutVerifyToken(string requestUri)
        //{
        //    if (requestUri.EndsWith("/Login"))
        //        return true;
        //    return false;
        //}
        public static Dictionary<string, object> GetToken(string token)
        {
            return JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);
        }

        public static string GetTokenId(string token)
        {
            var tokenValue = JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);
            return tokenValue["userid"].ToString();
        }

        public static int GetTokenPermission(string token)
        {
            var tokenValue = JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);
            return (int)tokenValue["Permission"];
        }

        //驗證token時效
        public bool IsTokenExpired(string dateTime)
        {
            return Convert.ToDateTime(dateTime) < DateTime.Now;
        }
    }
}