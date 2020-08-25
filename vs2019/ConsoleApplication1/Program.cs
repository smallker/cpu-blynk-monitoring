using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Management;

namespace Get_CPU_Temp5
{
    class Program
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
        static int GetSystemInfo()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.Accept(updateVisitor);
            foreach (IHardware v in computer.Hardware)
            {

                if (v.HardwareType != HardwareType.CPU)
                {
                    continue;
                }
                for (int j = 0; j < v.Sensors.Length; j++)
                {
                    if (v.Sensors[j].SensorType == SensorType.Temperature)
                        return int.Parse(v.Sensors[j].Value.ToString());
                    //Console.WriteLine("Temp => "+computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
                }
            }
            computer.Close();
            return 0;

        }
        static void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                               mboShutdownParams, null);
            }
        }
        static void Restart()
        {
            var cmd = new ProcessStartInfo("shutdown.exe", "-r -t 0");
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.ErrorDialog = false;
            Process.Start(cmd);
        }
        static void Main(string[] args)
        {
        start:
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("Port tersedia :");
            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
            Console.Write("Masukkan nama port (ex COM9) : ");
            String comPort = Console.ReadLine();
            PerformanceCounter cpuCounter;
            //PerformanceCounter ramCounter;
            SerialPort _serialPort;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _serialPort = new SerialPort();
            _serialPort.PortName = comPort;
            _serialPort.BaudRate = 9600;
            try
            {
                _serialPort.Open();
            }
            catch
            {
                Console.WriteLine("Port tidak tersedia\r\n");
                goto start;
            }

            try
            {
                while (true)
                {
                    int temperature = GetSystemInfo();
                    int cpuLoad = (int)cpuCounter.NextValue();
                    string cmd = _serialPort.ReadExisting();
                    string date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                    Console.WriteLine("\r\n\r\n");
                    Console.WriteLine(date);
                    Console.WriteLine("Suhu CPU         : " + GetSystemInfo() + " °C");
                    Console.WriteLine("Penggunaan CPU   : " + cpuLoad + " %");
                    if (cmd.Contains("a"))
                    {
                        Console.WriteLine(">>>>>>>> Komputer akan dimatikan <<<<<<<<<<<");
                        Shutdown();
                        break;
                    }
                    if (cmd.Contains("b"))
                    {
                        Console.WriteLine(">>>>>> Komputer akan restart <<<<<<<<<");
                        Restart();
                        break;
                    }
                    _serialPort.Write("{\"load\":" + cpuLoad.ToString() + ",\"temp\":" + temperature.ToString() + "}");
                    _serialPort.Write("\n");
                    Thread.Sleep(500);
                }
            }
            catch
            {
                Console.WriteLine("Restart program dan jalankan sebagai administrator");
            }
            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}