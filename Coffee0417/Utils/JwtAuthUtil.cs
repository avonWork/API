using Coffee0417.Models;
using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Coffee0417.Utils
{
    public class JwtAuthUtil
    {
        //加密
        public string GenerateToken(string userid, int permission)
        {
            string secret = "coffeeDemo"; //加解密的key,如果不一樣會無法成功解密
            Dictionary<string, Object> claim = new Dictionary<string, Object>(); //payload 需透過token傳遞的資料
            claim.Add("userid", userid);
            claim.Add("Permission", permission);
            claim.Add("iat", DateTime.Now.ToString());
            claim.Add("Exp", DateTime.Now.AddSeconds(Convert.ToInt32("86400")).ToString()); //Token 時效設定100秒
            var payload = claim;
            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512); //產生token
            return token;
        }
    }
}