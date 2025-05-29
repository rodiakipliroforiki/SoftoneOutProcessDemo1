using Softone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftoneOutProcessDemo1
{
    public class XConnectionManager:BindableBase
    {

        private ObservableCollection<XConnection> connections;
        public ObservableCollection<XConnection> Connections
        {
            get { return connections; }
            set
            {
                connections = value;
                OnPropertyChanged(nameof(Connections));
            }
        }

        public XConnectionManager()
        {
            Connections = new ObservableCollection<XConnection>();
        }

        private int totalConnections;
        public int TotalConnections
        {
            get { return totalConnections; }
            set
            {
                totalConnections = value;
                OnPropertyChanged(nameof(TotalConnections));
            }
        }

        public XConnection GetMainConnection()
        {
            XConnection connection = Connections.FirstOrDefault(c => c.IsMain && c.IsLogedIn);
            if (connection == null)
            {
                connection = new XConnection();
                connection.IsMain = true;
                Connections.Add(connection);
                TotalConnections++;
            }
            return connection;
        }

        internal int timesToSleepIfConnectionsHigh = 5;
        internal int timesSleptOnConnectionsHigh = 0;
        public XConnection GetXConnection(XConnection.ModuleEnum moduleType)
        {
            XConnection? connection = Connections.FirstOrDefault(c => !c.InProcess && c.IsLogedIn && !c.HasErrors && !c.IsInEFTPayment && !c.IsMain && c.ModuleType==moduleType);
            if (connection == null)
            {
                connection = new XConnection();
                connection.ModuleType = moduleType;
                Connections.Add(connection);
                TotalConnections++;
                timesSleptOnConnectionsHigh = 0;
            }
            return connection;
        }

        public void RemoveXConnection(XConnection connection)
        {
            if (connection != null)
            {
                connection.Disconnect();
                Connections.Remove(connection);
            }
        }

        public void CheckConnections()
        {
            List<XConnection> connectionsToRemove = new List<XConnection>();
            foreach (var connection in Connections)
            {
                if ((connection.HasErrors && !connection.InProcess && !connection.IsMain) || ( connection.IsLogedIn && !connection.InProcess  && !connection.IsInEFTPayment && connection.JobsCounter>Globals.MaxCount && !connection.IsMain))
                {
                    connectionsToRemove.Add(connection);
                }
            }
            if (connectionsToRemove.Count > 0)
            {
                foreach (var connection in connectionsToRemove)
                {
                    connection.Disconnect();
                    Connections.Remove(connection);
                    Thread.Sleep(500);
                }                
            }
        }

        public XSupport getX()
        {
            var xcon = GetXConnection(XConnection.ModuleEnum.General);
            xcon.InProcess = true;
            return xcon.X;
        }

        public void ReleaseX(XSupport x)
        {
            var xcon = Connections.FirstOrDefault(c => c.X == x);
            if (xcon != null)
            {
                xcon.InProcess = false;
                xcon.IsInEFTPayment = false;
                xcon.JobsCounter++;
            }
        }

        public void Dispose()
        {
            foreach (var connection in Connections)
            {
                connection.Disconnect();
            }
            Connections.Clear();
        }

        public void CheckMaxConnections()
        {
            CheckConnections();
            int notInProcessCount = Connections.Count(q => !q.InProcess && q.IsLogedIn && !q.HasErrors && !q.IsInEFTPayment);
            if (notInProcessCount > Globals.ConnectionsHighCount)
            {
                var connectionsToRemove = Connections.Where(q => !q.IsMain && !q.InProcess && q.IsLogedIn && !q.HasErrors && !q.IsInEFTPayment).OrderBy(q => q.StartedOn).Take(notInProcessCount - Globals.ConnectionsLowCount).ToList();
                lock (Connections)
                {
                    foreach (var connection in connectionsToRemove)
                    {
                        connection.Disconnect();
                        Connections.Remove(connection);
                        Thread.Sleep(500);
                    }                    
                }
            }
        }

        //public XModuleOrder GetXModuleOrder()
        //{            
        //    XConnection connection = Connections.FirstOrDefault(c => !c.InProcess && c.IsLogedIn && !c.HasErrors && !c.IsInEFTPayment && !c.IsMain && c.ModuleType == XConnection.ModuleEnum.Orders && c.OrderModule!=null);
        //    if (connection == null)
        //    {
        //        connection = new XConnection();
        //        connection.ModuleType = XConnection.ModuleEnum.Orders;
        //        Connections.Add(connection);
        //        TotalConnections++;
        //        timesSleptOnConnectionsHigh = 0;
        //        connection.OrderModule = new XModuleOrder(connection);
        //    }
        //    return connection.OrderModule;
        //}

    }
}
