using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.Utils
{
    public class Enum
    {
        public enum OrderStatus
        {
            未完成交易=0,
            處理中 = 1,
            已出貨 = 2,
            已取貨 = 3,
            已取消 = 4,
        }

        public enum Payment
        {
            LINEPay = 1,
            門市取貨 = 2,
        }

        public enum Notice
        {
            LINE通知 = 1,
            Email通知 = 2,
        }

        public enum message
        {
            前台後台ProTotal不符 = 1,
            前台後台SubTotal不符 = 2,
            會員帳號錯誤 = 3,
            會員密碼錯誤 = 4,
            會員帳號已經有人使用 = 5,
            會員帳號註冊成功 = 6,
            會員登入成功 = 7,
            管理者登入成功 = 8,
            管理者帳號錯誤 = 9,
            管理者密碼錯誤 = 10,
            管理者註冊成功 = 11,
            管理者帳號已經有人使用 = 12,
            權限驗證不符 = 13,
            尚未註冊 = 14,
            權限驗證成功 = 13,
            LINE登入成功 = 14,
            Email格式不正確 = 15,
            手機格式不正確 = 16,
            產品id錯誤 = 17,
            取得產品內容 = 18,
            產品搜尋無資料 = 19,
            取得產品列表 = 20,
            linepay訂單狀態修改成功 = 21,
            linepay訂單狀態修改失敗 = 22,
            linepay付款成功 = 23,
            linepay付款失敗 = 24,
            取得全部訂單 = 25,
            無符合訂單 = 26,
            取得符合訂單 = 27,
            請改用LINE重新登入 = 28,
            linepay付款網址成功 = 29,
            linepay付款網址不成功 = 30,
            訂單資料存進資料庫 = 31,
            尚未有訂單 = 32,
            line用戶沒授權你的LineApp = 33,
            line頁面逾期 = 34,
            state驗證失敗 = 35,
            會員帳號尚未開通 = 36,
            取得訂單收件人資料 = 37,
            新密碼寄信成功 = 38,
            會員密碼修改成功 = 39,
            商品狀態修改成功 = 40,
            收件人姓名為空值 = 41,
            收件人手機為空值 = 42,
            收件人地址為空值 = 43,
            付款方式未選擇 = 44
        }
    }
}