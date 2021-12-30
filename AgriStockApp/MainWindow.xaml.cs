using AgriStockApp.Scripts;
using System;
using System.Diagnostics;
using System.Windows;

namespace AgriStockApp
{
    public partial class MainWindow : Window
    {
        //Shared Props
        public static dynamic ServerData { get; set; }
        public static dynamic CareerSavegame { get; set; }
        public static dynamic EconomySavegame { get; set; }
        public static dynamic VehiclesSavegame { get; set; }
        public static string Current_FS_Host { get; set; }
        public static string Current_FS_Key { get; set; }

        //Ctor
        public MainWindow()
        {
            InitializeComponent();
            
            //Setting App Language
            this.SetLanguageDictionary();
            
            Debug.WriteLine("Hello!");
            
            //Will be removed later
            Current_FS_Host = Config.FS_Host;
            Current_FS_Key = Config.FS_Key;
            
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
    }
}
