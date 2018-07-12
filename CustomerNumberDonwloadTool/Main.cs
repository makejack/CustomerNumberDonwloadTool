using NetDimension.NanUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerNumberDonwloadTool
{
    public partial class Main : WinFormium
    {

        public static WinFormium MainForm;

        public Main()
            : base("http://res.app.local/index.html")
        {
            InitializeComponent();

            MainForm = this;

            LoadHandler.OnLoadEnd += HtmlLoadEnd;

            GlobalObject.AddFunction("ShowDevTools").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    Chromium.ShowDevTools();
                });
            };

            GlobalObject.AddFunction("ChangeSerialConnection").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    string portName = args.Arguments[0].StringValue;
                    if (SerialPortManager.Device.IsOpen)
                    {
                        SerialPortManager.Close();
                    }
                    else
                    {
                        SerialPortManager.Open(portName);
                    }
                    args.SetReturnValue(SerialPortManager.Device.IsOpen);
                });
            };

            GlobalObject.AddFunction("ChangeConnectionState").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    bool state = args.Arguments[0].BoolValue;
                    if (state)
                    {
                        ConnectionManager.Start();
                    }
                    else
                    {
                        ConnectionManager.Stop();
                    }
                });
            };

            GlobalObject.AddFunction("ReconnectDevice").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    ConnectionManager.Start();
                });
            };

            GlobalObject.AddFunction("RefreshClick").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    bool ret = false;
                    try
                    {
                        DataManager.Params.Clear();
                        string deal = PortAgreement.ReadAllCard();
                        SerialPortManager.Write(deal);
                        OverTimer.start();

                        ret = true;
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.ErrorInfo(ex.Message, ex);
                        JavascriptEvent.ErrorMessage(ex.Message);
                    }
                    args.SetReturnValue(ret);
                });
            };


            GlobalObject.AddFunction("DownloadClick").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    string strClientNumber = args.Arguments[0].StringValue;

                    Task.Factory.StartNew(() =>
                    {
                        foreach (Param item in DataManager.Params)
                        {
                            if (item.State == "未设置" && item.DataType == "正常")
                            {
                                string deal = PortAgreement.WriteClientNumber(item.CardNumber, strClientNumber);
                                bool ret = SerialPortManager.Write(deal);
                                if (ret)
                                {
                                    SerialPortManager.OperationResult = OperationResults.None;
                                    for (int i = 0; i < 500; i++)
                                    {
                                        Thread.Sleep(10);
                                        if (SerialPortManager.OperationResult != OperationResults.None)
                                        {
                                            if (SerialPortManager.OperationResult == OperationResults.Success)
                                            {
                                                item.State = "设置成功";

                                                DataManager.ViewListDisplay();
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        JavascriptEvent.OperationOver();
                    });
                });
            };

            GlobalObject.AddFunction("SetDeviceClient").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    string strClientNumber = args.Arguments[0].StringValue;
                    string deal = PortAgreement.EncryptionDevice(strClientNumber);
                    bool ret = SerialPortManager.Write(deal);
                    args.SetReturnValue(ret);
                });
            };

            GlobalObject.AddFunction("SetCardNumber").Execute += (func, args) =>
            {
                this.RequireUIThread(() =>
                {
                    string strOldNumber = args.Arguments[0].StringValue;
                    string strCardNumber = args.Arguments[1].StringValue;
                    string strType = args.Arguments[2].StringValue;
                    string deal = PortAgreement.WriteCardNumber(strOldNumber, strCardNumber,strType);
                    bool ret = SerialPortManager.Write(deal);
                    if (ret)
                    {
                        if (strCardNumber != "797979" || strCardNumber != "123456")
                        {
                            ConfigManager.SetConfig("Number", strCardNumber);
                        }
                    }
                    args.SetReturnValue(ret);
                });
            };

        }

        private void HtmlLoadEnd(object sender, Chromium.Event.CfxOnLoadEndEventArgs e)
        {
            string number = ConfigManager.GetConfig("Number");
            if (!string.IsNullOrEmpty(number))
            {
                JavascriptEvent.InitConfig(number);
            }

            SerialPortManager.InitDevice();
            PortMonitor.Start();
        }
    }
}
