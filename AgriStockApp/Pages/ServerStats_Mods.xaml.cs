using AgriStockApp.Scripts;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AgriStockApp.Pages
{
    public partial class ServerStats_Mods : UserControl
    {
        //Props
        public string ModPath { get; set; }
        public string WebPath { get; set; }
        public string GameFolder { get; set; }
        public string Game { get; set; }
        public ImageSource GameImg { get; set; }

        //Ctor
        public ServerStats_Mods(string xmlData)
        {
            InitializeComponent();
            WebPath = Config.FS_Host + "/mods/";
            LoadList(xmlData);
        }

        //Functions
        //Load & Display Modslist
        internal async void LoadList(string xmlData)
        {
            if (xmlData == "error") { return; }
            dynamic serverData = await Task.Run(() => JsonConvert.DeserializeObject(xmlData));
            
            //*************************************
            //Add Automatic game detector api based
            //*************************************
            Game = "fs22";

            if (Game == "fs22")
            {
                GameFolder = "FarmingSimulator2022";
                GameImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\Img\\fs22.png", UriKind.Absolute));
            }
            
            //Get Local Mods List
            ModPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\" + GameFolder + "\\mods";
            if (!Directory.Exists(ModPath))
            {
                Directory.CreateDirectory(ModPath);
            }
            string[] localMods = Directory.GetFiles(ModPath);

            //Foreach Serverside Mod
            foreach (var mod in serverData.mods)
            {
                //Construction de la Grid mère
                Grid MotherGrid = new Grid();
                MotherGrid.Height = 75;
                modsList_Zone.Children.Add(MotherGrid);
                
                //Ajout des colonnes
                ColumnDefinition MGrid_Gauche = new ColumnDefinition();
                ColumnDefinition MGrid_Centre = new ColumnDefinition();
                ColumnDefinition MGrid_Droite = new ColumnDefinition();
                MGrid_Gauche.Width = new GridLength(75);
                MGrid_Droite.Width = new GridLength(150);
                MotherGrid.ColumnDefinitions.Add(MGrid_Gauche);
                MotherGrid.ColumnDefinitions.Add(MGrid_Centre);
                MotherGrid.ColumnDefinitions.Add(MGrid_Droite);

                //Zone détails du mod
                StackPanel Details = new StackPanel();
                Details.VerticalAlignment = VerticalAlignment.Center;
                Details.Name = (string)mod.name;
                Grid.SetColumn(Details, 1);
                //ModName
                TextBlock ModName = new TextBlock();
                Details.Children.Add(ModName);
                ModName.Text = mod.name + ".zip";
                ModName.Foreground = Brushes.White;
                ModName.FontSize = 18;
                //ModAsh
                TextBlock ModAsh = new TextBlock();
                Details.Children.Add(ModAsh);
                ModAsh.Text = "Hash: " + (string)mod.hash;
                ModAsh.Foreground = Brushes.DarkOrange;
                ModAsh.FontSize = 10;

                //Zone Bouton
                Button Manage = new Button();
                Manage.VerticalAlignment = VerticalAlignment.Center;
                Manage.Name = "aa";
                Manage.Content = (string)Application.Current.FindResource("downLoad");
                Manage.ToolTip = (string)Application.Current.FindResource("downLoad") + " " + (string)mod.name;
                Manage.Background = new SolidColorBrush(Color.FromRgb(0, 151, 68));
                Manage.Margin = new Thickness(10);
                Manage.Click += AddMod;
                Grid.SetColumn(Manage, 2);

                //Zone Img
                Image ModImg = new Image();
                ModImg.Width = 50;
                ModImg.Height = 50;
                ModImg.HorizontalAlignment = HorizontalAlignment.Center;
                ModImg.VerticalAlignment = VerticalAlignment.Center;
                ModImg.Source = GameImg;
                Grid.SetColumn(ModImg, 0);

                //Search Local Copy
                foreach (var item in localMods)
                {
                    if (System.IO.Path.GetFileName(item) == (string)mod.name + ".zip")
                    {
                        //HashChecker
                        string modsHash = CalculateMD5(@System.IO.Path.Combine(ModPath, System.IO.Path.GetFileName(item)));

                        Debug.WriteLine(">> Hash: " + modsHash);

                        if (modsHash != (string)mod.hash)
                        {
                            Manage.Content = (string)Application.Current.FindResource("update");
                            Manage.ToolTip = (string)Application.Current.FindResource("update") + " " + (string)mod.name;
                            Manage.Background = Brushes.SkyBlue;
                            break;
                        }

                        Manage.Content = (string)Application.Current.FindResource("remove");
                        Manage.ToolTip = (string)Application.Current.FindResource("remove") + " " + (string)mod.name;
                        Manage.Background = Brushes.OrangeRed;
                        Manage.Click -= AddMod;
                        Manage.Click += RemoveMod;
                        break;
                    }
                }

                //Display
                MotherGrid.Children.Add(Details);
                MotherGrid.Children.Add(Manage);
                MotherGrid.Children.Add(ModImg);
            }
        }
        
        //Telechargement
        private static async Task DownloadFileAsync(string url, string filePath, Button source)
        {
            WebClient client = new WebClient();
            client.DownloadProgressChanged += (s, e) => { source.Content = e.ProgressPercentage + "%"; };
            await client.DownloadFileTaskAsync(new Uri(url), filePath);
        }

        //Get md5
        static string CalculateMD5(string filename)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {

                        var hash = md5.ComputeHash(stream);
                        stream.Dispose();

                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch { return "00X"; }
        }

        //Boutons
        //Ajouter un mod
        private async void AddMod(object sender, RoutedEventArgs e)
        {
            Button source = e.Source as Button;
            Grid pGrid = source.Parent as Grid;
            StackPanel pDetails = pGrid.Children[0] as StackPanel;
            TextBlock pModeName = pDetails.Children[0] as TextBlock;

            string WebModsPath = WebPath + pModeName.Text;

            source.IsEnabled = false;
            source.Content = (string)Application.Current.FindResource("wait");

            try
            {
                await DownloadFileAsync(WebModsPath, System.IO.Path.Combine(ModPath, pModeName.Text), source);

                source.Click -= AddMod;
                source.Click += RemoveMod;

                source.Content = (string)Application.Current.FindResource("remove");
                source.ToolTip = (string)Application.Current.FindResource("remove") + " " + pModeName.Text;
                source.Background = Brushes.OrangeRed;
            }
            catch (Exception ex)
            {
                source.Content = (string)Application.Current.FindResource("downLoad");
                Debug.WriteLine(ex);
            }
            source.IsEnabled = true;
        }

        //Retirer un mod
        private void RemoveMod(object sender, RoutedEventArgs e)
        {
            Button source = e.Source as Button;
            Grid pGrid = source.Parent as Grid;
            StackPanel pDetails = pGrid.Children[0] as StackPanel;
            TextBlock pModeName = pDetails.Children[0] as TextBlock;

            source.IsEnabled = false;

            try {
                File.Delete(System.IO.Path.Combine(ModPath, pModeName.Text));
                source.Click += AddMod;
                source.Click -= RemoveMod;

                source.Content = (string)Application.Current.FindResource("downLoad");
                source.ToolTip = (string)Application.Current.FindResource("downLoad") + " " + pModeName.Text;
                source.Background = new SolidColorBrush(Color.FromRgb(0, 151, 68));
            }
            catch
            {
                Debug.WriteLine(pModeName.Text + " not deleted");
            }

            source.IsEnabled = true;
        }
    }
}
