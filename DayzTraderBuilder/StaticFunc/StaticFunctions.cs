using DayzTraderBuilder.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static void CreateOrUpdateMD5(BuilderConfig builderConfig) {

            string md5filePath = $"{builderConfig.PathTraderFiles}{builderConfig.Md5FileName}";

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
                Console.WriteLine("[INFO] Не удалось создать или открыть файл.");
                Console.WriteLine("[INFO] Возможно файл занят другим процессом.");
                Console.WriteLine("[INFO] Не сохраняем");
            }

        }

    }
}
