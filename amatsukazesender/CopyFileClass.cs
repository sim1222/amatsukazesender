using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace amatsukazesender
{
    internal class CopyFileClass
    {

        public class PostNoteBody
        {
            public string i { get; set; }
            public string text { get; set; }
            public string visibility { get; set; }
        }


        async public static void CopyFile(string sourcedir, string destdir)
        {

            int retrynum = 10000;
            int retrycount = 0;

            var logger = Program.logger;

            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ファイルコピーを開始します");
                    logger.Info("ファイルコピー開始");
                    Console.ResetColor();
                    File.Copy(sourcedir, destdir, false);
                    break;
                }
                catch (IOException error)
                {
                    Console.WriteLine(error.Message);
                    if (retrycount++ < retrynum)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"エラーが発生しました: {error}");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"5秒後に再試行します… リトライ回数: {retrycount}");
                        Console.ResetColor();
                        logger.Error($"コピーエラー発生: {error}");
                        logger.Warn($"再試行待機 リトライ回数: {retrycount}");
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"リトライ回数が{retrycount}に達しました。処理を中止します。");
                        logger.Warn($"リトライ回数{0}突破、中止", retrycount);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"エラー内容: {error}");
                        logger.Error($"エラー内容: {error}");

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
                        throw error;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("コピーが完了しました。");
            logger.Info("コピー完了");
            Console.ResetColor();
        }
    }
}
