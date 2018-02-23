using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinDeveloper.ApiClient.OKExApi
{
    class ErrCodeMsg
    {
        public string ErrorCode { get; set; }
        public string Msg { get; set; }
    }
    class ErrCode
    {
        public static string Error_codeVal(string Error_code)
        {
            if (Error_code == "10000") { return "必選參數不能為空"; }
            if (Error_code == "10001") { return "用戶請求頻率過快，超過該接口允許的限額"; }
            if (Error_code == "10002") { return "系統錯誤"; }
            if (Error_code == "10004") { return "請求失敗"; }
            if (Error_code == "10005") { return "SecretKey不存在"; }
            if (Error_code == "10006") { return "Api_key不存在"; }
            if (Error_code == "10007") { return "簽名不匹配"; }
            if (Error_code == "10008") { return "非法參數"; }
            if (Error_code == "10009") { return "訂單不存在"; }
            if (Error_code == "10010") { return "餘額不足"; }
            if (Error_code == "10011") { return "買賣的數量小於BTC/LTC最小買賣額度"; }
            if (Error_code == "10012") { return "當前網站暫時只支持btc_usdltc_usd"; }
            if (Error_code == "10013") { return "此接口只支持https請求"; }
            if (Error_code == "10014") { return "下單價格不得≤0或≥1000000"; }
            if (Error_code == "10015") { return "下單價格與最新成交價偏差過大"; }
            if (Error_code == "10016") { return "幣數量不足"; }
            if (Error_code == "10017") { return "API鑑權失敗"; }
            if (Error_code == "10018") { return "借入不能小於最低限額[usd:100,btc:0.1,ltc:1]"; }
            if (Error_code == "10019") { return "頁面沒有同意借貸協議"; }
            if (Error_code == "10020") { return "費率不能大於1%"; }
            if (Error_code == "10021") { return "費率不能小於0.01%"; }
            if (Error_code == "10023") { return "獲取最新成交價錯誤"; }
            if (Error_code == "10024") { return "可藉金額不足"; }
            if (Error_code == "10025") { return "額度已滿，暫時無法借款"; }
            if (Error_code == "10026") { return "借款(含預約借款)及保證金部分不能提出"; }
            if (Error_code == "10027") { return "修改敏感提幣驗證信息，24小時內不允許提現"; }
            if (Error_code == "10028") { return "提幣金額已超過今日提幣限額"; }
            if (Error_code == "10029") { return "賬戶有借款，請撤消借款或者還清借款後提幣"; }
            if (Error_code == "10031") { return "存在BTC/LTC充值，該部分等值金額需6個網絡確認後方能提出"; }
            if (Error_code == "10032") { return "未綁定手機或谷歌驗證"; }
            if (Error_code == "10033") { return "服務費大於最大網絡手續費"; }
            if (Error_code == "10034") { return "服務費小於最低網絡手續費"; }
            if (Error_code == "10035") { return "可用BTC/LTC不足"; }
            if (Error_code == "10036") { return "提幣數量小於最小提幣數量"; }
            if (Error_code == "10037") { return "交易密碼未設置"; }
            if (Error_code == "10040") { return "取消提幣失敗"; }
            if (Error_code == "10041") { return "提幣地址不存在或未認證"; }
            if (Error_code == "10042") { return "交易密碼錯誤"; }
            if (Error_code == "10043") { return "合約權益錯誤，提幣失敗"; }
            if (Error_code == "10044") { return "取消借款失敗"; }
            if (Error_code == "10047") { return "當前為子賬戶，此功能未開放"; }
            if (Error_code == "10048") { return "提幣信息不存在"; }
            if (Error_code == "10049") { return "小額委託（<0.15BTC)的未成交委託數量不得大於50個"; }
            if (Error_code == "10050") { return "重複撤單"; }
            if (Error_code == "10052") { return "提幣受限"; }
            if (Error_code == "10064") { return "美元充值後的48小時內，該部分資產不能提出"; }
            if (Error_code == "10100") { return "賬戶被凍結"; }
            if (Error_code == "10101") { return "訂單類型錯誤"; }
            if (Error_code == "10102") { return "不是本用戶的訂單"; }
            if (Error_code == "10103") { return "私密訂單密鑰錯誤"; }
            if (Error_code == "10216") { return "非開放API"; }
            if (Error_code == "1002") { return "交易金額大於餘額"; }
            if (Error_code == "1003") { return "交易金額小於最小交易值"; }
            if (Error_code == "1004") { return "交易金額小於0"; }
            if (Error_code == "1007") { return "沒有交易市場信息"; }
            if (Error_code == "1008") { return "沒有最新行情信息"; }
            if (Error_code == "1009") { return "沒有訂單"; }
            if (Error_code == "1010") { return "撤銷訂單與原訂單用戶不一致"; }
            if (Error_code == "1011") { return "沒有查詢到該用戶"; }
            if (Error_code == "1013") { return "沒有訂單類型"; }
            if (Error_code == "1014") { return "沒有登錄"; }
            if (Error_code == "1015") { return "沒有獲取到行情深度信息"; }
            if (Error_code == "1017") { return "日期參數錯誤"; }
            if (Error_code == "1018") { return "下單失敗"; }
            if (Error_code == "1019") { return "撤銷訂單失敗"; }
            if (Error_code == "1024") { return "幣種不存在"; }
            if (Error_code == "1025") { return "沒有K線類型"; }
            if (Error_code == "1026") { return "沒有基準幣數量"; }
            if (Error_code == "1027") { return "參數不合法可能超出限制"; }
            if (Error_code == "1028") { return "保留小數位失敗"; }
            if (Error_code == "1029") { return "正在準備中"; }
            if (Error_code == "1030") { return "有融資融幣無法進行交易"; }
            if (Error_code == "1031") { return "轉賬餘額不足"; }
            if (Error_code == "1032") { return "該幣種不能轉賬"; }
            if (Error_code == "1035") { return "密碼不合法"; }
            if (Error_code == "1036") { return "谷歌驗證碼不合法"; }
            if (Error_code == "1037") { return "谷歌驗證碼不正確"; }
            if (Error_code == "1038") { return "谷歌驗證碼重複使用"; }
            if (Error_code == "1039") { return "短信驗證碼輸錯限制"; }
            if (Error_code == "1040") { return "短信驗證碼不合法"; }
            if (Error_code == "1041") { return "短信驗證碼不正確"; }
            if (Error_code == "1042") { return "谷歌驗證碼輸錯限制"; }
            if (Error_code == "1043") { return "登陸密碼不允許與交易密碼一致"; }
            if (Error_code == "1044") { return "原密碼錯誤"; }
            if (Error_code == "1045") { return "未設置二次驗證"; }
            if (Error_code == "1046") { return "原密碼未輸入"; }
            if (Error_code == "1048") { return "用戶被凍結"; }
            if (Error_code == "1050") { return "訂單已撤銷或者撤單中"; }
            if (Error_code == "1051") { return "訂單已完成交易"; }
            if (Error_code == "1201") { return "賬號零時刪除"; }
            if (Error_code == "1202") { return "賬號不存在"; }
            if (Error_code == "1203") { return "轉賬金額大於餘額"; }
            if (Error_code == "1204") { return "不同種幣種不能轉賬"; }
            if (Error_code == "1205") { return "賬號不存在主從關係"; }
            if (Error_code == "1206") { return "提現用戶被凍結"; }
            if (Error_code == "1207") { return "不支持轉賬"; }
            if (Error_code == "1208") { return "沒有該轉賬用戶"; }
            if (Error_code == "1209") { return "當前api不可用"; }
            if (Error_code == "1216") { return "市價交易暫停，請選擇限價交易"; }
            if (Error_code == "1217") { return "您的委託價格超過最新成交價的±5%，存在風險，請重新下單"; }
            if (Error_code == "1218") { return "下單失敗，請稍後再試"; }
            return "無對應參數";
        }
    }
}
