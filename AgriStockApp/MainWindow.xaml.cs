using AgriStockApp.Properties;
using AgriStockApp.Scripts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AgriStockApp
{
    public delegate void serverUpdateEventHandler();
    public partial class MainWindow : Window
    {
        //Shared Props
        public static dynamic ServerData { get; set; }
        public static dynamic CareerSavegame { get; set; }
        public static dynamic EconomySavegame { get; set; }
        public static dynamic VehiclesSavegame { get; set; }
        public static string Current_FS_Host { get; set; }
        public static string Current_FS_Key { get; set; }

        //Timer
        public static DispatcherTimer ServerTimer;

        //Event
        public static event serverUpdateEventHandler _servUpdate;

        //Ctor
        public MainWindow()
        {
            InitializeComponent();
            
            //Hello :)
            Debug.WriteLine("Hello!");

            //Setting App Language
            this.SetLanguageDictionary();

            //Setting Up Timer
            SetTimer();
                        
            //Setting Landing Page
            pageHolder.DataContext = this.DataContext;
            if (Settings.Default.ServerHost == String.Empty || Settings.Default.ServerKey == String.Empty)
            {
                //Redirect to settings...
                pageHolder.DataContext = new Pages.Settings(true);
            }
            else
            {
                //Get Server Datas
                RefreshXml();

                //Redirect to a nice page...
                pageHolder.DataContext = new Pages.ServerStats();
            }
        }

        //Language Selector
        private void SetLanguageDictionary()
        {
            string lang = Settings.Default.Lang;
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "Fr":
                    //Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("..\\Language\\Fr.xaml", UriKind.Relative) });
                    dict.Source = new Uri("..\\Language\\Fr.xaml", UriKind.Relative);
                    break;
                default:
                    //Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("..\\Language\\Def.xaml", UriKind.Relative) });
                    dict.Source = new Uri("..\\Language\\Def.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        //Timer Setup
        private void SetTimer()
        {
            ServerTimer = new DispatcherTimer();
            ServerTimer.Interval = TimeSpan.FromSeconds(60);
            ServerTimer.Tick += ServerDatasUpdate;
            ServerTimer.Start();
        }

        //ServerTimerTick
        private void ServerDatasUpdate(object sender, EventArgs e)
        {
            Debug.WriteLine("> ServerTimerTicked!");
            if (Settings.Default.ServerHost != String.Empty && Settings.Default.ServerKey != String.Empty)
            {
                RefreshXml();
            }
        }

        //Refresh Server Datas
        public static async void RefreshXml()
        {
            Debug.WriteLine(">> Refreshing Server Datas...");

            //Get server datas
            ServerData = await Task.Run(() => Fonctions.getServerData(FS_Api.Stats(Settings.Default.ServerHost, Settings.Default.ServerKey)));
            Debug.WriteLine(">>> Server API refreshed...");

            //Get server savegames
            CareerSavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Career(Settings.Default.ServerHost, Settings.Default.ServerKey)));
            Debug.WriteLine(">>> Career Savegame refreshed...");
            EconomySavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Economy(Settings.Default.ServerHost, Settings.Default.ServerKey)));
            Debug.WriteLine(">>> Economy Savegame refreshed...");
            VehiclesSavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Vehicles(Settings.Default.ServerHost, Settings.Default.ServerKey)));
            Debug.WriteLine(">>> Vehicles Savegame refreshed...");

            //Trigg Update
            _servUpdate.Invoke();
        }
    }
}
