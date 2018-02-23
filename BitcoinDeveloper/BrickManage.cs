using BitcoinDeveloper.Models;
using BitcoinService.ApiClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinService
{
    class BrickManage
    {
        private Execution Execution;
        private List<Account> AccountList;
        private EnumData.TradeCurrency Currency;
        private string ExecutionsId { get; set; }
        private string ExchangeType { get; set; }
        private string sUrl { get; set; }
        private int ProcessId { get; set; }
        public ExchangeApi API;
        private BitcoinDeveloper.ReturnMessageApi.Api1 ReturnMessageApi = new BitcoinDeveloper.ReturnMessageApi.Api1();
        /// <summary>
        /// 建立連線
        /// </summary>
        /// <param name="execution"></param>
        public BrickManage(Execution execution, string sExecutionsId, string Url, string sExchangeType,int iProcessId)
        {
            ProcessId = iProcessId;
            ExecutionsId = sExecutionsId;
            ExchangeType = sExchangeType;
            if (!string.IsNullOrWhiteSpace(Url)) { ReturnMessageApi.Url = Url; }
            Execution = execution;
            Currency = (EnumData.TradeCurrency)Enum.Parse(typeof(EnumData.TradeCurrency), Execution.CurrencyValue);
            AccountList = Execution.ExecutionsAccounts.Select(EA => EA.Account).ToList();
            API = new ExchangeApi();
            Console.WriteLine(string.Format("{0} - 已建立", Execution.Name));
        }
        /// <summary>
        /// 建立連結
        /// </summary>
        public void Initialize()
        {
            Console.WriteLine(string.Format("{0} - 初始化中…", Execution.Name));

            foreach (var Account in AccountList)
            {
                var Msg = API.ApiInit(Account, ExchangeType);
                if (!string.IsNullOrWhiteSpace(Msg))
                {
                    try
                    {
                        //回傳值
                        ReturnMessageApi.ReturnMessage(Msg, ExecutionsId, 0, ProcessId);
                    }
                    catch
                    {
                        Console.WriteLine("回傳連結錯誤");
                    }
                }

            }

            API.StartUp();

            Console.WriteLine(string.Format("{0} - 初始化結束", Execution.Name));
        }
        /// <summary>
        /// 比對
        /// </summary>
        public void MatchExchange()
        {
            Task.Factory.StartNew(() =>
            {
                while (API.InProgress)
                {
                    ExchangeData highestBid = API.ExchangeList.Select(e => e.Value).Where(e => e.Status.Equals(EnumData.ExchangeStatus.執行中)).OrderByDescending(e => e.Bid).First();
                    ExchangeData lowestAsk = API.ExchangeList.Select(e => e.Value).Where(e => e.Status.Equals(EnumData.ExchangeStatus.執行中) && !e.Name.Equals(highestBid.Name) && !e.Ask.Equals(0)).OrderBy(e => e.Ask).FirstOrDefault();
                    if (lowestAsk != null && highestBid != null)
                    {
                        var Askval = lowestAsk.Ask * (1 + (highestBid.Fee / 100));
                        var Bidval = highestBid.Bid * (1 - (highestBid.Fee / 100));
                        if (Askval < Bidval)
                        {
                            //獲利
                            var Profit = (Bidval * Execution.MinQuantity) - (Askval * Execution.MinQuantity);

                            string[] argsData = new string[] { 
                            Execution.Name,//執行序名稱
                            highestBid.Name, //最高賣價交易所
                            ((decimal)highestBid.Bid * (1 - ((decimal)highestBid.Fee / 100))).ToString(), //含手續費賣價
                            lowestAsk.Name, //最低買價交易所
                            ((decimal)lowestAsk.Ask * (1 + ((decimal)highestBid.Fee / 100))).ToString(),  //含手續費買價
                            DateTime.Now.ToString(), //時間
                            highestBid.Bid.ToString(), //不含手續費賣價
                            lowestAsk.Ask.ToString(),  //不含手續費買價
                            Profit.ToString(),  //獲利
                            Execution.MinQuantity.ToString()//交易數量
                            };
                            var msg = string.Format("{5} - {0} 配對完成： {1}的Bid - {2} 和 {3}的Ask - {4}   原Bid {6}   原Ask {7}   ;交易數量：{9}  ＞獲利 {8}", argsData);
                            Console.WriteLine(msg);
                            try
                            {
                                //回傳值
                                ReturnMessageApi.ReturnMessage(msg, ExecutionsId, 0, ProcessId);
                            }
                            catch
                            {
                                Console.WriteLine("回傳連結錯誤");
                            }
                            //交易
                            ExchangeCoin(highestBid, lowestAsk, AccountList, Profit, Execution.MinQuantity, Askval, Bidval);
                        }

                        else
                        {
                            var msg = string.Format("{1} - {0} 尚無成功配對", Execution.Name, DateTime.Now.ToString());
                            Console.WriteLine(msg);
                            try
                            {
                                ReturnMessageApi.ReturnMessage(msg, ExecutionsId, 0, ProcessId);
                            }
                            catch
                            {
                                Console.WriteLine("回傳連結錯誤");
                            }
                            Console.WriteLine(msg);
                        }

                    }
                    Thread.Sleep(1000);
                }
            });
        }  
        /// <summary>
        /// 交易
        /// </summary>
        /// <param name="highestBid">(賣價)最高資訊</param>
        /// <param name="lowestAsk">(買價)最低資訊</param>
        /// <param name="AccountList">交易所清單</param>
        /// <param name="Profit">獲利</param>
        private void ExchangeCoin(ExchangeData highestBid, ExchangeData lowestAsk, List<Account> AccountList, decimal Profit, decimal MinQuantity, decimal Askval, decimal Bidval)
        {
            using (BitcoinTransactionEntities db = new BitcoinTransactionEntities())
            {
                try
                {
                    var SaveLoop = true;
                    while (SaveLoop)
                    {
                        var Executions = db.Executions.Find(Execution.id);
                        if (Executions != null)
                        {
                            DateTime dt = DateTime.Now;
                            //if (string.IsNullOrWhiteSpace(Execution.ExchangeType))
                            //{
                            //    Execution.ExchangeType = "USDT-BTC";
                            //}
                            //新增記錄
                            //var Askval = (double)((decimal)lowestAsk.Ask * (1 + ((decimal)highestBid.Fee / 100)));
                            //var Bidval = (double)((decimal)highestBid.Bid * (1 - ((decimal)highestBid.Fee / 100)));
                            var TransactionRecord = new TransactionRecord { id = Guid.NewGuid(), Quantity = Execution.MinQuantity, Createdt = dt, Profit = Profit, ExchangeType = Execution.ExchangeType };
                            Executions.TransactionRecords.Add(TransactionRecord);
                            var AccountIdASK = Execution.ExecutionsAccounts.Where(x => x.Account.Exchange.Name.ToLower() == lowestAsk.Name.ToLower()).FirstOrDefault().AccountId;
                            var AccountIdBID = Execution.ExecutionsAccounts.Where(x => x.Account.Exchange.Name.ToLower() == highestBid.Name.ToLower()).FirstOrDefault().AccountId;
                            var TransactionConfirmedASK = new TransactionConfirmed { id = Guid.NewGuid(), AccountId = AccountIdASK, Prices = lowestAsk.Ask, ProcessingPrices = Askval, ProcessingFee = lowestAsk.Fee, Createdt = dt, TransactionType = 0, Status = 1 };
                            var TransactionConfirmedBID = new TransactionConfirmed { id = Guid.NewGuid(), AccountId = AccountIdBID, Prices = highestBid.Bid, ProcessingPrices = Bidval, ProcessingFee = highestBid.Fee, Createdt = dt, TransactionType = 1, Status = 1 };

                            //更新系統餘額
                            var ASKB = db.FundsBalances.Find(AccountIdASK, lowestAsk.MC);//買入
                            var BIDB = db.FundsBalances.Find(AccountIdBID, highestBid.MC);//賣出
                            if (ASKB != null && BIDB != null)//有設定餘額
                            {
                                ///
                                var ErrFile = Execution.Name + dt.ToString("yyyyMMdd") + ".txt";
                                string str = System.AppDomain.CurrentDomain.BaseDirectory;
                                str = Path.Combine(str, ErrFile);
                                try
                                {
                                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(str, true))
                                    {
                                        ///
                                        sw.WriteLine(dt.ToString("yyyyMMdd HH:mm:ss fffff"));
                                        sw.WriteLine(lowestAsk.MC + "總計：" + db.FundsBalances.Where(x => x.CurrencyVal == lowestAsk.MC).Sum(x => x.Quantity));
                                        sw.WriteLine(lowestAsk.DC + "總計：" + db.FundsBalances.Where(x => x.CurrencyVal == lowestAsk.DC).Sum(x => x.Quantity));
                                        sw.WriteLine(lowestAsk.Name + " " + lowestAsk.MC + "交易前：" + ASKB.Quantity);
                                        sw.WriteLine(highestBid.Name + " " + highestBid.MC + "交易前：" + BIDB.Quantity);
                                        ASKB.Quantity += MinQuantity;
                                        BIDB.Quantity -= MinQuantity;
                                        //
                                        sw.WriteLine(lowestAsk.Name + " " + lowestAsk.MC + "交易後：" + ASKB.Quantity);
                                        sw.WriteLine(highestBid.Name + " " + highestBid.MC + "交易後：" + BIDB.Quantity);
                                        sw.WriteLine();

                                        var ASKS = db.FundsBalances.Find(AccountIdASK, lowestAsk.DC);
                                        var BIDS = db.FundsBalances.Find(AccountIdBID, highestBid.DC);

                                        //
                                        sw.WriteLine(lowestAsk.Name + " " + lowestAsk.DC + "交易前：" + ASKS.Quantity);
                                        sw.WriteLine(lowestAsk.Name + " " + highestBid.DC + "交易前：" + BIDS.Quantity);

                                        //
                                        ASKS.Quantity -= Askval * MinQuantity;
                                        BIDS.Quantity += Bidval * MinQuantity;

                                        //
                                        sw.WriteLine(lowestAsk.Name + " " + lowestAsk.DC + "交易後：" + ASKS.Quantity);
                                        sw.WriteLine(lowestAsk.Name + " " + highestBid.DC + "交易後：" + BIDS.Quantity);
                                        sw.WriteLine();
                                        if (BIDB.Quantity > 0 && ASKS.Quantity > 0)
                                        {
                                            try
                                            {
                                                //存檔
                                                db.SaveChanges();
                                                //交易
                                                sw.WriteLine("API交易開始");
                                                var ExchangeMsg = API.StockExchange(highestBid, lowestAsk, MinQuantity);
                                                sw.WriteLine("API交易結束");
                                                if (ExchangeMsg.Stace)
                                                {
                                                    Console.WriteLine("交易成功");
                                                    sw.WriteLine("交易成功");
                                                    TransactionConfirmedASK.Status = 1;
                                                    TransactionConfirmedBID.Status = 1;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("交易失敗");
                                                    sw.WriteLine("交易失敗");
                                                    if (ExchangeMsg.AskStace.Stace)
                                                    {
                                                        TransactionConfirmedASK.Status = 1;
                                                    }
                                                    else
                                                    {
                                                        TransactionConfirmedASK.Status = -1;
                                                        TransactionConfirmedASK.Message = ExchangeMsg.AskStace.Msg;
                                                        ReturnMessageApi.ReturnMessage(ExchangeMsg.AskStace.Name + "：交易失敗", ExecutionsId, 0, ProcessId);
                                                    }
                                                    if (ExchangeMsg.BidStace.Stace)
                                                    {
                                                        TransactionConfirmedBID.Status = 1;
                                                    }
                                                    else
                                                    {
                                                        TransactionConfirmedBID.Status = -1;
                                                        TransactionConfirmedBID.Message = ExchangeMsg.BidStace.Msg;
                                                        ReturnMessageApi.ReturnMessage(ExchangeMsg.BidStace.Name + "：交易失敗", ExecutionsId, 0, ProcessId);
                                                    }

                                                }
                                                TransactionRecord.TransactionConfirmeds.Add(TransactionConfirmedASK);
                                                TransactionRecord.TransactionConfirmeds.Add(TransactionConfirmedBID);
                                                //存檔
                                                db.SaveChanges();
                                            }
                                            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                                            {
                                                var msg = "資料有被更新，新重新讀取";
                                                sw.WriteLine(msg);
                                                ReturnMessageApi.ReturnMessage(msg, ExecutionsId, 0, ProcessId);
                                            }
                                        }
                                        else
                                        {
                                            if (!(BIDB.Quantity > 0))
                                            {
                                                Console.WriteLine(highestBid.Name + "：" + highestBid.MC + "餘額不足");
                                            }
                                            if (!(ASKS.Quantity > 0))
                                            {
                                                Console.WriteLine(lowestAsk.Name + "：" + lowestAsk.DC + "餘額不足");
                                            }
                                        }
                                        ///
                                        sw.WriteLine("----------------------------");
                                        sw.WriteLine();
                                        sw.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ReturnMessageApi.ReturnMessage("文字檔寫入失敗；" + ex.ToString(), ExecutionsId, 0, ProcessId);
                                }
                            }
                            else
                            {
                                if (ASKB == null)
                                {
                                    Console.WriteLine(lowestAsk.Name + "：" + lowestAsk.DC + "餘額未設定");
                                }
                                if (BIDB == null)
                                {
                                    Console.WriteLine(highestBid.Name + "：" + highestBid.MC + "餘額未設定");
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine("交易失敗-查無執行名稱");
                        }
                        SaveLoop = false;
                    }
                }
                catch (Exception ex)
                {
                    var dt = DateTime.Now;//
                    var ErrFile = dt.ToString("yyyyMMdd") + ".txt";
                    Console.WriteLine("交易失敗-回存資料庫失敗");
                    Console.WriteLine(ex.ToString());
                    string str = System.AppDomain.CurrentDomain.BaseDirectory;
                    str = Path.Combine(str, "ErrMsg");
                    if (!Directory.Exists(str))
                    {
                        //新增資料夾
                        Directory.CreateDirectory(str);
                    }
                    str = System.IO.Path.Combine(str, ErrFile);
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(str, true))
                    {
                        sw.WriteLine(Execution.Name);
                        sw.WriteLine(dt);
                        sw.WriteLine(ex.ToString());
                        sw.WriteLine("----------------------------");
                        sw.WriteLine();
                        sw.Close();
                    }
                }
            }

        }


        public void Finish()
        {
            Console.WriteLine(string.Format("{0} - 關閉中…", Execution.Name));

            API.EndUp();

            Console.WriteLine(string.Format("{0} - 已關閉", Execution.Name));
        }
    }
}
