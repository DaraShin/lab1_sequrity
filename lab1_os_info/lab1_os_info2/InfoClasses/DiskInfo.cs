using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class DiskInfo
    {
        public String Name { get; set; }
        public Int32 TotalSpace { get; set; }    
        public Int32 FreeSpace { get; set; }

        public override string ToString()
        {
            return "Диск: " + Name + "\n"
                +   "Свободно " + FreeSpace + " Гб из " + TotalSpace + " Гб"; ;
        }
    }
}
