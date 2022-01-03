using AgriStockApp.Scripts;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ServerStats_Mods : UserControl, INotifyPropertyChanged
    {
        //Props
        public string ModPath { get; set; }
        public string WebPath { get; set; }
        public string GameFolder { get; set; }
        public string Game { get; set; }
        public ImageSource GameImg { get; set; }

        //PropsFull
        private bool isDownloadEnabled;
        public bool IsDownLoadEnabled
        {
            get { return isDownloadEnabled; }
            set { isDownloadEnabled = value; Update("IsDownLoadEnabled"); }
        }

        private int modsNew;
        public int ModsNew
        {
            get { return modsNew; }
            set { modsNew = value; Update("ModsNew"); }
        }

        private int modsSynced;
        public int ModsSynced
        {
            get { return modsSynced; }
            set { modsSynced = value; Update("ModsSynced"); }
        }

        private int modsTotal;
        public int ModsTotal
        {
            get { return modsTotal; }
            set { modsTotal = value; Update("ModsTotal"); }
        }

        private int modsToUpdate;
        public int ModsToUpdate
        {
            get { return modsToUpdate; }
            set { modsToUpdate = value; Update("ModsToUpdate"); }
        }

        private int progressTotal;
        public int ProgressTotal
        {
            get { return progressTotal; }
            set { progressTotal = value; Update("ProgressTotal"); }
        }

        private int progressCurrent;
        public int ProgressCurrent
        {
            get { return progressCurrent; }
            set { progressCurrent = value; Update("ProgressCurrent"); }
        }

        //Ctor
        public ServerStats_Mods(string xmlData)
        {
            InitializeComponent();
            this.DataContext = this;
            WebPath = Config.FS_Host + "/mods/";
            LoadList(xmlData);
        }

        //Lists used
        static List<CustomLists.Mod> Mods = new List<CustomLists.Mod>();

        //Functions
        //Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Update(string Name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        //Load & Display Modslist
        internal async void LoadList(string xmlData)
        {
            //Reset list
            Mods.Clear();
            //Security
            if (xmlData == "error") { return; }
            //Read datas
            dynamic serverData = await Task.Run(() => JsonConvert.DeserializeObject(xmlData));

            //Get server game
            Game = serverData.server.game;
            Debug.WriteLine(Game);

            if (Game == "Farming Simulator 22")
            {
                GameFolder = "FarmingSimulator2022";
                GameImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\Img\\fs22.png", UriKind.Absolute));
            }
            else if (Game == "Farming Simulator 19")
            {
                GameFolder = "FarmingSimulator2019";
                GameImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\Img\\fs19.png", UriKind.Absolute));
            }
            else { return; }

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
                //Création du mod
                var ThisMod = new CustomLists.Mod();
                //Ajout des premières données
                ThisMod.Name = mod.description;
                ThisMod.Author = mod.author;
                ThisMod.Version = mod.version;
                ThisMod.Hash = mod.hash;
                ThisMod.FileNameExt = mod.name + ".zip";
                ThisMod.Path = WebPath + ThisMod.FileNameExt;
                ThisMod.IsNew = true;
                
                //Ajout au nombre de mods total
                ModsTotal++;
                ModsNew++;

                //Construction de la Grid mère
                Grid MotherGrid = new Grid();
                MotherGrid.Height = 60;
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
                ModName.TextWrapping = TextWrapping.WrapWithOverflow;
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
                Manage.Visibility = Visibility.Collapsed;
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
                    if (Path.GetFileName(item) == (string)mod.name + ".zip")
                    {
                        ThisMod.IsNew = false;
                        try
                        {
                            //HashChecker
                            string modsHash = await Task.Run(() => Fonctions.GetModHash(@Path.Combine(ModPath, Path.GetFileName(item))));
                            Debug.WriteLine(">> Hash: " + modsHash);

                            //Si Hash ne corresponds pas
                            if (modsHash != (string)mod.hash)
                            {
                                ThisMod.IsUpdate = true;
                                ModsToUpdate++;
                                ModsNew--;
                                Manage.Content = (string)Application.Current.FindResource("update");
                                Manage.ToolTip = (string)Application.Current.FindResource("update") + " " + (string)mod.name;
                                Manage.Background = Brushes.Violet;
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }

                        //Si hash corresponds
                        ThisMod.IsInstalled = true;
                        ModsSynced++;
                        ModsNew--;
                        Manage.Content = (string)Application.Current.FindResource("remove");
                        Manage.ToolTip = (string)Application.Current.FindResource("remove") + " " + (string)mod.name;
                        Manage.Background = Brushes.DarkOrange;
                        Manage.Click -= AddMod;
                        Manage.Click += RemoveMod;
                        break;
                    }
                }

                //Add Mod to lists
                Mods.Add(ThisMod);

                //Display
                MotherGrid.Children.Add(Details);
                MotherGrid.Children.Add(Manage);
                MotherGrid.Children.Add(ModImg);
            }

            //Active Sync Button
            ProgressCircle.Visibility = Visibility.Collapsed;
            SyncButtonIcon.Visibility = Visibility.Visible;
            if (ModsSynced != ModsTotal)
            {
                SyncButton.IsEnabled = true;
                SyncButtonText.Text = (string)Application.Current.FindResource("downLoad");
                SyncButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Download;
            }
            else
            {
                SyncButtonText.Text = (string)Application.Current.FindResource("upToDate");
                SyncButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircleOutline;
            }
        }
        
        //Telechargement
        private async Task DownloadFileAsync(string url, string filePath, bool isNew, bool isUpdate)
        {
            WebClient client = new WebClient();
            
            //Progress Update
            client.DownloadProgressChanged += (s, e) => { ProgressCurrent = e.ProgressPercentage; };
            
            //Démarrage du DL
            await client.DownloadFileTaskAsync(new Uri(url), filePath);

            //Quand DL terminé
            ModsSynced++;
            if (isNew) ModsNew--;
            if (isUpdate) ModsToUpdate--;
        }

        //Boutons
        //Ajouter un mod
        private void AddMod(object sender, RoutedEventArgs e)
        {
            //Button source = e.Source as Button;
            //Grid pGrid = source.Parent as Grid;
            //StackPanel pDetails = pGrid.Children[0] as StackPanel;
            //TextBlock pModeName = pDetails.Children[0] as TextBlock;

            //string WebModsPath = WebPath + pModeName.Text;

            //source.IsEnabled = false;
            //source.Content = (string)Application.Current.FindResource("wait");

            //try
            //{
            //    await DownloadFileAsync(WebModsPath, Path.Combine(ModPath, pModeName.Text), source);

            //    source.Click -= AddMod;
            //    source.Click += RemoveMod;

            //    source.Content = (string)Application.Current.FindResource("remove");
            //    source.ToolTip = (string)Application.Current.FindResource("remove") + " " + pModeName.Text;
            //    source.Background = Brushes.DarkOrange;
            //}
            //catch (Exception ex)
            //{
            //    source.Content = (string)Application.Current.FindResource("downLoad");
            //    Debug.WriteLine(ex);
            //}
            //source.IsEnabled = true;
        }

        //Retirer un mod
        private void RemoveMod(object sender, RoutedEventArgs e)
        {
            Button source = e.Source as Button;
            Grid pGrid = source.Parent as Grid;
            StackPanel pDetails = pGrid.Children[0] as StackPanel;
            TextBlock pModeName = pDetails.Children[0] as TextBlock;

            source.IsEnabled = false;

            try
            {
                File.Delete(Path.Combine(ModPath, pModeName.Text));
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

        private async void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            SyncButton.IsEnabled = false;
            SyncButtonText.Text = (string)Application.Current.FindResource("updating");
            ProgressCircle.Visibility = Visibility.Visible;
            SyncButtonIcon.Visibility = Visibility.Collapsed;
            foreach (var mod in Mods)
            {
                //Si déjà installé, zap au prochain
                if (mod.IsInstalled || mod.Path == null) { continue; }

                //Télécharge le mod
                await DownloadFileAsync(mod.Path, Path.Combine(ModPath, mod.FileNameExt), mod.IsNew, mod.IsUpdate);
                SyncButton.IsEnabled = false;
            }

            ProgressCircle.Visibility = Visibility.Collapsed;
            SyncButtonIcon.Visibility = Visibility.Visible;
            SyncButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircleOutline;
            SyncButtonText.Text = (string)Application.Current.FindResource("done");
        }
    }
}
