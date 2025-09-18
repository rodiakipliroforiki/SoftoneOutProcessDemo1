using Softone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftoneOutProcessDemo1
{
    public class XModuleRetailDoc: IDisposable
    {
        public XModule module;
        public XConnection connection;
        public XSupport X;
        public XTable RETAILDOC;
        public XTable MTRDOC;
        public XTable ITELINES;
        public XTable VBUFSET;
        public XTable VCARDS;
        private bool disposedValue;

        public XModuleRetailDoc(XConnection.ModuleEnum moduleType)
        {
            connection=Globals.connectionManager.GetXConnection(moduleType);
            if (connection != null)
            {
                connection.InProcess = true;
                X = connection.X;
                module = X.CreateModule("RETAILDOC");
                RETAILDOC = module.GetTable("SALDOC");
                MTRDOC = module.GetTable("MTRDOC");
                ITELINES = module.GetTable("ITELINES");
                VBUFSET = module.GetTable("VBUFSET");
                VCARDS = module.GetTable("VCARDS");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    module.Close();
                    // TODO: dispose managed state (managed objects)
                    //if (VCARDS!=null)
                    //{
                    //    VCARDS.Dispose();
                    //    VCARDS = null;
                    //}
                    //if (VBUFSET!=null)
                    //{
                    //    VBUFSET.Dispose();
                    //    VBUFSET = null;
                    //}
                    //if (ITELINES != null)
                    //{
                    //    ITELINES.Dispose();
                    //    ITELINES = null;
                    //}
                    //if (MTRDOC != null)
                    //{
                    //    MTRDOC.Dispose();
                    //    MTRDOC = null;
                    //}
                    //if (RETAILDOC != null)
                    //{
                    //    RETAILDOC.Dispose();
                    //    RETAILDOC = null;
                    //}
                    //if (module != null)
                    //{
                    //    module.Dispose();
                    //    module = null;
                    //}
                    if (connection != null)
                    {
                        connection.InProcess = false;
                        connection.IsInEFTPayment = false;
                        connection.JobsCounter++;
                    }
                }

                X = null;
                connection = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~XModuleRetailDoc()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
