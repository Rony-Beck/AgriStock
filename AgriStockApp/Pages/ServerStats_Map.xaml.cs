using AgriStockApp.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class ServerStats_Map : UserControl
    {
        //Props
        public int MapSize { get; set; }

        //Zoom Points
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;

        //Ctor
        public ServerStats_Map(string xmlData)
        {
            InitializeComponent();
            Debug.WriteLine(">>> Opening ServerStats_Map...");

            //Img Zoom Triggers
            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;

            slider.ValueChanged += OnSliderValueChanged;

            //Load map and datas
            dynamic serverData = JsonConvert.DeserializeObject(xmlData);
            LoadMap(serverData);
            LoadFields(serverData.fields);
            LoadVehicles(serverData.vehicles);
            LoadUsers(serverData.slots.players);
        }

        //Functions
        internal void LoadVehicles(dynamic serverData)
        {
            foreach (var item in serverData)
            {
                if (item.type == "tractor") { LoadTractors(item); }
                else if (item.type == "trailer" || item.type == "dolly" || item.type == "waterTrailer" || item.type == "forageWagon" || item.type == "strawBlower" || item.type == "mixerWagon") { LoadTrailers(item); }
                else if (item.type == "teleHandler") { LoadForkLifts(item); }
                else if (item.type == "pallet") { LoadPallets(item); }
                else { LoadOthers(item); }
            }
        }

        internal void LoadMap(dynamic serverData)
        {
            MapSize = (int)serverData.server.mapSize;
            
            grid.Width = MapSize;
            grid.Height = MapSize;
            mapOver.Width = MapSize;
            mapOver.Height = MapSize;


            mapName.Text = serverData.server.mapName;
            fieldsValue.Text = serverData.fields.Count.ToString();
            mapSizeValue.Text = MapSize.ToString();
            mapImage.Source =  new ImageSourceConverter().ConvertFromString(FS_Api.Map(MainWindow.Current_FS_Host, MainWindow.Current_FS_Key)) as ImageSource;
        }

        internal void LoadPallets (dynamic pallet)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Black;
            ellipse.Width = 5;
            ellipse.Height = 5;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = pallet.name;

            MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
            icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cube;
            icon.Width = 4;
            icon.Height = 4;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.Foreground = Brushes.Gray;
            icon.ToolTip = pallet.name;

            mapOver.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, ((double)pallet.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
            Canvas.SetTop(ellipse, ((double)pallet.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

            mapOver.Children.Add(icon);
            Canvas.SetLeft(icon, ((double)pallet.x / 2) + (MapSize / 2) - (icon.Width / 2));
            Canvas.SetTop(icon, ((double)pallet.z / 2) + (MapSize / 2) - (icon.Height / 2));
        }

        internal void LoadForkLifts(dynamic forklift)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Black;
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = forklift.name;

            MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
            icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Forklift;
            icon.Width = 9;
            icon.Height = 9;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.Foreground = Brushes.Yellow;
            icon.ToolTip = forklift.name;

            mapOver.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, ((double)forklift.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
            Canvas.SetTop(ellipse, ((double)forklift.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

            mapOver.Children.Add(icon);
            Canvas.SetLeft(icon, ((double)forklift.x / 2) + (MapSize / 2) - (icon.Width / 2));
            Canvas.SetTop(icon, ((double)forklift.z / 2) + (MapSize / 2) - (icon.Height / 2));
        }

        internal void LoadFields(dynamic serverData)
        {
            foreach (var field in serverData)
            {
                TextBlock fieldNumber = new TextBlock();
                fieldNumber.Text = field.id;
                fieldNumber.FontSize = 16;
                if ((bool)field.isOwned) { fieldNumber.Foreground = Brushes.LightGreen; }
                else { fieldNumber.Foreground = Brushes.LightGray; }

                mapOver.Children.Add(fieldNumber);
                Canvas.SetLeft(fieldNumber, ((double)field.x / 2) + (MapSize / 2) - (fieldNumber.FontSize / 2));
                Canvas.SetTop(fieldNumber, ((double)field.z / 2) + (MapSize / 2) - (fieldNumber.FontSize / 2));
            }
        }

        internal void LoadOthers(dynamic other)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Black;
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = other.name;

            MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
            icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.HelpCircleOutline;
            icon.Width = 9;
            icon.Height = 9;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.Foreground = Brushes.White;
            icon.ToolTip = other.name;

            mapOver.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, ((double)other.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
            Canvas.SetTop(ellipse, ((double)other.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

            mapOver.Children.Add(icon);
            Canvas.SetLeft(icon, ((double)other.x / 2) + (MapSize / 2) - (icon.Width / 2));
            Canvas.SetTop(icon, ((double)other.z / 2) + (MapSize / 2) - (icon.Height / 2));
        }

        internal void LoadTractors(dynamic tractor)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.DarkOrange;
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = tractor.name;

            MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
            icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.TractorVariant;
            icon.Width = 9;
            icon.Height = 9;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.Foreground = Brushes.White;
            icon.ToolTip = tractor.name;

            mapOver.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, ((double)tractor.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
            Canvas.SetTop(ellipse, ((double)tractor.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

            mapOver.Children.Add(icon);
            Canvas.SetLeft(icon, ((double)tractor.x / 2) + (MapSize / 2) - (icon.Width / 2));
            Canvas.SetTop(icon, ((double)tractor.z / 2) + (MapSize / 2) - (icon.Height / 2));
        }

        internal void LoadTrailers(dynamic trailer)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.DarkBlue;
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.StrokeThickness = 2;
            ellipse.ToolTip = trailer.name;

            MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
            icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.TruckTrailer;
            icon.Width = 9;
            icon.Height = 9;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.Foreground = Brushes.White;
            icon.ToolTip = trailer.name;

            mapOver.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, ((double)trailer.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
            Canvas.SetTop(ellipse, ((double)trailer.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

            mapOver.Children.Add(icon);
            Canvas.SetLeft(icon, ((double)trailer.x / 2) + (MapSize / 2) - (icon.Width / 2));
            Canvas.SetTop(icon, ((double)trailer.z / 2) + (MapSize / 2) - (icon.Height / 2));
        }

        internal void LoadUsers(dynamic serverData)
        {
            foreach (var user in serverData)
            {
                if ((bool)user.isUsed == true)
                {
                    if (user.x != null)
                    {
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = Brushes.DarkGreen;
                        ellipse.Width = 20;
                        ellipse.Height = 20;
                        ellipse.StrokeThickness = 2;
                        ellipse.ToolTip = user.name;

                        MaterialDesignThemes.Wpf.PackIcon icon = new MaterialDesignThemes.Wpf.PackIcon();
                        icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountCircle;
                        icon.Width = 18;
                        icon.Height = 18;
                        icon.VerticalAlignment = VerticalAlignment.Center;
                        icon.HorizontalAlignment = HorizontalAlignment.Center;
                        icon.Foreground = Brushes.White;
                        icon.ToolTip = user.name;

                        mapOver.Children.Add(ellipse);
                        Canvas.SetLeft(ellipse, ((double)user.x / 2) + (MapSize / 2) - (ellipse.Width / 2));
                        Canvas.SetTop(ellipse, ((double)user.z / 2) + (MapSize / 2) - (ellipse.Height / 2));

                        mapOver.Children.Add(icon);
                        Canvas.SetLeft(icon, ((double)user.x / 2) + (MapSize / 2) - (icon.Width / 2));
                        Canvas.SetTop(icon, ((double)user.z / 2) + (MapSize / 2) - (icon.Height / 2));
                    }
                }
            }
        }

        #region Img Scroll
        //Img Control
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(scrollViewer);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(scrollViewer);
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y <
                scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                scrollViewer.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(scrollViewer);
            }
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            if (e.Delta > 0)
            {
                slider.Value += 0.1;
            }
            if (e.Delta < 0)
            {
                slider.Value -= 0.1;
            }

            e.Handled = true;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        void OnSliderValueChanged(object sender,
             RoutedPropertyChangedEventArgs<double> e)
        {
            scaleTransform.ScaleX = e.NewValue;
            scaleTransform.ScaleY = e.NewValue;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2,
                                             scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
        }

        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2,
                                                         scrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              scrollViewer.TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        #endregion
    }
}
