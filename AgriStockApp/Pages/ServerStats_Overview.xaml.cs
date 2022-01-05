using AgriStockApp.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgriStockApp.Pages
{
    public partial class ServerStats_Overview : UserControl, INotifyPropertyChanged
    {
        //Props Server
        private string createdDateData;
        public string CreatedDateData
        {
            get { return createdDateData; }
            set { createdDateData = value; Update("CreatedDateData"); }
        }

        private string mapNameData;
        public string MapNameData
        {
            get { return mapNameData; }
            set { mapNameData = value; Update("MapNameData"); }
        }

        private string saveDateData;
        public string SaveDateData
        {
            get { return saveDateData; }
            set { saveDateData = value; Update("SaveDateData"); }
        }

        private string slotsUsageData;
        public string SlotsUsageData
        {
            get { return slotsUsageData; }
            set { slotsUsageData = value; Update("SlotsUsageData"); }
        }

        //Fields
        Window window;
        ContentControl statsPageHolder;

        //Event
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Update(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        //Ctor
        public ServerStats_Overview(string xmlData)
        {
            InitializeComponent();
            Debug.WriteLine(">>> Opening ServerStats_Overview...");
            this.DataContext = this;

            //Trigger
            this.Loaded += Ready;
        }

        //Functions
        private void Ready(object sender, RoutedEventArgs e)
        {
            MainWindow._servUpdate += new serverUpdateEventHandler(AutoRefresh);
            statsPageHolder = (ContentControl)this.Parent;
            if (MainWindow.CareerSavegame == "off" || MainWindow.CareerSavegame == "idle" || MainWindow.CareerSavegame == "error") { return; }

            SetData(JsonConvert.DeserializeObject(MainWindow.CareerSavegame));
            SetMinimap();
        }

        private void AutoRefresh()
        {
            //If not active page => Skip
            window = Window.GetWindow(this);
            if (window == null) return;

            //Else
            SetData(JsonConvert.DeserializeObject(MainWindow.CareerSavegame));
            SetMinimap();
        }

        internal void SetData(dynamic json)
        {
            string xData = MainWindow.ServerData;
            //if offline
            if (xData == "error")
            {
                return;
            }
            //Else
            dynamic serverData = JsonConvert.DeserializeObject(xData);
            MapNameData = serverData.server.mapName;

            //dynamic serverData = JsonConvert.DeserializeObject(json);
            dynamic data = json.careerSavegame.settings;

            CreatedDateData = data.creationDate;
            SaveDateData = data.saveDate;
            SlotsUsageData = json.careerSavegame.slotSystem.slotUsage;
        }

        internal void SetMinimap()
        {
            MiniMap.Source = new ImageSourceConverter().ConvertFromString(FS_Api.Map(Properties.Settings.Default.ServerHost, Properties.Settings.Default.ServerKey)) as ImageSource;
        }

        //Actions
        private void MiniMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ServerStats.NavFrom = "ServerStats_Map";
            statsPageHolder.DataContext = new ServerStats_Map(MainWindow.ServerData);
        }
    }
}
