using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpConfig;

namespace CustomerNumberDonwloadTool
{
    public class ConfigManager
    {

        private static readonly string m_ConfigPath = "Config.cfg";

        private static Configuration LoadConfig()
        {
            Configuration cfg;
            if (!File.Exists(m_ConfigPath))
            {
                cfg = new Configuration();
                cfg["General"]["Number"].StringValue = "";
                SaveConfig(cfg);
            }
            else
            {
                cfg = Configuration.LoadFromFile(m_ConfigPath);
            }
            return cfg;
        }

        public static string GetConfig(string strElem)
        {
            Configuration cfg = LoadConfig();
            return cfg["General"][strElem].StringValue;
        }

        public static void SetConfig(string strElem, string value)
        {
            Configuration cfg = LoadConfig();
            cfg["General"][strElem].StringValue = value;
            SaveConfig(cfg);
        }

        private static void SaveConfig(Configuration cfg)
        {
            cfg.SaveToFile(m_ConfigPath);
        }
    }
}
