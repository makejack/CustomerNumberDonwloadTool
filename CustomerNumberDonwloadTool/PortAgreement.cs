using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerNumberDonwloadTool
{
    public class PortAgreement
    {
        public static string EncryptionDevice(string number = "9887", string pwd = "766554")
        {
            return $"A00001000000{number}{pwd}";
        }

        public static string ReadAllCard()
        {
            return "0A8000000000010003";
        }

        public static string WriteClientNumber(string cardnumber, string clientnumber)
        {
            return $"1E01{cardnumber}01000000{clientnumber}00766554766554766554";
        }

        public static string WriteCardNumber(string oldnumber, string number,string type)
        {
            return $"2B00{oldnumber}{type}000000{number}988711766554766554766554";
        }
    }
}
