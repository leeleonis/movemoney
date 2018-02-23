using BitcoinDeveloper.Models;
using BitcoinDeveloper.Models.Repositiry;
using BitcoinService.ApiClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinService
{
    class Program
    {
        //private static Binance.Net.BinanceSocketClient Binanceclient = new Binance.Net.BinanceSocketClient();//Binancel的連線
        //private static List<string> BinanceCanSymbol = new List<string>();//Binancel能交換的
        private static BitcoinTransactionEntities db = new BitcoinTransactionEntities();
        private static IRepository<Execution> ExecutionsTB;

        private static List<BrickManage> BrickList;
        private static List<ExchangeData> ExchangeDataList = new List<ExchangeData>();

        static void Main(string[] args)
        {
            var sUrl = "";//主機網址
            var nowxit = true;
            //System.Diagnostics.Process.Start(target, "我是參數");
            //印出程式的名稱
            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            var id = Guid.NewGuid();
            id = Guid.Parse("b4eaf34d-34cb-4983-86f1-18e3aa6ef4bb");
            if (args.Any())
            {
                id = Guid.Parse(args[0]);
                sUrl = args[1];
                //印出傳入的參數
                Console.WriteLine(args[0].ToString());
            }
            Process currentProcess = Process.GetCurrentProcess();
            ////連線SignalR Hub
            //var connection = new HubConnection("http://localhost:19636/");
            //IHubProxy commHub = connection.CreateHubProxy("CommHub");
            ////顯示Hub傳入的文字訊息
            //commHub.On("ShowMessage", msg => Console.WriteLine(msg));
            ////利用done旗標決定程式中止
            //bool done = false;
            ////當Hub要求執行Exit()時，將done設為true
            //commHub.On("Exit", () => { done = true; });
            ////建立連線，連線建立完成後向Hub註冊識別名稱
            //connection.Start().ContinueWith(task =>
            //{
            //    if (!task.IsFaulted)
            //        //連線成功時呼叫Server端方法register()
            //        commHub.Invoke("register", id.ToString());
            //    else
            //        done = true;
            //});



            ExecutionsTB = new GenericRepository<Execution>(db);
            List<Execution> ExcutionList = ExecutionsTB.GetAll(true).Where(e => e.id == id ).ToList(); //&& e.Status.Equals(1)
            BrickList = new List<BrickManage>();

            var Tasklist = new List<Task>();
            if (ExcutionList.Any())
            {
                foreach (Execution Execution in ExcutionList)
                {
                    BrickManage BrickTest = new BrickManage(Execution, id.ToString(), sUrl, Execution.ExchangeType, currentProcess.Id);
                    BrickList.Add(BrickTest);
                    Task Init = Task.Factory.StartNew(() => BrickTest.Initialize()).ContinueWith(task => BrickTest.MatchExchange(), TaskContinuationOptions.OnlyOnRanToCompletion);
                    Tasklist.Add(Init);
                }
            }
            else
            {
                Console.WriteLine("沒有交易所");
            }
            while (nowxit)
            {
                var red = Console.ReadLine();
                if (red == "exit")
                {
                    nowxit = false;
                    foreach (BrickManage BrickTest in BrickList)
                    {
                        BrickTest.Finish();
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
            Console.ReadLine();
        }

        private static void Mappingline()
        {
            Task InitT = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    foreach (var item in ExchangeDataList)
                    {
                        Console.WriteLine(item.Name);
                        Console.WriteLine(item.Ask);
                        Console.WriteLine(item.Bid);
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });
        }
    }
}
