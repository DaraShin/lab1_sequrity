using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class AntivirusInfo
    {
        public String Name { get; set; }
        public Boolean? IsEnabled { get; set; }
        public Boolean? IsUpdated { get; set; }

        public override String ToString()
        {
            String info = Name + "\n";
            info += "Включен: ";
            switch (IsEnabled)
            {
                case true:
                    info += "да";
                    break;
                case false:
                    info += "нет";
                    break;
                case null:
                    info += "нет информации";
                    break;
            }
            info += "\nСигнатуры обновлены: ";
            switch (IsUpdated)
            {
                case true:
                    info += "да";
                    break;
                case false:
                    info += "нет";
                    break;
                case null:
                    info += "нет информации";
                    break;
            }
            return info;
               
        }
    }
}
