using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Text;

namespace HashCompTest
{
    internal class Class1
    {
        public static void HashComparison(string sourcedir, string destdir)
        {

            int retrymax = 10000;
            int retrycount = 0;



            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("ハッシュ値の比較をします");
            Console.ResetColor();

            var sHA256 = SHA256.Create();
            var BefFile = new FileStream(sourcedir, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var AftFile = new FileStream(destdir, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            while (retrycount < retrymax)
            {
                try
                {
                    byte[] tmpBefHash = sHA256.ComputeHash(BefFile);
                    byte[] tmpAftHash = sHA256.ComputeHash(AftFile);

                    string BefHash = BitConverter.ToString(tmpBefHash).Replace("-", "");
                    string AftHash = BitConverter.ToString(tmpAftHash).Replace("-", "");

                    Console.WriteLine("元ファイル:       " + BefHash);

                    Console.WriteLine("コピー先ファイル: " + AftHash);

                    if (BefHash == AftHash)
                    {
                        Console.WriteLine("一致しました");

                        try
                        {
                            Console.WriteLine("元TSを削除します");
                            Console.ResetColor();
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ハッシュ比較エラー: " + e);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"5秒後に再試行します… リトライ回数: {retrycount}");
                            Console.ResetColor();
                            retrycount++;
                            Thread.Sleep(5000);
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ハッシュ不一致");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"5秒後に再試行します… リトライ回数: {retrycount}");
                        Console.ResetColor();
                        retrycount++;
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("エラー: " + e);
                    Console.ResetColor();
                }
            }
            if (retrycount >= retrymax)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ハッシュ比較エラーのためコピーを再試行します");
                Console.ResetColor();
                HashComparison(sourcedir, destdir);

            }

        }
    }
}
