using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerNumberDonwloadTool
{
    public class SerialPortManager
    {
        public static SerialPortEx Device;
        public static OperationResults OperationResult;
        private static List<byte> m_Bytes;

        public static void InitDevice()
        {
            Device = new SerialPortEx()
            {
                BaudRate = WinApi.B38400,
                DataBit = WinApi.BIT_8,
                StopBit = WinApi.STOP_1,
                Parity = WinApi.p_NONE,
            };
            Device.DataReceived += SerialDataReceived;
        }

        private static void SerialDataReceived(int port)
        {
            Thread.Sleep(10);
            try
            {
                int len = Device.GetIqueue;
                if (len <= 0)
                {
                    try
                    {
                        OverTimer.Stop();
                        switch (m_Bytes[0])
                        {
                            case 10:
                                if (m_Bytes[1] == 8)
                                {
                                    JavascriptEvent.OperationOver();
                                }
                                else
                                {
                                    DataManager.VerifyingRepetition(m_Bytes);

                                    OverTimer.start();
                                }
                                break;
                            case 30:
                                if (m_Bytes[1] == 8)
                                {
                                    OperationResult = OperationResults.Fail;
                                }
                                else
                                {
                                    OperationResult = OperationResults.Success;
                                }
                                break;
                            case 43:
                                bool ret = m_Bytes[1] != 8;
                                JavascriptEvent.EndMessage(ret);
                                if (ret)
                                {
                                    JavascriptEvent.IncrementingNumber();
                                }
                                break;
                            case 160:
                                JavascriptEvent.OperationOver();
                                if (m_Bytes[1] == 0)
                                {
                                    JavascriptEvent.NewsMessage("发行器编号设置成功。");
                                }
                                else
                                {
                                    JavascriptEvent.NewsMessage("发行器编号设置失败，请重新操作。");
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.ErrorInfo(ex.Message, ex);
                        JavascriptEvent.ErrorMessage(ex.Message);
                    }
                    finally
                    {
                        m_Bytes.Clear();
                    }
                    return;
                }
                byte[] by = Device.Read(len);
                if (m_Bytes == null)
                {
                    m_Bytes = new List<byte>();
                }
                m_Bytes.AddRange(by);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                JavascriptEvent.ErrorMessage(ex.Message);
            }
        }

        public static bool Write(string deal)
        {
            try
            {
                if (Device.IsOpen)
                {
                    Device.Write(deal);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                JavascriptEvent.ErrorMessage(ex.Message);
            }
            return false;
        }

        public static bool Open(string strPortName)
        {
            Device.PortName = strPortName;
            try
            {
                Device.Open();

                return true;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                JavascriptEvent.ErrorMessage(ex.Message);
            }
            return false;
        }

        public static bool Close()
        {
            try
            {
                Device.Close();

                return true;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                JavascriptEvent.ErrorMessage(ex.Message);
            }
            return false;
        }

        public static void SetSerial(SerialPortEx serial)
        {
            Device = serial;
            Device.DataReceived += SerialDataReceived;

            JavascriptEvent.SerialChange();
        }
    }
}
