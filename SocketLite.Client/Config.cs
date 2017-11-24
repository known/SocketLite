using System.Windows.Forms;

namespace SocketLite.Client
{
    class Config
    {
        public const string KeyClientId = "ClientId";
        public const string KeyAppServerIP = "AppServerIP";
        public const string KeyAppServerPort = "AppServerPort";

        static Config()
        {
            ClientId = ConfigHelper.AppSettings(KeyClientId);
            AppServerIP = ConfigHelper.AppSettings(KeyAppServerIP);
            AppServerPort = ConfigHelper.AppSettings<int>(KeyAppServerPort);
        }

        public static string ClientId { get; private set; }
        public static string AppServerIP { get; private set; }
        public static int AppServerPort { get; private set; }

        public static void Save(string ip, string port)
        {
            var helper = ConfigHelper.OpenConfig(Application.ExecutablePath);
            helper.SetAppSetting(KeyClientId, Utils.GetMac());
            helper.SetAppSetting(KeyAppServerIP, ip);
            helper.SetAppSetting(KeyAppServerPort, port);
            helper.Save();
        }
    }
}
