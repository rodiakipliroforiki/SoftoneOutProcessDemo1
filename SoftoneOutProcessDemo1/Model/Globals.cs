using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftoneOutProcessDemo1
{
    internal static class Globals
    {
        public static ConnectionSettingsModel ConnectionSettings { get; set; } = new ConnectionSettingsModel();
        public static XConnectionManager connectionManager=new XConnectionManager();
        public static MainWindow? mainWindow = null;
        public static DateTime? loginDate = null;
        public static int MaxCount = 40;
        public static int ConnectionsHighCount = 10;
        public static int ConnectionsLowCount = 1;
    }
}
