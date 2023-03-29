using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class FirewallInfo
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public override string ToString()
        {
            return "Название программы: " + Name + "\n"
                + "Включен: " + (IsEnabled ? "да" : "нет");
        }
    }
}
