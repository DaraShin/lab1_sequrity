using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class RamInfo
    {
        public Int32 TotalSize { get; set; }
        public Int32 LoadPercent { get; set; }

        public override string ToString()
        {
            return "Оперативная память: " + TotalSize + " Гб\n"
                + "Процент загрузки RAM: " + LoadPercent + "%";
        }
    }
}
