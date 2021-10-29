using Coffee0417.Models;
using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Coffee0417.Controllers
{
    public class LineBotController : ApiController
    {
        private Model1 db = new Model1();

        [Route("api/LineBot2")]
        [HttpPost]
        public IHttpActionResult POST2()
        {
            //設定你的Channel Access Token
            string ChannelAccessToken =
                "36bArAQhVLM/kuvR5Hz3tR6re2E3PuRbaMMUp43r4Mhy+B9Ct0Jbz7FWR3S7+cmceFWDOYdT8+1FbHYpXZV9kPelPOMQws5vjtH5WgSxcPZjSD3ip8+oXF1D9iaz6iXpplnwh+Inw5nTPiYJ+5BYOwdB04t89/1O/w1cDnyilFU=";
            isRock.LineBot.Bot bot = new isRock.LineBot.Bot(ChannelAccessToken);

            //如果有Web.Config app setting，以此優先
            //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("LineChannelAccessToken"))
            //{
            //    ChannelAccessToken = System.Configuration.ConfigurationManager.AppSettings["LineChannelAccessToken"];
            //}

            //create bot instance

            try
            {
                //取得 http Post RawData(should be JSON)
                string postData = Request.Content.ReadAsStringAsync().Result;
                //剖析JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);
                var UserSays = ReceivedMessage.events[0].message.text;
                var ReplyToken = ReceivedMessage.events[0].replyToken;
                //依照用戶說的特定關鍵字來回應
                switch (UserSays.ToLower())
                {
                    case "/teststicker":
                        //回覆貼圖
                        bot.ReplyMessage(ReplyToken, 1, 1);
                        break;

                    case "/testimage":
                        //回覆圖片
                        bot.ReplyMessage(ReplyToken,
                            new Uri(
                                "https://scontent-tpe1-1.xx.fbcdn.net/v/t31.0-8/15800635_1324407647598805_917901174271992826_o.jpg?oh=2fe14b080454b33be59cdfea8245406d&oe=591D5C94"));
                        break;

                    default:
                        //回覆訊息
                        string Message = "哈囉, 你說了:" + UserSays;
                        //回覆用戶
                        bot.ReplyMessage(ReplyToken, Message);
                        break;
                }

                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }

        [Route("api/LineBot3")]
        [HttpPost]
        public IHttpActionResult POST3()
        {
            string ChannelAccessToken =
                "QxJmdCyvk2QOgioIuNBPSma0pDy5u6M8BjmdTTvK+66B8lv+R8sX1OyidrWP/99QkRLZBtPK2TeR5d0EaQji9FTW+dq7/zhklw/UzMAzZPoTNOk3FTLISvwaX260YKyy037PJF1MYs2ok8yz3ezNywdB04t89/1O/w1cDnyilFU=";
            //TODO: 上面請改成自己的token;

            //取得 http Post RawData(should be JSON)
            string postData = Request.Content.ReadAsStringAsync().Result;
            //剖析JSON
            var ReceivedMessage = Utility.Parsing(postData);

            try
            {
                //回覆訊息
                string Message;
                Message = "收到 event 類型為 :  " + ReceivedMessage.events[0].type;
                if (ReceivedMessage.events[0].type == "message")
                {
                    Message += "\n 你傳來的訊息類型為: " + ReceivedMessage.events[0].message.type;
                }

                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(
                    ReceivedMessage.events[0].replyToken, Message, ChannelAccessToken);
                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                //請自行處理Exception
                isRock.LineBot.Utility.ReplyMessage(
                    ReceivedMessage.events[0].replyToken, "ERROR:" + ex.Message, ChannelAccessToken);

                return Ok();
            }
        }

        //正在使用中
        [Route("api/LineBot")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            string ChannelAccessToken = ConfigurationManager.AppSettings["ChannelAccessToken"];
            //取得 http Post RawData(should be JSON)
            string postData = Request.Content.ReadAsStringAsync().Result;
            //剖析JSON
            var ReceivedMessage = Utility.Parsing(postData);
            var LineEvent = ReceivedMessage.events.FirstOrDefault();
            var userid = ReceivedMessage.events.FirstOrDefault().source.userId;
            var botOrder = db.Orders.Where(x => x.UserId == userid).OrderByDescending(y => y.Id).FirstOrDefault();
            var pro = db.Products.OrderBy(_ => Guid.NewGuid()).Take(1).FirstOrDefault();

            #region Buttons Template Message

            ////建立一個Buttons Template Message物件
            //var ButtonsTemplateMsg = new isRock.LineBot.ButtonsTemplate();
            ////設定thumbnailImageUrl
            //ButtonsTemplateMsg.altText = "無法顯示時的替代文字";
            //ButtonsTemplateMsg.thumbnailImageUrl = new Uri("https://arock.blob.core.windows.net/blogdata201709/14-143030-1cd8cf1e-8f77-4652-9afa-605d27f20933.png");
            //ButtonsTemplateMsg.text = "請問您想購買哪一類的服飾?";
            //ButtonsTemplateMsg.title = "詢問"; //標題
            ////建立actions
            //var actions = new List<isRock.LineBot.TemplateActionBase>();
            //actions.Add(new isRock.LineBot.MessageAction() { label = "男裝", text = "man" });
            //actions.Add(new isRock.LineBot.MessageAction() { label = "女裝", text = "women" });
            //actions.Add(new isRock.LineBot.MessageAction() { label = "童裝", text = "children" });
            ////將建立好的actions選項加入
            //ButtonsTemplateMsg.actions = actions;
            ////建立bot instance
            //isRock.LineBot.Bot bot = new isRock.LineBot.Bot(ChannelAccessToken);
            ////send ButtonsTemplateMsg
            //bot.PushMessage(userid, ButtonsTemplateMsg);

            #endregion Buttons Template Message

            //如果不是訊息
            if (LineEvent.type != "message") return Ok();
            //依照訊息類型處理
            switch (LineEvent.message.type.ToLower())
            {
                case "text":
                    //回覆文字訊息
                    if (LineEvent.message.text == "最新訂單")
                    {
                        if (botOrder == null)
                        {
                            Utility.ReplyMessage(LineEvent.replyToken,
                                $"你尚未購買商品!!\n可以按 [門市資訊] 至本網站購買", ChannelAccessToken);
                        }
                        else
                        {
                            Utility.ReplyMessage(LineEvent.replyToken,
                                $"訂單id: {botOrder.Id}\n訂單狀態: {botOrder.Status}\n訂單金額: {botOrder.SubTotal}", ChannelAccessToken);
                        }
                    }
                    else if (LineEvent.message.text == "推薦商品")
                    {
                        Utility.ReplyMessage(LineEvent.replyToken,
                            $"產品名稱: {pro.ProductName}\n產品味道: {pro.ProductDescription}\n產品金額: {pro.ProducPrice}", ChannelAccessToken);
                    }
                    else
                    {
                        Utility.ReplyMessage(LineEvent.replyToken,
                            $"測試訊息: {LineEvent.message.text}", ChannelAccessToken);
                    }
                    break;

                default:
                    Utility.ReplyMessage(LineEvent.replyToken,
                        $"只接收文字格式!!\n不接受圖片/影音/其他格式", ChannelAccessToken);
                    break;
            }
            return Ok();
        }
    }
}