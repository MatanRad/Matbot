using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private static string fname = "ACSerial.conf";
        private static int namecap = 10;
        private static string defserial = "COM4";


        public static string GetACSerialName()
        {
            try
            {
                string s = File.ReadAllText(fname);
                if (s.Length <= namecap) return s;
            }
            catch (FileNotFoundException) { }

            return defserial;
            
        }

        public static bool SetACSerialName(string name)
        {
            if (name.Length <= namecap)
            {
                File.WriteAllText(fname, name);
                return true;
            }
            else return false;
            
        }

        public static bool SendAC(int temp, bool power, ACPower level)
        {
            try
            {
                Debug.WriteLine((power ? 50 : 0) + temp);
                SerialPort port = new SerialPort(GetACSerialName(), 9600);
                port.Open();
                char[] c = { (char)((power ? 50 : 0) + temp), (char)level };
                port.Write(c, 0, 2);
                //Debug.WriteLine(port.ReadByte());
                port.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}