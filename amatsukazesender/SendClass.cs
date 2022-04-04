using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace amatsukazesender
{
    internal class SendClass
    {


        public class PostNoteBody
        {
            public string i { get; set; }
            public string text { get; set; }
            public string visibility { get; set; }
        }



        public static async void SendJob(string amtpath, string amtopt)
        {
            var logger = Program.logger;

            int retrymax = 10000;
            int retrycount = 0;

            var amtproc = new Process();

            amtproc.StartInfo.FileName = amtpath;
            amtproc.StartInfo.Arguments = amtopt;
            amtproc.StartInfo.UseShellExecute = false;
            amtproc.StartInfo.CreateNoWindow = true;
            amtproc.StartInfo.RedirectStandardOutput = true;
            logger.Info("タスク追加開始");

            retrycount = 0;

            while (true)
            {
                amtproc.Start();

                var amtlog = amtproc.StandardOutput.ReadToEnd();

                amtproc.WaitForExit();

                if (amtlog.Contains("件追加しました") == true)
                {
                    break;
                }
                else if (retrycount++ < retrymax)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("エラーが発生しました");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"5秒後に再試行します… リトライ回数: {retrycount}");
                    Console.ResetColor();
                    logger.Error("タスク追加エラー発生");
                    logger.Warn($"再試行待機 リトライ回数: {retrycount}");
                    Thread.Sleep(5000);
                }
            }
            if (retrymax < retrycount)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("エラーが発生しました");
                Console.ResetColor();

                string makeNoteURL = ConfigurationManager.AppSettings["misskeyDomain"] + "/api/notes/create";
                string misskeyToken = ConfigurationManager.AppSettings["misskeyToken"];
                string noteText = "@sim1222@misskey.io @_kokt@simkey.net エンコードジョブの追加に失敗しました。";

                var req = new HttpClient();

                var postBodyCs = new PostNoteBody
                {
                    i = misskeyToken,
                    text = noteText,
                    visibility = "public"
                };

                var postBodyJson = JsonSerializer.Serialize(postBodyCs);
                var Body = new StringContent(postBodyJson);
                try
                {
                    var res = await req.PostAsync(makeNoteURL, Body);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERRRRRRRRRRRRRRRRRRRRRRRRRRRRR: " + e);
                }

            }
        }
    }
}
