using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ClockWidget
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp 
    {
        private const string Unique = "ClockWidget_mutex";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
            else 
            {
                MessageBox.Show("ClockWidget already launched!", "ClockWidget", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // Handle command line arguments of second instance
            return true;
        }
        #endregion
    }
}
