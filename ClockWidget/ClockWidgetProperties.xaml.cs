using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace ClockWidget
{
    /// <summary>
    /// Logique d'interaction pour ClockWidgetProperties.xaml
    /// </summary>
    public partial class ClockWidgetProperties : Window
    {
        private bool force_close = false;
        private MainWindow _main_form;

        private Configuration config_backup;
        private Configuration current_config;

        public ClockWidgetProperties(MainWindow main_form)
        {
            InitializeComponent();
            _main_form = main_form;


            RegistryKey key = Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Windows\CurrentVersion\Run", true);
            cb_Startup.IsChecked = key.GetValue(MainWindow.AppName) != null;
            key.Close();
            key.Dispose();
            
            Configuration config = File.Exists(_main_form.config_file) 
             ? Serializer.DeSerializeObject<Configuration>(_main_form.config_file)
             : InitDefaultConfig();

            Load(config);

            cb_lock.Click += lock_widget;
            cb_Startup.Click += startup;
            cb_top.Click += top_statu;
            size_slider.ValueChanged += size_slider_ValueChanged;
            opacity_slider.ValueChanged += opacity_slider_ValueChanged;
            
            bt_load.Click += bt_load_Click;
            bt_Save.Click += bt_Save_Click;

            FontColor.TextChanged += FontColor_TextChanged;
            tb_Background.TextChanged += Background_TextChanged;
        }

        private void top_statu(object sender, RoutedEventArgs e)
        {
            _main_form.Topmost = !_main_form.Topmost;
            current_config.Topmost = !current_config.Topmost;
        }

        private void Background_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string color;
            switch (tb_Background.Text.Length)
            {
                case 0: color = "#00FFFFFF"; break;
                case 6: color = "#FF" + tb_Background.Text; break;
                default: return;
            }
            _main_form.text_background = (Brush)(new BrushConverter()).ConvertFrom(color);
            current_config.BackgroundColor = color;
        }

        private void FontColor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string color;
            if (FontColor.Text.Length != 6) return;
            
            color = "#FF" + FontColor.Text; 
            _main_form.text_foreground = (Brush)(new BrushConverter()).ConvertFrom(color);
            current_config.FontColor = color;
        }

        private Configuration InitDefaultConfig()
        {
            Configuration config = new Configuration();

            Serializer.SerializeObject<Configuration>(config, _main_form.config_file);

            return config;
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {
            Serializer.SerializeObject<Configuration>(current_config, _main_form.config_file);
            config_backup = current_config.Clone();
        }

        private void bt_load_Click(object sender, RoutedEventArgs e)
        {
            Load(config_backup);
        }

        public void Load(Configuration config)
        {
            cb_lock.IsChecked = _main_form.locked = config.Lock;
            cb_top.IsChecked = _main_form.Topmost = config.Topmost;

            _main_form.lbl_time.FontSize = size_slider.Value = config.FontSize;
            _main_form.lbl_time.Opacity = opacity_slider.Value = config.Opacity;

            tb_Background.Text = config.BackgroundColor.Length > 0 
                ? config.BackgroundColor.Replace("#00FFFFFF", string.Empty).Replace("#FF", string.Empty)
                : string.Empty;
            _main_form.text_background = (Brush)(new BrushConverter()).ConvertFrom(config.BackgroundColor);

            FontColor.Text = config.FontColor.Replace("#FF", string.Empty);
            _main_form.text_foreground = (Brush)(new BrushConverter()).ConvertFrom(config.FontColor);

            config_backup  = config.Clone();
            current_config = config.Clone();
        }

        private void lock_widget(object sender, RoutedEventArgs e)
        {
            _main_form.locked = !_main_form.locked;
            current_config.Lock = !current_config.Lock;
        }

        private void startup(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Windows\CurrentVersion\Run", true);

            bool autostart = cb_Startup.IsChecked == true;
            if (autostart)
            {
                //Surround path with " " to make sure that there are no problems
                //if path contains spaces.
                key.SetValue(MainWindow.AppName, "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
            else
            { 
                key.DeleteValue(MainWindow.AppName); 
            }

            MessageBox.Show("ClockWidget is " + (autostart ? "on" : "removed from").ToString() + " windows' startup", "ClockWidget config", MessageBoxButton.OK, MessageBoxImage.Information);

            key.Close();
            key.Dispose();
        }

        private void size_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _main_form.text_size = size_slider.Value;
            current_config.FontSize = size_slider.Value;
        }

        private void opacity_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _main_form.text_opacity = opacity_slider.Value;
            current_config.Opacity = opacity_slider.Value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (force_close) return;
            this.Hide();
            _main_form.IsEnabled = true;
            e.Cancel = true;
        }

        public void shutdown()
        {
            force_close = true;
            this.Close();
        }

    }
}
