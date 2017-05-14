using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    enum ACPower
    {
        low, med, high, auto, turbo
    }


    class ACManager
    {
        public static string PortName = "COM3";

        public static bool SendAC(int temp, bool power, ACPower level)
        {
            try
            {
                Debug.WriteLine((power ? 50 : 0) + temp);
                SerialPort port = new SerialPort(PortName, 9600);
                port.Open();
                char[] c = { (char)((power ? 50 : 0) + temp) , (char)level};
                port.Write(c, 0, 2);
                //Debug.WriteLine(port.ReadByte());
                port.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}

