using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.Devices;

namespace CustomerNumberDonwloadTool
{
    public class PortMonitor
    {
        private static System.Timers.Timer m_TsearchPort;
        private static Mutex m_Mutex;
        private static Computer m_Computer;
        private static int m_PortCount = 0;

        public static List<string> GetPortNames
        {
            get
            {
                if (m_Computer == null)
                {
                    throw new Exception("未实例化");
                }
                return new List<string>(m_Computer.Ports.SerialPortNames);
            }
        }

        public static void Start()
        {
            if (m_Computer == null)
            {
                m_Computer = new Computer();
            }
            if (m_TsearchPort == null)
            {
                m_Mutex = new Mutex();
                m_TsearchPort = new System.Timers.Timer(250)
                {
                    AutoReset = true,
                };
                m_TsearchPort.Elapsed += Moitor;
            }
            m_TsearchPort.Start();
        }

        private static void Moitor(object sender, ElapsedEventArgs e)
        {
            m_Mutex.WaitOne();
            if (m_Computer.Ports.SerialPortNames.Count != m_PortCount)
            {
                try
                {
                    m_PortCount = m_Computer.Ports.SerialPortNames.Count;

                    JavascriptEvent.PortChange();

                    if (SerialPortManager.Device.IsOpen)
                    {
                        try
                        {
                            bool exist = m_Computer.Ports.SerialPortNames.Contains<string>(SerialPortManager.Device.PortName);
                            if (!exist)
                            {
                                SerialPortManager.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4Helper.ErrorInfo(ex.Message, ex);
                            JavascriptEvent.ErrorMessage(ex.Message);
                        }
                        finally
                        {
                            JavascriptEvent.SerialChange();
                        }
                    }

                    if (!SerialPortManager.Device.IsOpen)
                    {
                        ConnectionManager.Start();
                    }
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    JavascriptEvent.ErrorMessage(ex.Message);
                }
            }
            m_Mutex.ReleaseMutex();
        }

        public static void Stop()
        {
            if (m_TsearchPort != null)
            {
                m_TsearchPort.Stop();
            }
        }
    }
}
