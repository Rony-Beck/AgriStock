using System.Windows;
using System.Windows.Controls;

namespace AgriStockApp.Pages
{
    public partial class Settings : UserControl
    {
        //Props
        public bool FirstLaunch { get; set; }
        
        //Ctor
        public Settings(bool firstLaunch)
        {
            InitializeComponent();
            this.DataContext = this;
            FirstLaunch = firstLaunch;
            FillFields();
        }

        //Functions
        internal void FillFields()
        {
            //Fill Hostname & Server Key
            HostNameForm.Text = Properties.Settings.Default.ServerHost;
            ServerKeyForm.Password = Properties.Settings.Default.ServerKey;

            //Select Language
            if (Properties.Settings.Default.Lang == "En") { ENButton.IsChecked = true; }
            if (Properties.Settings.Default.Lang == "Fr") { FRButton.IsChecked = true; }
        }

        //Controls
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Disable Button
            SaveButton.IsEnabled = false;
            
            //Set Host Configuration
            Properties.Settings.Default.ServerHost = HostNameForm.Text;
            Properties.Settings.Default.ServerKey = ServerKeyForm.Password; 
            
            //Set Language
            if (ENButton.IsChecked == true) { Properties.Settings.Default.Lang = "En"; }
            if (FRButton.IsChecked == true) { Properties.Settings.Default.Lang = "Fr"; }

            //Save Settings
            Properties.Settings.Default.Save();

            //End
            if (FirstLaunch == true)
            {
                SaveButton.Content = (string)Application.Current.FindResource("restartApp");
            }
            else
            {
                SaveButton.IsEnabled = true;
            }
        }
    }
}
