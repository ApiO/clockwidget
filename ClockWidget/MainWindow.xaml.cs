using System;
using System.Configuration;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

using System.Windows.Threading;

namespace ClockWidget
{
    public partial class MainWindow : Window
    {

        public const string AppName = "ClockWidget";
        public string config_file;
        public bool locked = false;
        
        private ClockWidgetProperties properties_form;
        private DispatcherTimer dispatcherTimer;


        public double text_size
        {
            get { return lbl_time.FontSize; }
            set { lbl_time.FontSize = value; }
        }

        public Brush text_foreground
        {
            get { return lbl_time.Foreground; }
            set { lbl_time.Foreground = value; }
        }

        public Brush text_background
        {
            get { return lbl_time.Background; }
            set { lbl_time.Background = value; }
        }

        public double text_opacity
        {
            get { return lbl_time.Opacity; }
            set { lbl_time.Opacity = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            lbl_time.Content = DateTime.Now.ToString("HH:mm");
            lbl_time.MouseDoubleClick += close;

            lbl_time.MouseRightButtonDown += lbl_time_MouseRightButtonDown;

            config_file = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".exe", ".bin");
            properties_form = new ClockWidgetProperties(this);
        }

        private void lbl_time_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(properties_form.Visibility != System.Windows.Visibility.Visible)
             properties_form.Show();
        }

        private void close(object sender, MouseEventArgs e)
        {
            Properties.Settings.Default.MainWindowPlacement = GetPlacement(this);
            Properties.Settings.Default.Save();
            properties_form.shutdown();
            this.Close();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lbl_time.Content = DateTime.Now.ToString("HH:mm");
            CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (locked) return;
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        public static void SetPlacement(Window window, string placementXml)
        {
            WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
        }

        public static string GetPlacement(Window window)
        {
            return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetPlacement(this, Properties.Settings.Default.MainWindowPlacement);
        }

    }
}
