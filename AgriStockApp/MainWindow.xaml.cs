using AgriStockApp.Scripts;
using RestSharp;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;
using System.Xml.Linq;

namespace AgriStockApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Props
        public string ApiResponse { get; set; }
        public static string Current_FS_Host { get; set; }
        public static string Current_FS_Key { get; set; }

        //Ctor
        public MainWindow()
        {
            InitializeComponent();
            this.SetLanguageDictionary();
            Debug.WriteLine("Hello!");
            pageHolder.DataContext = this.DataContext;
            Current_FS_Host = Config.FS_Host;
            Current_FS_Key = Config.FS_Key;
            Debug.WriteLine("Ready to serve.");
            pageHolder.DataContext = new Pages.ServerStats(); ;
        }

        //Function
        private void ShowMap()
        {
            ImageSource MapSource = new ImageSourceConverter().ConvertFromString(FS_Api.Map(Current_FS_Host, Current_FS_Key)) as ImageSource;
            //ServerMap.Source = MapSource;
            //textBlock.Text = MapSource.ToString();
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

        //Button
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            ApiResponse = await Task.Run(() => Fonctions.getServerData(FS_Api.Stats(Current_FS_Host, Current_FS_Key)));

            //textBlock.Text = ApiResponse;
        }

        private async void button_Click2(object sender, RoutedEventArgs e)
        {
            ApiResponse = await Task.Run(() => Fonctions.getServerData(FS_Api.Economy(Current_FS_Host, Current_FS_Key)));

            //textBlock.Text = ApiResponse;
        }

        private async void button_Click3(object sender, RoutedEventArgs e)
        {
            ApiResponse = await Task.Run(() => Fonctions.getServerData(FS_Api.Career(Current_FS_Host, Current_FS_Key)));

            //textBlock.Text = ApiResponse;
        }

        private async void button_Click4(object sender, RoutedEventArgs e)
        {
            ApiResponse = await Task.Run(() => Fonctions.getServerData(FS_Api.Vehicles(Current_FS_Host, Current_FS_Key)));

            //textBlock.Text = ApiResponse;
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            //if (menuMain.Visibility == Visibility.Collapsed)
            //{
            //    menuMain.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    menuMain.Visibility = Visibility.Collapsed;
            //}
        }

        private void servStatsButton_Click(object sender, RoutedEventArgs e)
        {
            //menuMain.Visibility = Visibility.Collapsed;
            //pageHolder.DataContext = new Pages.ServerStats();
        }
    }
}
