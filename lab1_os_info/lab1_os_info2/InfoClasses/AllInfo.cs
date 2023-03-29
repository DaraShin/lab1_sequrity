using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_os_info2.InfoClasses
{
    class AllInfo
    {
        public List<AntivirusInfo> antiviruses { get; set; }
        public FirewallInfo firewall { get; set; }
        public OperatingSystemInfo osInfo { get; set; }
        public List<String> updates { get; set; }
        public ProcessorInfo processorInfo { get; set; }
        public RamInfo ramInfo { get; set; }
        public List<DiskInfo> disks { get; set; }
    }
}
