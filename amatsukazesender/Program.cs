using System;
using Oika.Libs.CuiCommandParser;

namespace amatsukazesender
{
    
    class Program
    {
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








        }
    }
}
