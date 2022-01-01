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
            
            //Setting App Language
            this.SetLanguageDictionary();

            //Setting Up Timer
            SetTimer();
            
            Debug.WriteLine("Hello!");
            
            //Will be removed later
            Current_FS_Host = Config.FS_Host;
            Current_FS_Key = Config.FS_Key;

            //Get Server Datas
            RefreshXml();

            Debug.WriteLine("Ready to serve.");

            //Setting Landing Page
            pageHolder.DataContext = this.DataContext;
            pageHolder.DataContext = new Pages.ServerStats(); ;
        }

        //Language Selector
        private void SetLanguageDictionary()
        {
            string lang = "fr";
            ResourceDictionary dict = new ResourceDictionary();
            switch (lang)
            {
                case "fr":
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
            RefreshXml();
        }

        //Refresh Server Datas
        public static async void RefreshXml()
        {
            Debug.WriteLine(">> Refreshing Server Datas...");

            //Get server datas
            ServerData = await Task.Run(() => Fonctions.getServerData(FS_Api.Stats(Current_FS_Host, Current_FS_Key)));
            Debug.WriteLine(">>> Server API refreshed...");

            //Get server savegames
            CareerSavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Career(Current_FS_Host, Current_FS_Key)));
            Debug.WriteLine(">>> Career Savegame refreshed...");
            EconomySavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Economy(Current_FS_Host, Current_FS_Key)));
            Debug.WriteLine(">>> Economy Savegame refreshed...");
            VehiclesSavegame = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Vehicles(Current_FS_Host, Current_FS_Key)));
            Debug.WriteLine(">>> Vehicles Savegame refreshed...");

            //Trigg Update
            _servUpdate.Invoke();
        }
    }
}
