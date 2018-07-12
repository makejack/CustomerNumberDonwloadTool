using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpConfig;

namespace CustomerNumberDonwloadTool
{
    public class DataManager
    {
        public static List<Param> Params = new List<Param>();

        public static bool VerifyingRepetition(List<byte> list)
        {
            string strNumber = GetCardNumbr(list);
            bool ret = Params.Where(e => e.CardNumber == strNumber).Count() == 0;
            if (ret)
            {
                string dataType = list[5] == 224 ? "密码错误或编号不正确" : "正常";
                Params.Add(new Param(strNumber, dataType));
                ViewListDisplay();
            }
            return ret;
        }

        public static void ViewListDisplay()
        {
            string json = Utility.JsonSerializerByArrayData(Params.ToArray());
            JavascriptEvent.ViewListDisplay(json);
        }

        private static string GetCardNumbr(List<byte> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                sb.AppendFormat("{0:X2}", list[2 + i]);
            }
            return sb.ToString();
        }

    }

    public enum OperationResults
    {
        None = 0,
        Fail,
        Success,
    }

    [System.Runtime.Serialization.DataContract]
    public class Param
    {
        public Param(string number, string dataType)
        {
            this.CardNumber = number;
            this.State = "未设置";
            this.DataType = dataType;
        }

        [System.Runtime.Serialization.DataMember]
        public string CardNumber { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string State { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string DataType { get; set; }
    }
}
