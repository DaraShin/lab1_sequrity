using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    internal class ProcessorInfo
    {
        public Int32 PhysicalCoresNumber { get; set; }
        public Int32 LogicalCoresNumber { get; set; }
        public Int32 LoadPercent { get; set; }

        public override String ToString()
        {
            return "Количество физических ядер: " + PhysicalCoresNumber + "\n"
             + "Количество логических процессоров: " + LogicalCoresNumber + "\n"
             + "Процент загрузки процессора: " + LoadPercent + "%";
        }
    }
}
