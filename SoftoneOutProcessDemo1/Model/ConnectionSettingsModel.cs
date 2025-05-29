using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftoneOutProcessDemo1
{
    internal class ConnectionSettingsModel
    {
        public string? xdllFilePath { get; set; }
        public string? xcoFilePath { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public int? company { get; set; }
        public int? branch { get; set; }
        public bool offline { get; set; } = false;

        internal void CopyFrom(ConnectionSettingsModel c)
        {
            this.xdllFilePath = c.xdllFilePath;
            this.xcoFilePath = c.xcoFilePath;
            this.userName = c.userName;
            this.password = c.password;
            this.company = c.company;
            this.branch = c.branch;
            this.offline = c.offline;
        }
    }
}
