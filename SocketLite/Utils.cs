using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Management;

namespace SocketLite
{
    public class Utils
    {
        public static T ConvertTo<T>(object value, T defaultValue = default(T))
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            var valueString = value.ToString();
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return defaultValue;

            if (type.IsEnum)
                return (T)Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            try
            {
                return (T)Convert.ChangeType(valueString, type);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string Serialize(object obj)
        {
            if (obj == null)
                return string.Empty;

            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static BackgroundWorker ExecuteAsync(AsyncContext context, Action<DoWorkEventArgs> doAction, Action<RunWorkerCompletedEventArgs> completeAction)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, e) =>
            {
                doAction?.Invoke(e);
            };
            backgroundWorker.RunWorkerCompleted += (o, e) =>
            {
                completeAction?.Invoke(e);
            };
            backgroundWorker.RunWorkerAsync(context);
            return backgroundWorker;
        }

        public static string GetMac()
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                {
                    return mo["MacAddress"].ToString();
                }
            }
            return string.Empty;
        }
    }
}
