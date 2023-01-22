using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using DayzTraderBuilder.StaticFunc;

namespace DayzTraderBuilder.Classes
{
    // максимальное количество трейдеров = 14
    // + файл CUrrency - он пишется первым
    public class TraderFile
    {
        // имя файла
        public string Name { get; set; }
        // сумма MD5
        public string Md5 { get; set; }
        // полный путь к файлу
        public string Path { get; set; }

        public TraderFile(string _name, string _path) { 
        
            Name = _name;
            Path = _path;

            Md5 = ComputeMD5Checksum(Path);
             
        }




        // получить сумму MD5 файла
        private string ComputeMD5Checksum(string path)
        {
            if (File.Exists(path))
            {
                return StaticFunctions.GetMd5Sum(path);
            }
            else {
                Console.WriteLine("[ERROR] Не найден файл указанный в списке FilesList");
                Console.WriteLine($"[FILE:] {path}");
            }
            return string.Empty;
        }
    }


     
}




