using AgriStockApp.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public partial class ServerStats_Data : UserControl
    {
        //Fields
        Window window;

        //Ctor
        public ServerStats_Data()
        {
            //Wake up
            InitializeComponent();
            Debug.WriteLine(">>> Opening ServerStats_Data...");

            //Update Trigger
            MainWindow._servUpdate += new serverUpdateEventHandler(Update_Data);

            //Display Data
            Read_Data();
        }

        //Functions
        //Update datas
        internal void Update_Data()
        {
            //If not active page => Skip
            window = Window.GetWindow(this);
            if (window == null) return;

            //Else
            Read_Data();
        }

        //Display datas
        internal async void Read_Data()
        {
            string xmlData = MainWindow.ServerData;
            if (xmlData == "error") { dataBox.Text = "Error!"; return; }
            dynamic serverData = await Task.Run(() => JsonConvert.DeserializeObject(xmlData));
            dataBox.Text = await Task.Run(() => serverData.ToString());

            Debug.WriteLine(">>>> ServerStats_Data Displayed...");
        }
    }
}
