using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Coffee0417.Utils
{
    public class Myall
    {
        //Email格式驗證
        public static bool getemail(string s)
        {
            //1@y.a
            string pattern = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            bool result = Regex.IsMatch(s, pattern);
            return result;
        }

        //手機格式驗證
        public static bool getphone(string s)
        {
            string pattern = @"^09\d{2}(\d{6}|-?\d{3}-?\d{3})$";
            bool result = Regex.IsMatch(s, pattern);
            return result;
        }

        //寄信
        public static void SendEmail(string receiveEmail, string bodycontent, string title)
        {
            //設定smtp主機
            string smtpAddress = "smtp.gmail.com";
            //設定Port
            int portNumber = 587;
            bool enableSSL = true;
            //填入寄送方email和密碼
            string emailFrom = "avonworktest@gmail.com";
            string password = "avon201012";
            //收信方email 可以用逗號區分多個收件人
            string emailTo = receiveEmail;
            //主旨
            string filetime = DateTime.Now.ToString("yyyy.MM.dd");
            string subject = filetime + title;
            //內容
            string body = bodycontent;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail); //如果有錯記得打開低安全
                }
            }
        }
    }
}