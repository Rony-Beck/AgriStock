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
    public partial class ServerStats_Overview : UserControl
    {
        //Ctor
        public ServerStats_Overview(string xmlData)
        {
            InitializeComponent();
            Debug.WriteLine(">>> Opening ServerStats_Overview...");
            Debug.WriteLine(xmlData);

            if (xmlData == "off" || xmlData == "idle") { return; }

            SetData(JsonConvert.DeserializeObject(xmlData));
        }

        //Functions
        internal void SetData(dynamic json)
        {
            //dynamic serverData = JsonConvert.DeserializeObject(json);
            dynamic data = json.careerSavegame.settings;
            Created.Text = data.creationDate;
            Slots.Text = json.careerSavegame.slotSystem.slotUsage;
        }
    }
}
