using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetDimension.NanUI;

namespace CustomerNumberDonwloadTool
{
    public class JavascriptEvent
    {
        public static void InitConfig(string config)
        {
            Main.MainForm.ExecuteJavascript($"InitConfig('{config}')");
        }

        public static void IncrementingNumber()
        {
            Main.MainForm.ExecuteJavascript($"IncrementingNumber()");
        }

        public static void EndMessage(bool ret)
        {
            Main.MainForm.ExecuteJavascript($"EndMessage('{(ret ? "true" : "")}')");
        }

        public static void ErrorMessage(string msg)
        {
            Main.MainForm.ExecuteJavascript($"DisplayErrorMessage('{msg}')");
        }

        public static void NewsMessage(string msg)
        {
            Main.MainForm.ExecuteJavascript($"NewsMessage('{msg}')");
        }

        public static void PortChange()
        {
            List<string> portNames = PortMonitor.GetPortNames;
            string json = Utility.JsonSerializerByArrayData<string>(portNames.ToArray());
            Main.MainForm.ExecuteJavascript($"PortChange('{json}')");
        }

        public static void SerialChange()
        {
            string json = Utility.JsonSerializerBySingleData(SerialPortManager.Device);
            Main.MainForm.ExecuteJavascript($"SerialChange('{json}')");
        }

        public static void OperationOver()
        {
            Main.MainForm.ExecuteJavascript($"FuncBtnChange('{(SerialPortManager.Device.IsOpen ? "" : "true")}')");
        }

        public static void ConnectionFail()
        {
            Main.MainForm.ExecuteJavascript("ConnectionFiall()");
        }

        public static void ViewListDisplay(string json)
        {
            Main.MainForm.ExecuteJavascript($"ListDisplay('{json}')");
        }
    }
}
