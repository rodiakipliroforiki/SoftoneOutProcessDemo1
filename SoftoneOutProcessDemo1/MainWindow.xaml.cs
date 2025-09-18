using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Softone;

namespace SoftoneOutProcessDemo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.Form helperForm = null;
        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
            Globals.mainWindow = this;
            Globals.loginDate = tbdate.SelectedDate;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (!string.IsNullOrEmpty(Globals.ConnectionSettings.xdllFilePath))
            {
                helperForm=new System.Windows.Forms.Form();
                if (helperForm != null)
                {
                    XSupport.InitInterop(helperForm.Handle.ToInt32(), Globals.ConnectionSettings.xdllFilePath);
                }
            }
        }

        private void btnGetXdll_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "xdll"; // Default file name
            dialog.DefaultExt = ".dll"; // Default file extension
            dialog.Filter = "xdll (.dll)|*.dll"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                tbxdll.Text = filename;
            }
        }

        private void btnGetXco_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "softone"; // Default file name
            dialog.DefaultExt = ".xco"; // Default file extension
            dialog.Filter = "xco files (.xco)|*.xco"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                tbxco.Text = filename;
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        internal void SaveSettings()
        {
            Globals.ConnectionSettings.xdllFilePath = tbxdll.Text;
            Globals.ConnectionSettings.xcoFilePath = tbxco.Text;
            Globals.ConnectionSettings.userName = tbusername.Text;
            Globals.ConnectionSettings.password = tbpassword.Password;
            if (tbcompany.Text != null && tbcompany.Text.Length > 0)
            {
                Globals.ConnectionSettings.company = Convert.ToInt32(tbcompany.Text);
            }
            if (tbbranch.Text != null && tbbranch.Text.Length > 0)
            {
                Globals.ConnectionSettings.branch = Convert.ToInt32(tbbranch.Text);
            }
            if (tboffline.IsChecked==true)
            {
                Globals.ConnectionSettings.offline = true;
            }
            string fileName = "S1appsettings.json";
            string jsonString = JsonSerializer.Serialize(Globals.ConnectionSettings);
            File.WriteAllText(fileName, jsonString);
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnReadSettings_Click(object sender, RoutedEventArgs e)
        {
           ReadSettings();
        }

        internal void ReadSettings()
        {
            string fileName = "S1appsettings.json";
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                if (jsonString!=null && !string.IsNullOrEmpty(jsonString))
                {
                    ConnectionSettingsModel? c = JsonSerializer.Deserialize<ConnectionSettingsModel>(jsonString);
                    if (c != null)
                    {
                        Globals.ConnectionSettings.CopyFrom(c);
                        tbxdll.Text = Globals.ConnectionSettings.xdllFilePath;
                        tbxco.Text = Globals.ConnectionSettings.xcoFilePath;
                        tbusername.Text = Globals.ConnectionSettings.userName;
                        tbpassword.Password = Globals.ConnectionSettings.password;
                        tbcompany.Text = Globals.ConnectionSettings.company.ToString();
                        tbbranch.Text = Globals.ConnectionSettings.branch.ToString();
                        tboffline.IsChecked = Globals.ConnectionSettings.offline;
                    }

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void btTest1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //XSupport.InitInterop(0, Globals.ConnectionSettings.xdllFilePath);
                Globals.connectionManager.GetMainConnection();
                rtbLog.AppendText("Main connection established.\n");
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Error establishing main connection: {ex.Message}\n");
            }
        }

        private void btTest10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.connectionManager.GetMainConnection();
                rtbLog.AppendText("Main connection established.\n");

                for (int i = 0; i < 10; i++)
                {
                    XConnection a=Globals.connectionManager.GetXConnection(XConnection.ModuleEnum.General);
                    a.InProcess = true;
                    var b=a?.X?.SQL("select TOP 1 MTRL from MTRL WHERE COMPANY=:X.SYS.COMPANY AND SODTYPE=51 AND ISACTIVE=1", new object[] {a?.X?.ConnectionInfo.CompanyId });
                    var c = a?.X?.CreateModule("ITEM");
                    if (b != null) c?.LocateData(Convert.ToInt32(b));
                    long TotalMemoryUsed = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; //in MB
                    rtbLog.AppendText($"Main connection {i + 1} established. Memory used {TotalMemoryUsed} MB Total connections {Globals.connectionManager.Connections.Count} \n");
                    b = null;
                    c.Dispose();
                    c = null;
                }


                //Globals.connectionManager.Dispose();
                //long TotalMemoryUsed1 = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; //in MB

                //rtbLog.AppendText($"disconnecting.. Memory used {TotalMemoryUsed1} MB Total connections {Globals.connectionManager.Connections.Count} \n");


                foreach (var con in Globals.connectionManager.Connections.ToList())
                {
                    con.InProcess = false;
                    con.Disconnect();
                    long TotalMemoryUsed = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; //in MB

                    rtbLog.AppendText($"disconnecting.. Memory used {TotalMemoryUsed} MB Total connections {Globals.connectionManager.Connections.Count} \n");
                    Globals.connectionManager.RemoveXConnection(con);
                    //Thread.Sleep(5000);
                }
                //Globals.connectionManager.RemoveXConnection()
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Error establishing main connection: {ex.Message}\n");
            }
        }

        private void LogMemoryUsage(string stage)
        {
            long TotalMemoryUsed = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; //in MB
            this.Dispatcher.Invoke((Action)delegate
            {
                if (((string)(new TextRange(rtbLog.Document.ContentStart, rtbLog.Document.ContentEnd).Text)).Length > 10000)
                {
                    rtbLog.Document.Blocks.Clear();
                }
                rtbLog.AppendText($"{stage}, Memory {TotalMemoryUsed} MB, connections={Globals.connectionManager.Connections.Count} \n");
                if (cbScroll.IsChecked==true)
                {
                    rtbLog.ScrollToEnd();
                }
            });
        }

        Queue<int> findocsToOpen = new Queue<int>();
        private System.Timers.Timer _historyTimer = null;
        private int iterations = 0;
        private void btTest100_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMemoryUsage("Starting");
                XConnection xcon = Globals.connectionManager.GetMainConnection();
                if (cbSQLMonitor.IsChecked == true)
                {
                    xcon.X.ExecS1Command("ACMD:acSQLMonitor", helperForm);
                }

                rtbLog.AppendText("Main connection established.\n");
                findocsToOpen.Clear();
                using (XTable t = xcon.X.GetSQLDataSet("SELECT TOP 3000 FINDOC FROM FINDOC WHERE SOSOURCE=1351 AND SOREDIR=10000 AND COMPANY=:1 AND BRANCH=:2 AND FISCPRD=:3 ORDER BY FINDOC DESC", new object[] { xcon.X.ConnectionInfo.CompanyId, xcon.X.ConnectionInfo.BranchId, xcon.X.ConnectionInfo.YearId }))
                {
                    if (t.Count > 0)
                    {
                        for (int j = 0; j < t.Count; j++)
                        {
                            findocsToOpen.Enqueue(Convert.ToInt32(t[j, "FINDOC"]));
                        }
                    }
                }
                LogMemoryUsage("After finding retaildocs");

                if (_historyTimer == null)
                {
                    _historyTimer = new System.Timers.Timer();
                }
                _historyTimer.Interval = Globals.TestsMin;
                _historyTimer.Elapsed += (s, ev) =>
                {
                    if (findocsToOpen.Count > 0)
                    {
                        // this.Dispatcher.Invoke((Action)delegate
                        //{
                        try
                        {
                            int currentiteration = Interlocked.Increment(ref iterations);
                            LogMemoryUsage($"+# {currentiteration}");
                            XModuleRetailDoc doc = new XModuleRetailDoc(XConnection.ModuleEnum.RetailDoc);
                            int findoc = findocsToOpen.Dequeue();
                            doc.module.LocateData(findoc);
                            doc.RETAILDOC.Current.Edit(0);
                            doc.RETAILDOC.Current["COMMENTS1"] = $"Edited at {DateTime.Now.ToLongTimeString()}";
                            doc.RETAILDOC.Current.Post();
                            doc.module.PostData();
                            doc.Dispose();
                            LogMemoryUsage($"-# {currentiteration}");
                        } 
                        catch(Exception ex)
                        {
                            LogMemoryUsage($"error # {ex.Message}");
                        }
                        //});
                        Random r = new Random();
                        int rInt = r.Next(Globals.TestsMin, Globals.TestsMax);
                        _historyTimer.Interval = rInt;
                    }
                    else
                    {
                        _historyTimer.Stop();                        
                    }
                };
                _historyTimer.Start();

            } catch(Exception ex)
            {
                rtbLog.AppendText($"error 100: {ex.Message}");
            }
        }
    }
}