using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class OperatingSystemInfo
    {
        public string OSName { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            return "Операционная система: " + OSName
                + "\nВерсия: " + Version;
        }
    }
}
