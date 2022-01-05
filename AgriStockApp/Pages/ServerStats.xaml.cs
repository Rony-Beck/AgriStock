using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AgriStockApp.Pages
{
    public partial class ServerStats : UserControl
    {
        //Props
        public static string NavFrom { get; set; }
        public bool ColdStart { get; set; }

        //Ctor
        public ServerStats()
        {
            InitializeComponent();
            
            Debug.WriteLine(">> Accessing ServerStats...");
            ColdStart = true;
            statsPageHolder.DataContext = this.DataContext;

            //Update Trigger
            MainWindow._servUpdate +=  new serverUpdateEventHandler(ServUpdate);
        }

        //Fonctions
        //After auto-update
        internal void ServUpdate()
        {
            ReadDatas();
            notifBar.MessageQueue.Enqueue((string)Application.Current.FindResource("refreshedDatas"));

            if (ColdStart == true)
            {
                ColdStart = false;
                //Go HomePage
                statsPageHolder.DataContext = new ServerStats_Overview(MainWindow.CareerSavegame);
                NavFrom = "ServerStats_Overview";
            }
        }

        //Read & Display datas
        internal void ReadDatas()
        {
            serverRefresh.IsEnabled = true;
            string xData = MainWindow.ServerData;

            //if offline
            if (xData == "error")
            {
                serverName.Text = (string)Application.Current.FindResource("serverOffline");
                serverName.Foreground = new SolidColorBrush(Colors.OrangeRed);
                serverPlayers.Text = "-/-";
                statsPageHolder.DataContext = new ServerStats_Overview("off");
                Players_list.ItemsSource = null;
                Menu_Is_Active(false);
                return;
            }

            //else
            dynamic serverData = JsonConvert.DeserializeObject(xData);
            
            //check if idle
            if (serverData.server.game == "")
            {
                serverName.Text = (string)Application.Current.FindResource("serverIdle");
                serverName.Foreground = new SolidColorBrush(Colors.Orange);
                serverPlayers.Text = "-/-";
                statsPageHolder.DataContext = new ServerStats_Overview("idle");
                Players_list.ItemsSource = null;
                Menu_Is_Active(false);
                return;
            }

            //online
            serverName.Text = serverData.server.name;
            serverName.Foreground = new SolidColorBrush(Colors.SkyBlue);
            serverPlayers.Text = serverData.slots.used + "/" + serverData.slots.capacity;
            
            Menu_Is_Active(true);
            Fill_Players_List(serverData.slots.players);
        }

        //Activate buttons
        internal void Menu_Is_Active(bool trueFalse)
        {
            Overview_Button.IsEnabled = trueFalse;
            Map_Button.IsEnabled = trueFalse;
            Data_Button.IsEnabled = trueFalse;
            Mods_Button.IsEnabled = trueFalse;
        }

        //Display Playerlist
        internal void Fill_Players_List(dynamic serverData)
        {
            player.Clear();
            Players_list.ItemsSource = null;

            foreach (var slot in serverData)
            {
                if ((bool)slot.isUsed)
                {
                    player.Add(new Player
                    {
                        isUsed = (bool)slot.isUsed,
                        isAdmin = (bool)slot.isAdmin,
                        uptime = (int)slot.uptime,
                        name = slot.name
                    });
                }
                else
                {
                    player.Add(new Player
                    {
                        isUsed = (bool)slot.isUsed,
                        name = (string)Application.Current.FindResource("emptySlot")
                    });
                }
            }
            Players_list.ItemsSource = player;
        }

        //Lists
        static List<Player> player = new List<Player>();
        internal class Player
        {
            public bool isUsed { get; set; }
            public bool isAdmin { get; set; }
            public int uptime { get; set; }
            public string name { get; set; }
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public string nameColor
            {
                get
                {
                    if (isAdmin == true) { return Colors.OrangeRed.ToString(); }
                    else if (isUsed == true) { return Colors.ForestGreen.ToString(); }
                    else { return Colors.Gray.ToString(); }
                }
            }
            public Visibility infoVisibility
            {
                get
                {
                    if (isUsed == true) { return Visibility.Visible; }
                    else { return Visibility.Collapsed; }
                }
            }
            public string uptimeDisplay
            {
                get
                {
                    return TimeSpan.FromMinutes(uptime).ToString(@"hh\hmm");
                }
            }
        }

        //Buttons
        //Refresh datas
        private void serverRefresh_Click(object sender, RoutedEventArgs e)
        {
            serverRefresh.IsEnabled = false;
            MainWindow.RefreshXml();
            ReadDatas();
            if (NavFrom == "ServerStats_Mods") { statsPageHolder.DataContext = new ServerStats_Mods(MainWindow.ServerData); }
        }

        //Access Overview page
        private void Overview_Button_Click(object sender, RoutedEventArgs e)
        {
            statsPageHolder.DataContext = new ServerStats_Overview(MainWindow.CareerSavegame);
            NavFrom = "ServerStats_Overview";
        }

        //Access Map page
        private void Map_Button_Click(object sender, RoutedEventArgs e)
        {
            statsPageHolder.DataContext = new ServerStats_Map(MainWindow.ServerData);
            NavFrom = "ServerStats_Map";
        }

        //Access Rawdatas page
        private void Data_Button_Click(object sender, RoutedEventArgs e)
        {
            statsPageHolder.DataContext = new ServerStats_Data();
            NavFrom = "ServerStats_Data";
        }

        //Access Mods page
        private void Mods_Button_Click(object sender, RoutedEventArgs e)
        {
            statsPageHolder.DataContext = new ServerStats_Mods(MainWindow.ServerData);
            NavFrom = "ServerStats_Mods";
        }

        //Access Settings page
        private void Settingd_Button_Click(object sender, RoutedEventArgs e)
        {
            statsPageHolder.DataContext = new Settings(false);
            NavFrom = "Settings";
        }
    }
}
