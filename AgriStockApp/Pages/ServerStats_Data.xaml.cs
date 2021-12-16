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
        public ServerStats_Data(string xmlData)
        {
            InitializeComponent();
            Debug.WriteLine(">>> Opening ServerStats_Data...");

            RefreshXml();
        }

        //Functions
        //Read & Display datas
        internal async void Read_Data(string xmlData)
        {
            if (xmlData == "error") { dataBox.Text = "Error!"; return; }
            dynamic serverData = await Task.Run(() => JsonConvert.DeserializeObject(xmlData));
            dataBox.Text = serverData.ToString();

            Debug.WriteLine(">>>> ServerStats_Data Displayed...");
        }

        //Refresh datas
        internal async void RefreshXml()
        {
            Debug.WriteLine(">>>> Refreshing ServerStats_Data...");

            string XMLData = await Task.Run(() => Fonctions.getServerData_XML_to_JSON(FS_Api.Career(MainWindow.Current_FS_Host, MainWindow.Current_FS_Key)));
            Read_Data(XMLData);
        }
    }
}
