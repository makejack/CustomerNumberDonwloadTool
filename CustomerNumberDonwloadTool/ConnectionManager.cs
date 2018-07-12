using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Threading;

namespace CustomerNumberDonwloadTool
{
    public class ConnectionManager
    {

        private static Timer m_ConnectionTimer;

        private static bool m_TimerEnabled;
        private static bool m_StopTimer;
        private static bool m_ConnectionDevice;
        private static SerialPortEx m_Serial;
        private static List<byte> m_Bytes;


        public static void Start()
        {
            if (m_Bytes == null)
            {
                m_Bytes = new List<byte>();
            }
            if (m_ConnectionTimer == null)
            {
                m_ConnectionTimer = new Timer(100)
                {
                    AutoReset = false,
                };
                m_ConnectionTimer.Elapsed += ConnectionDeviceThread;
            }
            if (!m_TimerEnabled)
            {
                m_ConnectionTimer.Enabled = true;
            }
        }

        public static void Stop()
        {
            if (m_TimerEnabled)
            {
                m_StopTimer = true;
            }
        }

        private static void ConnectionDeviceThread(object sender, ElapsedEventArgs e)
        {
            m_TimerEnabled = true;

            try
            {
                if (SerialPortManager.Device.IsOpen)
                {
                    return;
                }
                string deal = PortAgreement.EncryptionDevice();
                for (int i = 0; i < 3; i++)
                {
                    if (m_StopTimer) return;
                    foreach (string item in PortMonitor.GetPortNames)
                    {
                        if (m_StopTimer)
                        {
                            return;
                        }
                        if (m_Serial == null)
                        {
                            m_Serial = InitSerial();
                        }
                        m_Serial.PortName = item;
                        try
                        {
                            m_Serial.Open();
                            m_Serial.Write(deal);
                            bool ret = WaitResult();
                            if (ret)
                            {
                                m_Serial.DataReceived -= SerialDataReceived;
                                Thread.Sleep(10);
                                SerialPortManager.SetSerial(m_Serial);
                                return;
                            }
                            m_Serial.Close();
                        }
                        catch (Exception ex)
                        {
                            Log4Helper.ErrorInfo(ex.Message, ex);
                        }
                    }
                }

                if (!SerialPortManager.Device.IsOpen)
                {
                    JavascriptEvent.ConnectionFail();
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                JavascriptEvent.ErrorMessage(ex.Message);
            }
            finally
            {
                m_Serial = null;
                m_ConnectionDevice = false;
                m_TimerEnabled = false;
                m_StopTimer = false;
            }
        }

        private static SerialPortEx InitSerial()
        {
            SerialPortEx serial = new SerialPortEx()
            {
                BaudRate = WinApi.B38400,
                DataBit = WinApi.BIT_8,
                StopBit = WinApi.STOP_1,
                Parity = WinApi.p_NONE,
            };
            serial.DataReceived += SerialDataReceived;
            return serial;
        }

        private static bool WaitResult()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(50);
                if (m_ConnectionDevice)
                {
                    return true;
                }
            }
            return false;
        }

        private static void SerialDataReceived(int port)
        {
            Thread.Sleep(5);
            try
            {
                int len = m_Serial.GetIqueue;
                if (len <= 0)
                {
                    try
                    {
                        if (m_Bytes[0] == 160)
                        {
                            m_ConnectionDevice = true;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_Bytes.Clear();
                    }
                    return;
                }
                byte[] by = m_Serial.Read(len);
                m_Bytes.AddRange(by);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
        }
    }
}
