using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayzTraderBuilder.Classes
{
    // запись из конфиг файла по суммам MD5
    public class MD5info
    {
        // имя тега = имя файла
        public string FileName { get; set; }
        // сумма md5 в string
        public string Md5 { get; set; }

        public MD5info(string _name, string _md5) {

            FileName = _name;
            Md5 = _md5;
        }
    }
}
