using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timer = System.Timers.Timer;

namespace CustomerNumberDonwloadTool
{
    public class OverTimer
    {
        private static Timer m_OverTimer;

        public static void start()
        {
            if (m_OverTimer == null)
            {
                m_OverTimer = new Timer(5000)
                {
                    AutoReset = false,
                };
                m_OverTimer.Elapsed += OverTimerThread;
            }
            m_OverTimer.Start();
        }

        public static void Stop()
        {
            if (m_OverTimer != null)
            {
                m_OverTimer.Stop();
            }
        }

        private static void OverTimerThread(object sender, System.Timers.ElapsedEventArgs e)
        {
            JavascriptEvent.OperationOver();

            JavascriptEvent.NewsMessage("等待超时，请重新操作。");
        }
    }
}
