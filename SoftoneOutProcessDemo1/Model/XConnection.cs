using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Softone;

namespace SoftoneOutProcessDemo1
{
    public class XConnection : BindableBase
    {
        private bool inProcess;
        public bool InProcess
        {
            get => inProcess;
            set
            {
                SetProperty(ref inProcess, value);
            }
        }

        private bool isInEFTPayment;
        public bool IsInEFTPayment
        {
            get => isInEFTPayment;
            set
            {
                SetProperty(ref isInEFTPayment, value);
            }
        }

        private bool isLogedIn;
        public bool IsLogedIn
        {
            get => isLogedIn;
            set
            {
                SetProperty(ref isLogedIn, value);
            }
        }

        private DateTime startedOn;
        public DateTime StartedOn
        {
            get => startedOn;
            set
            {
                SetProperty(ref startedOn, value);
            }
        }

        private bool hasErrors;
        public bool HasErrors
        {
            get => hasErrors;
            set
            {
                SetProperty(ref hasErrors, value);
            }
        }

        private string errorMessage = string.Empty; // Initialize to avoid CS8618  
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                SetProperty(ref errorMessage, value);
            }
        }

        private int jobsCounter;
        public int JobsCounter
        {
            get => jobsCounter;
            set
            {
                SetProperty(ref jobsCounter, value);
            }
        }

        private int ordersCounter;
        private int retaildocCounter;
        private int eftpayCounter;
        private int cfncusdocCounter;

        public int OrdersCounter
        {
            get => ordersCounter;
            set
            {
                SetProperty(ref ordersCounter, value);
            }
        }
        public int RetaildocCounter
        {
            get => retaildocCounter;
            set
            {
                SetProperty(ref retaildocCounter, value);
            }
        }
        public int EftpayCounter
        {
            get => eftpayCounter;
            set
            {
                SetProperty(ref eftpayCounter, value);
            }
        }
        public int CfncusdocCounter
        {
            get => cfncusdocCounter;
            set
            {
                SetProperty(ref cfncusdocCounter, value);
            }
        }

        private bool isMain;
        public bool IsMain
        {
            get => isMain;
            set
            {
                SetProperty(ref isMain, value);
            }
        }

        public enum ModuleEnum { General = 0, Orders = 1, RetailDoc = 2, EftPay = 3, Cfncusdoc = 4, Other = 5 }

        private ModuleEnum moduleType;
        public ModuleEnum ModuleType
        {
            get => moduleType;
            set
            {
                SetProperty(ref moduleType, value);
            }
        }

        public XSupport? X { get; set; } = null!; // Use null-forgiving operator to avoid CS8618  

        public XConnection()
        {
            Globals.mainWindow?.Dispatcher.Invoke(() =>
            {
                try
                {
                    DateTime loginDate;
                    if (Globals.loginDate.HasValue) // Fix for CS8629: Nullable value type may be null  
                    {
                        loginDate = new DateTime(Globals.loginDate.Value.Year, Globals.loginDate.Value.Month, Globals.loginDate.Value.Day);
                    }
                    else
                    {
                        loginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    }

                    X = XSupport.Login(
                        Globals.ConnectionSettings.xcoFilePath,
                        Globals.ConnectionSettings.userName,
                        Globals.ConnectionSettings.password,
                        Globals.ConnectionSettings.company ?? 0, // Fix for CS1503: Convert nullable int to int with default value  
                        Globals.ConnectionSettings.branch ?? 0, // Fix for CS1503: Convert nullable int to int with default value  
                        loginDate,
                        Globals.ConnectionSettings.offline ? 1 : 0
                    );
                    StartedOn = DateTime.Now;
                    IsLogedIn = true;
                }
                catch (Exception ex)
                {
                    HasErrors = true;
                    ErrorMessage = ex.Message;
                }
            });
        }

        public void Disconnect()
        {
            try
            {
                if (X != null)
                {
                    //X.Dispose();
                    X.Close();
                    X = null;
                    IsLogedIn = false;
                }
            }
            catch (Exception ex)
            {
                HasErrors = true;
                ErrorMessage = ex.Message;
            }
        }
    }
}
