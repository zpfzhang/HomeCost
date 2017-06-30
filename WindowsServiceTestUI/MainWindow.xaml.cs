using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
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

namespace WindowsServiceTestUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = currentDirectory + "\\Service";
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "Install.bat";
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            System.Environment.CurrentDirectory = currentDirectory;
        }

        private void UnInstallBtn_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = currentDirectory + "\\Service";
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "Uninstall.bat";
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            System.Environment.CurrentDirectory = currentDirectory;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceController serviceController = new ServiceController("HomeCostService");
            serviceController.Start();
            PauseBtn.Content = "Pause";
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceController serviceController = new ServiceController("HomeCostService");
            if (serviceController.CanStop)
            {
                serviceController.Stop();
                PauseBtn.Content = "Continue";
            }            
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceController serviceController = new ServiceController("HomeCostService");
            if (serviceController.CanPauseAndContinue)
            {
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    serviceController.Pause();
                    this.Content = "Continue";
                }
                else if (serviceController.Status == ServiceControllerStatus.Paused)
                {
                    serviceController.Continue();
                    this.Content = "Pause";
                }

            }
        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceController serviceController = new ServiceController("HomeCostService");
            string status = serviceController.Status.ToString();
            MessageBox.Show(status);
        }
    }
}
