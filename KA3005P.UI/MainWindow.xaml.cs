using System.Windows;

namespace KA3005P.UI
{
    public partial class MainWindow : Window
    {
        private MainWindowModel Model
        {
            get;
            set;
        }
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Model = new MainWindowModel();
            this.DataContext = this.Model;
            this.Left = this.Model.WindowLeft;
            this.Top = this.Model.WindowTop;
        }

        private void btnSetVoltage_Click(object sender, RoutedEventArgs e)
        {
            double value = 0.0;
            if(double.TryParse(this.txtSetVoltageValue.Text, out value))
            {
                this.Model.Voltage = value;
            }
        }

        private void btnOutputEnabled_Click(object sender, RoutedEventArgs e)
        {
            this.Model.OutputEnabled = !this.Model.OutputEnabled;
            return;
        }
        private void btnVoltageFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            this.Model.BrowseVoltageFile();
            return;
        }
        private void btnVoltageFileStart_Click(object sender, RoutedEventArgs e)
        {
            if(this.Model.VoltageOutputFileStatus == MainWindowModel.VoltageOutputFileStatuses.IsOuputting)
                this.Model.StopVoltageFile();
            else
                this.Model.StartVoltageFile();
            return;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Model.OutputEnabled = false;
            this.Model.WindowLeft = this.Left;
            this.Model.WindowTop = this.Top;
            return;
        }

        private void mnuitmRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.Model.UpdateStatus();
            return;
        }

        private void mnuitmReconnect_Click(object sender, RoutedEventArgs e)
        {
            this.Model.ConnectKorad();
            return;
        }
    }
}
