using System;
using Oika.Libs.CuiCommandParser;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Security.Cryptography;

namespace amatsukazesender
{
    
    class Program
    {

        public static Logger logger = LogManager.GetCurrentClassLogger();
        




        static void Main(string[] args)
        {
            var parser = new CommandParser();
            var options = new[]
            {
                new CommandOption('b', "beforestr", "C:\\", "置き換え前Path", CommandOptionKind.NeedsValue),
                new CommandOption('a', "afterstr", "F:\\", "置き換え対象Path", CommandOptionKind.NeedsValue),
                new CommandOption('p', "amtaddpath", "C:\\Amatsukaze_0.9.1.4\\Amatsukaze\\exe_files\\AmatsukazeAddTask.exe", "AmatsukazeAddTask.exeのパス", CommandOptionKind.NeedsValue)
            };

            foreach (var opt in options)
            {
                parser.RegisterOption(opt);
            }


            var usagebuilder = parser.NewUsageBuilder("amtsend");
            usagebuilder.OptionKeyValueSeparator = '=';
            usagebuilder.Summary = "TSファイルのパスを置き換えたあとに、AmatsukazeにAddTaskします";

            usagebuilder.AddUseCase(usagebuilder.NewUseCase().AddArg(usagebuilder.NewUseCaseArg("OPTION").AsMultiple().AsOptional())
                                                             .AddArg(usagebuilder.NewUseCaseArg("TSFile"))
                                                             .AddArg(usagebuilder.NewUseCaseArg("BeforeSTR"))
                                                             .AddArg(usagebuilder.NewUseCaseArg("AfterSTR"))
                                                             .AddArg(usagebuilder.NewUseCaseArg("Amatsukaze Path").AsOptional()));



            var apppro = System.Reflection.Assembly.GetExecutingAssembly();
            var appinfo = apppro.GetName();
            Console.WriteLine("Amatsukaze Sender Ver {0}", appinfo.Version);
            
            
            var parsed = parser.Parse(args);
            if (parsed == null)
            {
                return;
            }


            Console.WriteLine(usagebuilder.ToString());
            // initおわり

            string prebefval = "C:\\TV\\";
            string preaftval = "F:\\";
            string preamtpath = "C:\\friio\\Amatsukaze_0.9.1.4\\Amatsukaze\\exe_files\\AmatsukazeAddTask.exe";


            // String teststr = "C:\\TV\\afffafs\\saafsf!!!!!##nya.mp4";
            // Console.WriteLine("Original: {0}", teststr);
            // Console.WriteLine("Replace: {0}", teststr.Replace(prebefval, preaftval));


            var opbefval = parsed.GetOptionValue('b');
            var opaftval = parsed.GetOptionValue('a');
            var opamtpath = parsed.GetOptionValue('p');


            var suffix = parsed.CommandParameters;
            string befval = (parsed.HasOption('b') ? opbefval : prebefval);
            string aftval = (parsed.HasOption('a') ? opaftval : preaftval);
            string amtpath = (parsed.HasOption('p') ? opamtpath : preamtpath);


            if (suffix.Count == 0)
            {
                return;
            }
            
            if (suffix[0].EndsWith(".ts") == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("これはTSファイルではありません");
                return;
            }


            logger.Info("Logger Init");

            string sourcedir = suffix[0];
            string destdir = suffix[0].Replace(befval, aftval);
            string remotedir = suffix[0].Replace("C:\\TV", "F:\\TV");

            string amtopt = (" -r \"G:\\friio\\Amatsukaze_0.9.1.4\\Amatsukaze\" -ip \"192.168.1.3\" -p 32768 -o \"F:\\TV\\encoded\" -s \"自動選択_デフォルト\" --priority 3 --no-move " + "-f \"" + remotedir + "\"");

            string amtcommand = (amtpath + amtopt);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("元ファイル: {0}", sourcedir);
            logger.Info("元ファイル: {0}", sourcedir);
            Console.WriteLine();
            Console.WriteLine("コピー先パス: {0}", destdir);
            logger.Info("コピー先パス: {0}", destdir);
            Console.WriteLine();
            Console.WriteLine("コピー先環境パス: {0}", remotedir);
            logger.Info("コピー先環境パス: {0}", remotedir);
            Console.WriteLine();
            Console.WriteLine("Amatsukazeパス: {0}", amtpath);
            logger.Info("Amatsukazeパス: {0}", amtpath);
            Console.WriteLine();
            Console.WriteLine("Amatsukaze実行コマンド: {0}", amtcommand);
            logger.Info("Amatsukaze実行コマンド: {0}", amtcommand);
            Console.WriteLine();
            Console.ResetColor();




            //main

            //FileCopy
            CopyFileClass.CopyFile(sourcedir, destdir);
            //HashComparison
            HashComparisonClass.HashComparison(sourcedir, destdir);


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Amatsukazeにタスクを追加します");
            Console.ResetColor();
            //SendJob
            SendClass.SendJob(amtpath, amtopt);



            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("多分完了しました。");
            Console.ResetColor();



            return;

        }
    }
}
