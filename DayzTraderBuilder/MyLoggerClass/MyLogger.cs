using System; 
using System.IO;
using System.Text;

namespace DayzTraderBuilder.MyLoggerClass
{
    public class MyLogger
    {
        private string LogFileName;
        private string LogFilePath;
        private string FullPath;
        //public FileStream logf;
        //private StreamWriter Sw;
        public bool FileIsCreated { get; private set; }
         
        

        public MyLogger(string _path)
        {
            LogFilePath = $"{_path}Logs\\";
            //LogFilePath = LogFilePath.Replace("\\", "/") ;
            LogFileName = DateTime.Now.ToString().Replace(":","-");
            FullPath = $"{LogFilePath}{LogFileName}.txt";
            FileIsCreated = true;

            // Добавить проход по папке LogFilePath
            // и удалять файлы старше, например, 3-х дней
            DltOldLogs(LogFilePath);

            try
            {
                // проверим папку, если нету - создадим
                if (!System.IO.Directory.Exists(LogFilePath))
                {
                    System.IO.Directory.CreateDirectory(LogFilePath);
                }


                //logf = File.Create(FullPath);
                //logf = File.OpenWrite(FullPath);
                //logf.SetLength(0); // очистка файла

                //Sw = new StreamWriter(logf);

                //Sw.WriteLine("Файл для логирования создан!");
            }
            catch {
                FileIsCreated = false ;
            }

        }

        //~MyLogger()
        //{
        //    try { logf.Close();  } catch { } 
        //}

        // ЛОгирование информационного сообщения
        public void Info(string _message)
        {
            if (!FileIsCreated) return; 
            File.AppendAllText(FullPath, $"INFO: {_message}\r\n", Encoding.GetEncoding("Windows-1251"));

        }

        // сообщение об ошибке 
        public void Error(string _message)
        { 
            if (!FileIsCreated) return; 
            File.AppendAllText(FullPath, $"ERROR: {_message}\r\n", Encoding.GetEncoding("Windows-1251"));
        }
        // сообщение об успехе
        public void Done(string _message)
        {
            if (!FileIsCreated) return; 
            File.AppendAllText(FullPath, $"DONE: {_message}\r\n", Encoding.GetEncoding("Windows-1251"));
        }

        public void Debug(string _message)
        {
            if (!FileIsCreated) return;
#if DEBUG
            // сделал флаг, что бы в дебаге можно было отключать спам сообщений  
            File.AppendAllText(FullPath, $"DEBUG: {_message}\r\n", Encoding.GetEncoding("Windows-1251"));
#endif


        }

        public void Msg(string _message)
        {
            if (!FileIsCreated) return; 
            File.AppendAllText(FullPath, $"{_message}\r\n", Encoding.GetEncoding("Windows-1251"));
        }
         
        // удаление лог файлов из папки
        private void DltOldLogs(string _logPath)
        {
            if (!System.IO.Directory.Exists(LogFilePath)) { return; } // нет папки - на выход

            string[] allfiles = Directory.GetFiles(_logPath, "*.txt");

            var curDate = DateTime.Now;

            foreach(string file in allfiles)
            {
                // получим дату создания файла
                var fileDate = System.IO.File.GetCreationTime(file);
                // разница дат
                var diff = curDate - fileDate; 
                // разница в часах
                var diffhours = diff.TotalHours;
                // старше 24 часов -  удаляем
                if(diffhours > 24) {
                    File.Delete(file);
                }

            }

        }



    }


}
