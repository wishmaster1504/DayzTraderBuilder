using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


// Класс настроек программы
// 
// Читаем все необходимые настройки
//

namespace DayzTraderBuilder.Classes
{
    public class BuilderConfig
    { 
        // путь к файлам трейдера и MD5
        public string PathTraderFiles { get; set; }
        public List<TraderFile> TraderFiles { get; set; } = new List<TraderFile>();
        // Основное имя файла трейдера, который потом будем копировать
        public string MainFileName { get; set; }
        // флаг надо ли создавать новый файл трейдера
        public bool NeedCreateNewTraderFile = false;
        // Куда копируем собранный файл трейдера
        public string PathCopyTo { get; set; }

        public string Md5FileName = "MD5SUM.config";
        // путь к файлу конфига, сюда же будем трейд формировать
        public string ConfigPath { get; set; }
        public string ConfigFullPath { get; set; }

        // создаем объект класса
        // если пришел путь к конфиг файлу из-вне (без имени файла но с последней наклонной чертой)
        public BuilderConfig(string _path)
        {
            ConfigPath = _path; 
            ConfigFullPath = $"{_path}TraderBuilder.config";// Path + TraderBuilder.config


            // Читаем конфиг файл TraderBuilder.config
            NeedCreateNewTraderFile = ReadCfgFile(ConfigFullPath);

        }

        // Чтение файла TraderBuilder.config
        private bool ReadCfgFile(string fullPath)
        {
            
             
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("[ERROR] Файл настроек не найден");
                Console.WriteLine($"[FILE:] {fullPath}");
                return false; // файл трейдера не пересобираем
            }


            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(fullPath);
            }
            catch {
                // не смогли разобрать конфиг или найти файлы
                Console.WriteLine("[ERROR] Ошибка чтения конфиг файла");
                Console.WriteLine($"[FILE:] {fullPath}");
                return false; // файл трейдера не пересобираем
            }
             


            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement node in xRoot.ChildNodes)
                {
                    if (node.Name == "PathTraderFiles")
                    {
                        PathTraderFiles = node.InnerText;
                    }

                    if (node.Name == "MainFileName")
                    {
                        MainFileName = node.InnerText;
                    }

                    if (node.Name == "PathCopyTo")
                    {
                        PathCopyTo = node.InnerText;
                    }

                    if (node.Name == "FilesList")
                    {
                        foreach (XmlNode childnode in node.ChildNodes)
                        {
                            TraderFile tf = new TraderFile(childnode.InnerText,$"{PathTraderFiles}{childnode.InnerText}");
                            TraderFiles.Add(tf);
                        }
                    }


                } // foreach

            } // if (xRoot != null)

            // проверим все ли файлы из списка мы прочитали
            // если хоть по одному нет суммы MD5 - значит файла нет
            // значит не верно задан список в конфиге - Исправить конфиг
            foreach (var item in TraderFiles)
            {
                if (item.Md5 == string.Empty)
                {
                    Console.WriteLine("[ERROR] Не смогли определить сумму MD5 по файлу");
                    Console.WriteLine("[ERROR] Указанный файл отсутствует. Исправьте конфиг или добавьте файл");
                    Console.WriteLine($"[FILE:] {item.Path}");
                    return false; // файл трейдера не пересобираем
                }
            }

            //var index = TraderFiles.Any(s => s.Equals(string.Empty));

            // получим прошлые MD5
            List<MD5info> mD5s = new List<MD5info>();
            mD5s = GetListLastMd5Sum();

            // Если список пришел пустой, значит запускаем 1-й раз
            // Надо создавать файл трейдера
            if (mD5s.Count == 0) return true;

            // проверим по каждому трейд файлу, что
            // сумма md5 для него есть
            // сумма md5 не изменилась/изменилась
            foreach (var item in TraderFiles)
            {
                if (!mD5s.Exists(s => s.FileName == item.Name && s.Md5 == item.Md5))
                {
                    // по одному из файлов нет суммы MD5 или она изменилась
                    return true; // пересобираем трейдер
                }
            }


                return false;
        }

        private List<MD5info> GetListLastMd5Sum()
        {
            List<MD5info> mD5Infos= new List<MD5info>();
            string fullPath = $"{ConfigPath}{Md5FileName}";

            if (!File.Exists(fullPath)) return mD5Infos; // нет файла, значит первый запуск


            XmlDocument xDoc = new XmlDocument();
            
            xDoc.Load(fullPath);


            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                foreach (XmlElement node in xRoot.ChildNodes)
                {
                    MD5info md5 = new MD5info(node.Name, node.InnerText);
                    mD5Infos.Add(md5);
                }
            } // xRoot

            return mD5Infos;
        }
    }
}
