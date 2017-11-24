using System;
using System.Configuration;

namespace SocketLite
{
    public class ConfigHelper
    {
        private Configuration config;

        public ConfigHelper(Configuration config)
        {
            this.config = config ?? throw new ArgumentNullException("config");
        }

        public void SetAppSetting(string key, string value)
        {
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
        }

        public void Save()
        {
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static ConfigHelper OpenConfig(string exePath)
        {
            var config = ConfigurationManager.OpenExeConfiguration(exePath);
            return new ConfigHelper(config);
        }

        public static string AppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static T AppSettings<T>(string key)
        {
            var value = AppSettings(key);
            return Utils.ConvertTo<T>(value);
        }
    }
}
