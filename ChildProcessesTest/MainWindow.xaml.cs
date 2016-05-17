using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChildProcessesTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int childCount = 0;
        public int childsExited = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Process proc = Process.Start("F:\\Games\\Minecraft\\MinecraftLauncher.exe");
            proc.WaitForExit();
            Console.WriteLine("Process has exited");
            IEnumerable<Process> list = ProcessExtensions.GetChildProcesses(proc);
            
            foreach(Process process in list)
            {
                childCount++;
                process.EnableRaisingEvents = true;
                process.Exited += (exitSender, exit_e) => childProcess_Exited(sender, e, process);
            }
        }

        private void childProcess_Exited(object sender, System.EventArgs e, Process process)
        {
            childsExited++;
            Console.WriteLine("Child Process Exited: " + process.ProcessName);

            if (childsExited == childCount)
                Console.WriteLine("All child processes have exited.");
        }
    }// end MainWindow

    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            Console.WriteLine("Entering GetChildProcesses (IEnumerable)");
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
                Console.Write("NEW PROCESS: ");
                Console.WriteLine(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
    }// end ProcessExtensions
}
