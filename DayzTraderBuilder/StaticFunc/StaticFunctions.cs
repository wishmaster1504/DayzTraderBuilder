using DayzTraderBuilder.Classes;
using DayzTraderBuilder.MyLoggerClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DayzTraderBuilder.StaticFunc
{
    public static class StaticFunctions
    {

        // получение суммы MD5 по файлу
        // вызываем только если проверили что файл существует
        public static string GetMd5Sum(string path)
        {

            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }

        // создание или обновление файла с суммами MD5
        public static void CreateOrUpdateMD5(BuilderConfig builderConfig, MyLogger _logger) {

            string md5filePath = $"{builderConfig.ConfigPath}{builderConfig.Md5FileName}";

            try
            {
                  
                FileStream fs = File.Create(md5filePath);
                fs.SetLength(0); // очистка файла

                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>"); 
                sw.WriteLine("<configuration>");


                foreach(var item in builderConfig.TraderFiles)
                {
                    string md5 = GetMd5Sum(item.Path);
                    // <TraderCurrency.txt>MD5SUM</TraderCurrency.txt>
                    sw.WriteLine($"<{item.Name}>{md5}</{item.Name}>");
                }

                 
                sw.WriteLine("</configuration>");

                sw.Close();
            } 
            catch {

                // если свалилось, значит уже кто то пишет
                // тогда этот процесс не пишет 
                _logger.Info("Не удалось создать или открыть файл.");
                _logger.Info($"{md5filePath}");
                _logger.Info("Возможно файл занят другим процессом");
                _logger.Info("Не сохраняем. Забиваем ХУЙ");
            }

        }

        
        // Создание файла трейдера
        public static void CreateTraderFile(BuilderConfig builderConfig, MyLogger _logger)
        {
            // файл TraderConfig.txt
            string targetFilePath = $"{builderConfig.ConfigPath}{builderConfig.MainFileName}";
            int rowNumber = 0;
            try
            {

                FileStream fs = File.Create(targetFilePath);
                fs.SetLength(0); // очистка файла

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine("\t\t/////////////////////////////////////////////////////////////////////////////////////////////////");
                sw.WriteLine("        //                                                                                             //");
                sw.WriteLine("        //      Need Help? The Trader Mod has its own Channel on the DayZ Modders Discord Server!      //");
                sw.WriteLine("        //                                                                                             //");
                sw.WriteLine("        //      Only Singleline Comments work. Don't use Multiline Comments!                           //");
                sw.WriteLine("        //      /* THIS COMMENT WILL CRASH THE SERVER! */                                              //");
                sw.WriteLine("        //      // THIS COMMENT WORKS!                                                                 //"); 
                sw.WriteLine("        //                                                                                             //");
                sw.WriteLine("        /////////////////////////////////////////////////////////////////////////////////////////////////");
                sw.WriteLine("// Quantity * means max value; Quantity V means Vehicle; Quantity M means Magazine; Quantity W means Weapon; Quantity S means Steack Meat"); 

                foreach(var item in builderConfig.TraderFiles)
                {
                    // открываем файл, читаем и пишем
                    try
                    {
                        FileStream fromF = File.Open(item.Path, FileMode.Open,FileAccess.Read, FileShare.Read);
                        StreamReader sr = new StreamReader(fromF);

                        while (!sr.EndOfStream)
                        {
                           rowNumber ++;
                           string s = sr.ReadLine();
                           sw.WriteLine(s);
                        }
                        fromF.Close();     
                    }
                    catch
                    {
                        //
                        _logger.Error("Ошибка записи в файл трейдера.");
                        _logger.Error($"Копировался файл: {item.Path}");
                        _logger.Error($"Строка номер: {rowNumber}");
                    }

                }

                sw.WriteLine("<FileEnd>\t\t\t\t\t\t\t\t\t\t\t\t// This has to be on the End of this File and is very importan");

                sw.Close();
            }
            catch {
                _logger.Error("Ошибка Создания файла.");
                _logger.Error($"Файл: {targetFilePath}");
            }

        }


       
    }
}
