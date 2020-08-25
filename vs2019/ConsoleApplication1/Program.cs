using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;


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
            for (int i = 0; i < computer.Hardware.Length; i++)
            {

                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                            return int.Parse(computer.Hardware[i].Sensors[j].Value.ToString());
                            //Console.WriteLine("Temp => "+computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
                    }
                }
            }
            computer.Close();
            return 0;
            
        }
        static void Main(string[] args)
        {
            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;
            SerialPort _serialPort;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM9";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
            while (true)
            {
                int temperature = GetSystemInfo();
                int cpuLoad = (int)cpuCounter.NextValue();
                Console.WriteLine("Temp => :" + GetSystemInfo());
                Console.WriteLine("CPU Usage => "+(int)cpuCounter.NextValue() + "%");
                //string a = _serialPort.ReadExisting();
                //string a = _serialPort.Write("a");
                //_serialPort.Write("a");
                //Thread.Sleep(1000);
                //_serialPort.Write("b");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }
    }
}