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
            WebPath = Properties.Settings.Default.ServerHost + "/mods/";
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

        //Reset Counter
        internal void ResetCounters()
        {
            ModsNew = 0;
            ModsSynced = 0;
            ModsTotal = 0;
            ModsToUpdate = 0;
        }

        //Load & Display Modslist
        internal async void LoadList(string xmlData)
        {
            //Reset list
            Mods.Clear();
            modsList_Zone.Children.Clear();
            //Reset counters
            ResetCounters();
            //Security
            if (xmlData == "error") { return; }
            //Read datas
            dynamic serverData = await Task.Run(() => JsonConvert.DeserializeObject(xmlData));

            //Get server game
            Game = serverData.server.game;

            //Set game folder
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
            else 
            {
                Unsupported_Game();
                return; 
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
                MotherGrid.Margin = new Thickness(0, 0, 0, 10);
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
                ModName.Text = mod.description;
                ModName.Foreground = Brushes.White;
                ModName.FontSize = 18;
                ModName.TextWrapping = TextWrapping.WrapWithOverflow;
                //ModAuthor
                TextBlock ModAuthor = new TextBlock();
                Details.Children.Add(ModAuthor);
                ModAuthor.Text = (string)Application.Current.FindResource("author") + ": " + (string)mod.author;
                ModAuthor.Foreground = Brushes.LightGray;
                ModAuthor.FontSize = 10;
                //ModVersion
                TextBlock ModVersion = new TextBlock();
                Details.Children.Add(ModVersion);
                ModVersion.Text = "Version: " + (string)mod.version;
                ModVersion.Foreground = Brushes.ForestGreen;
                ModVersion.FontSize = 10;

                //Zone IconState
                MaterialDesignThemes.Wpf.PackIcon IconState = new MaterialDesignThemes.Wpf.PackIcon();
                IconState.VerticalAlignment = VerticalAlignment.Center;
                IconState.Kind = MaterialDesignThemes.Wpf.PackIconKind.NewBox;
                IconState.HorizontalAlignment = HorizontalAlignment.Right;
                IconState.Margin = new Thickness(0, 0, 10, 0);
                IconState.Foreground = Brushes.ForestGreen;
                IconState.Width = 30;
                IconState.Height = 30;
                Grid.SetColumn(IconState, 2);

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
                            //Debug.WriteLine(">> Hash: " + modsHash);

                            //Si Hash ne corresponds pas
                            if (modsHash != (string)mod.hash)
                            {
                                ThisMod.IsUpdate = true;
                                ModsToUpdate++;
                                ModsNew--;
                                IconState.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                                IconState.Foreground = Brushes.Orange;
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
                        IconState.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckAll;
                        break;
                    }
                }

                //Add Mod to lists
                Mods.Add(ThisMod);

                //Display
                MotherGrid.Children.Add(Details);
                MotherGrid.Children.Add(IconState);
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

        //Jeu non supporté
        internal void Unsupported_Game()
        {
            ProgressCircle.Visibility = Visibility.Collapsed;
            SyncButtonIcon.Visibility = Visibility.Visible;

            SyncButton.IsEnabled = false;
            SyncButton.Background = Brushes.OrangeRed;
            SyncButtonText.Text = (string)Application.Current.FindResource("unsupportedGame");
            SyncButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.SyncAlert;
        }

        //Boutons
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

            //Recharger le tableau
            LoadList(MainWindow.ServerData);

            //Modifier bouton
            ProgressCircle.Visibility = Visibility.Collapsed;
            SyncButtonIcon.Visibility = Visibility.Visible;
            SyncButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircleOutline;
            SyncButtonText.Text = (string)Application.Current.FindResource("done");
        }
    }
}
