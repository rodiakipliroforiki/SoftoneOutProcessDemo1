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
        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
            Globals.mainWindow = this;
            Globals.loginDate = tbdate.SelectedDate;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (!string.IsNullOrEmpty(Globals.ConnectionSettings.xdllFilePath))
            {
                XSupport.InitInterop(0, Globals.ConnectionSettings.xdllFilePath);
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
    }
}