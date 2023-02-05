using DayzTraderBuilder.Classes;
using System; 
using System.Linq; 
using System.IO;
using DayzTraderBuilder.MyLoggerClass;

namespace DayzTraderBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BuilderConfig config;
            MyLogger myLogger;
            string configPath;

            Console.WriteLine("[INFO] Начало работы программы");

            if (args.Count() > 0)
            {
                // если прислали путь к фалу конфига
                configPath = args[0];
                //string fullPath = $"{path}TraderBuilder.config";
                Console.WriteLine("[INFO] Загружается конфиг из файла:");
                Console.WriteLine(configPath);
                 
            }
            else
            {
                configPath = $"{Environment.CurrentDirectory}\\";
                // имя файла конфига по умолчанию с путем
                //string fullPath = $"{curPath}TraderBuilder.config";
                 
            }

            // создаем логгер
            myLogger = new MyLogger(configPath);
            myLogger.Info("Начало работы программы");

            // объект конфига
            config = new BuilderConfig(configPath, myLogger);


            if (!config.NeedCreateNewTraderFile)
            { 
                myLogger.Info("Произошла ошибка либо не было изменений в файлах трейдера.");
                myLogger.Info("Пересоздание файла не выполняется. Выход из программы.");
                return;
            }

            // функция создания файла и его заполнение (пихаем сюда config)
            myLogger.Info("Начинаем собирать файл трейдера.");
            StaticFunc.StaticFunctions.CreateTraderFile(config, myLogger);

            // Создание или пересоздание файла MD5
            myLogger.Info("Создание/пересоздание файла с MD5.");
            StaticFunc.StaticFunctions.CreateOrUpdateMD5(config, myLogger);

            
            // Копируем созданный файл в указанные директории
            if (config.PathCopyTo.Count> 0 )
            {
                myLogger.Info("Копирование собранного файла трейдера в директории назначения.");
                foreach (var path in config.PathCopyTo)
                {
                    if(!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); } // создадим если нет папки
                    File.Copy($"{config.ConfigPath}{config.MainFileName}", $"{path}{config.MainFileName}",true);
                }

                
            }

            //Console.ReadKey();
        }
    }
}
