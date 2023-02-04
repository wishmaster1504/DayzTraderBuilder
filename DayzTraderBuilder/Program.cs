using DayzTraderBuilder.Classes;
using System; 
using System.Linq; 
using System.IO;


namespace DayzTraderBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BuilderConfig config;

            if (args.Count() > 0)
            {
                // если прислали путь к фалу конфига
                string path = args[0];
                //string fullPath = $"{path}TraderBuilder.config";
                Console.WriteLine("[INFO] Загружается конфиг из файла:");
                Console.WriteLine(path);
                
                config = new BuilderConfig(path);
            }
            else
            { 
                string curPath = $"{Environment.CurrentDirectory}/";
                // имя файла конфига по умолчанию с путем
                //string fullPath = $"{curPath}TraderBuilder.config";

                config = new BuilderConfig(curPath);
            }


              

            if (!config.NeedCreateNewTraderFile)
            {
                Console.WriteLine("[INFO] Произошла ошибка либо не было изменений в файлах трейдера");
                Console.WriteLine("[INFO] Пересоздание файла не выполняется. Выход из программы.");
                return;
            }

            // функция создания файла и его заполнение (пихаем сюда config)
            StaticFunc.StaticFunctions.CreateTraderFile(config);

            // Создание или пересоздание файла MD5
            StaticFunc.StaticFunctions.CreateOrUpdateMD5(config);


            // Копируем созданный файл в указанные директории
            if (config.PathCopyTo.Count> 0 )
            {
                foreach (var path in config.PathCopyTo)
                {
                    File.Copy($"{config.ConfigPath}{config.MainFileName}", $"{path}{config.MainFileName}",true);
                }

                
            }

            Console.ReadKey();
        }
    }
}
