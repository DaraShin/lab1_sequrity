using System;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
using NetFwTypeLib;
using OSVersionExtension;
using WUApiLib;
using lab1_os_info2.InfoClasses;
using System.Collections.Generic;
using System.Text.Json;
using WindowsFirewallHelper.Collections;

namespace lab1_os_info2
{
    public partial class MainWindow : Window
    {
        Task getUpdatesInfoTask;
        Task getHardwareInfoTask;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnShowBtnClick(object sender, RoutedEventArgs e)
        {
            infoTextBlock.Text = "";
            AllInfo allInfo = new AllInfo();

            loadingProgressPanel.Visibility = Visibility.Collapsed;

            if ((bool)antivirusCheckBox.IsChecked)
            {
                infoTextBlock.Text += "Информация об анитивирусах:\n";

                List<AntivirusInfo> antivirusList = GetAntivirusInfo();
                infoTextBlock.Text += ListToString(antivirusList) + "\n";
                infoTextBlock.Text += "--------------------------------------------------------------------------------\n\n";

                if ((bool)jsonCheckBox.IsChecked)
                {
                    allInfo.antiviruses = antivirusList;
                }
            }

            if ((bool)firewallCheckBox.IsChecked)
            {
                FirewallInfo firewallInfo = GetFirewallInfo();
                infoTextBlock.Text += "Информация о брандмауэре:\n";
                infoTextBlock.Text += firewallInfo + "\n";
                infoTextBlock.Text += "\n--------------------------------------------------------------------------------\n\n";

                if ((bool)jsonCheckBox.IsChecked)
                {
                    allInfo.firewall = firewallInfo;
                }
            }

            if ((bool)osUpdateCheckBox.IsChecked)
            {
                loadingProgressPanel.Visibility = Visibility.Visible;

                getUpdatesInfoTask = new Task(() =>
                {
                    OperatingSystemInfo osInfo = getOsInfo();

                    List<String> updatesList = GetUpdateInfo();
                    String updateInfo = "Найдено обновлений: " + updatesList.Count + "\n";
                    updateInfo += ListToString(updatesList) + "\n";

                    Dispatcher.Invoke(() =>
                    {

                        infoTextBlock.Text += osInfo + "\n"
                            + updateInfo
                            + "\n--------------------------------------------------------------------------------\n\n";
                        if ((bool)jsonCheckBox.IsChecked)
                        {
                            allInfo.osInfo = osInfo;
                            allInfo.updates = updatesList;
                        }
                    });
                });
                getUpdatesInfoTask.Start();
            }

            if ((bool)hardwareCheckBox.IsChecked)
            {
                loadingProgressPanel.Visibility = Visibility.Visible;

                getHardwareInfoTask = new Task(() =>
                {
                    String hardwareInfo = "";
                    ProcessorInfo processorInfo = GetProcessorInfo();
                    RamInfo ramInfo = GetRamInfo();
                    List<DiskInfo> disks = GetDiskInfo();

                    hardwareInfo += processorInfo.ToString() + "\n\n";
                    hardwareInfo += ramInfo.ToString() + "\n\n";
                    hardwareInfo += ListToString(disks) + "\n";

                    Dispatcher.Invoke(() =>
                    {
                        infoTextBlock.Text += hardwareInfo + "--------------------------------------------------------------------------------\n\n";
                        if ((bool)jsonCheckBox.IsChecked)
                        {
                            allInfo.processorInfo = processorInfo;
                            allInfo.ramInfo = ramInfo;
                            allInfo.disks = disks;
                        }
                    });
                });
                getHardwareInfoTask.Start();
            }

            showBtn.IsEnabled = false;
            if (getHardwareInfoTask != null)
            {
                await getHardwareInfoTask;
            }
            if (getUpdatesInfoTask != null)
            {
                await getUpdatesInfoTask;
            }
            showBtn.IsEnabled = true;
            loadingProgressPanel.Visibility = Visibility.Collapsed;

            if ((bool)jsonCheckBox.IsChecked)
            {
                infoTextBlock.Text += "Данные в формате JSON:\n";
                infoTextBlock.Text += JsonSerializer.Serialize(allInfo, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
        }

        private List<AntivirusInfo> GetAntivirusInfo()
        {
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            int counter = 0;
            List<AntivirusInfo> antivirusList = new List<AntivirusInfo>(data.Count);
            foreach (ManagementObject virusChecker in data)
            {
                antivirusList.Add(new AntivirusInfo());
                antivirusList[counter].Name = (String)virusChecker["displayName"];
                GetAntivirusState(virusChecker["productState"].ToString(), antivirusList[counter]);
                counter++;
            }
            return antivirusList;
        }

        private void GetAntivirusState(String stateCode, AntivirusInfo antivirusInfo)
        {
            switch (stateCode)
            {
                case "262144":
                    antivirusInfo.IsEnabled = false;
                    antivirusInfo.IsUpdated = true;
                    break;
                case "266240":
                    antivirusInfo.IsEnabled = true;
                    antivirusInfo.IsUpdated = true;
                    break;
                case "266256":
                    antivirusInfo.IsEnabled = true;
                    antivirusInfo.IsUpdated = null;
                    break;
                case "262160":
                    antivirusInfo.IsEnabled = false;
                    antivirusInfo.IsUpdated = null;
                    break;
                case "393472":
                    antivirusInfo.IsEnabled = false;
                    antivirusInfo.IsUpdated = true;
                    break;
                case "397584":
                    antivirusInfo.IsEnabled = false;
                    antivirusInfo.IsUpdated = false;
                    break;
                case "397568":
                    antivirusInfo.IsEnabled = true;
                    antivirusInfo.IsUpdated = true;
                    break;
                case "397312":
                    antivirusInfo.IsEnabled = true;
                    antivirusInfo.IsUpdated = true;
                    break;
                case "393216":
                    antivirusInfo.IsEnabled = false;
                    antivirusInfo.IsUpdated = true;
                    break;
            }
        }

        private FirewallInfo GetFirewallInfo()
        {
            try
            {
                FirewallInfo firewallInfo = new FirewallInfo();

                Type tpNetFirewall = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                INetFwMgr mgrInstance = (INetFwMgr)Activator.CreateInstance(tpNetFirewall);
                firewallInfo.IsEnabled = mgrInstance.LocalPolicy.CurrentProfile.FirewallEnabled;

                if (WindowsFirewallHelper.FirewallManager.RegisteredProducts.Count > 0)
                {
                    firewallInfo.Name = WindowsFirewallHelper.FirewallManager.RegisteredProducts[0].FriendlyName;
                }

                return firewallInfo;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private OperatingSystemInfo getOsInfo()
        {
            OperatingSystemInfo osInfo = new OperatingSystemInfo();
            osInfo.OSName = OSVersion.GetOperatingSystem().ToString();
            osInfo.Version = OSVersion.GetOSVersion().Version.Major + "."
                + OSVersion.GetOSVersion().Version.Minor + "."
                + OSVersion.GetOSVersion().Version.Build;
            return osInfo;
        }

        private List<String> GetUpdateInfo()
        {
            UpdateSession uSession = new UpdateSession();
            IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher();
            uSearcher.Online = false;

            List<String> updatesList;
            try
            {
                ISearchResult sResult = uSearcher.Search("IsInstalled=0 And IsHidden=0");
                updatesList = new List<string>(sResult.Updates.Count); ;
                foreach (IUpdate update in sResult.Updates)
                {
                    updatesList.Add(update.Title);
                }

                return updatesList;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private ProcessorInfo GetProcessorInfo()
        {
            ProcessorInfo info = new ProcessorInfo();
            ManagementClass management = new ManagementClass("Win32_Processor");
            ManagementObjectCollection managementobject = management.GetInstances();

            foreach (ManagementObject mngObject in managementobject)
            {
                info.PhysicalCoresNumber = Int32.Parse(mngObject.Properties["NumberOfCores"].Value.ToString());
                info.LogicalCoresNumber = Int32.Parse(mngObject.Properties["NumberOfLogicalProcessors"].Value.ToString());
                info.LoadPercent = Int32.Parse(mngObject.Properties["LoadPercentage"].Value.ToString());
            }
            return info;
        }

        private RamInfo GetRamInfo()
        {
            RamInfo info = new RamInfo();
            ManagementClass management = new ManagementClass("Win32_PhysicalMemory");
            ManagementObjectCollection managementobject = management.GetInstances();

            Double totalRam = 0;
            Double freeRam = 0;
            foreach (ManagementObject mngObject in managementobject)
            {
                totalRam += Double.Parse(mngObject.Properties["Capacity"].Value.ToString()) / 1024 / 1024 / 1024;
            }
            info.TotalSize = (Int32)totalRam;

            management = new ManagementClass("Win32_OperatingSystem");
            managementobject = management.GetInstances();

            foreach (ManagementObject mngObject in managementobject)
            {
                freeRam += Double.Parse(mngObject.Properties["FreePhysicalMemory"].Value.ToString()) / 1024 / 1024;
            }

            info.LoadPercent = (Int32)((1.0 - freeRam / totalRam) * 100);
            return info;
        }

        private List<DiskInfo> GetDiskInfo()
        {
            List<DiskInfo> diskList = new List<DiskInfo>();
            DiskInfo diskInfo;

            ManagementClass management = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection managementobject = management.GetInstances();

            foreach (ManagementObject mngObject in managementobject)
            {
                diskInfo = new DiskInfo();
                diskInfo.Name = mngObject.Properties["Name"].Value.ToString();
                diskInfo.TotalSpace = (Int32)(Double.Parse(mngObject.Properties["Size"].Value.ToString()) / 1024 / 1024 / 1024);
                diskInfo.FreeSpace = (Int32)(Double.Parse(mngObject.Properties["FreeSpace"].Value.ToString()) / 1024 / 1024 / 1024);
                diskList.Add(diskInfo);
            }
            return diskList;
        }


        private String ListToString<T>(List<T> antivirusList)
        {
            String antivirusListString = "";
            for (int i = 0; i < antivirusList.Count; i++)
            {
                antivirusListString += "#" + (i + 1) + "\n";
                antivirusListString += antivirusList[i].ToString() + "\n";
            }
            return antivirusListString;
        }

        private void onSelectAllBtnClick(object sender, RoutedEventArgs e)
        {
            antivirusCheckBox.IsChecked = true;
            firewallCheckBox.IsChecked = true;
            osUpdateCheckBox.IsChecked = true;
            hardwareCheckBox.IsChecked = true;
        }

        private void onClearAllBtnClick(object sender, RoutedEventArgs e)
        {
            antivirusCheckBox.IsChecked = false;
            firewallCheckBox.IsChecked = false;
            osUpdateCheckBox.IsChecked = false;
            hardwareCheckBox.IsChecked = false;
        }
    }
}
