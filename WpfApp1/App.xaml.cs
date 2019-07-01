using DesktopNotifications;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace MTGApro
{
    public partial class App : Application, ISingleInstanceApp
    {
        private const string appGuid = "c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9";

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            //if(args.Length>0) File.WriteAllText(@"err_log.txt", args[0]);
            if (SingleInstance<App>.InitializeAsFirstInstance(appGuid) || (args.Length>0 && args[0]==@"restarting"))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                SingleInstance<App>.Cleanup();
            }

          //  DesktopNotificationManagerCompat.RegisterAumidAndComServer<MyNotificationActivator>("MTGApro");
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs e)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            // Get the Name of the AssemblyFile
            var assemblyName = new AssemblyName(e.Name);
            var dllName = assemblyName.Name + ".dll";

            // Load from Embedded Resources - This function is not called if the Assembly is already
            // in the same folder as the app.
            var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(dllName));
            if (resources.Any())
            {

                // 99% of cases will only have one matching item, but if you don't,
                // you will have to change the logic to handle those cases.
                var resourceName = resources.First();
                using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    var block = new byte[stream.Length];

                    // Safely try to load the assembly.
                    try
                    {
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                    catch (BadImageFormatException)
                    {
                        return null;
                    }
                }
            }

            // in the case the resource doesn't exist, return null.
            return null;
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            Window mainWindow = MainWindow; 
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.Show();
                mainWindow.ShowInTaskbar = true;
                mainWindow.WindowState = WindowState.Normal;
            }

            mainWindow.Activate();

            /*MessageBox.Show("App is already running");
            Current.MainWindow.WindowState = WindowState.Normal;*/
            return true;
        }

    }

    /*[ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9"), ComVisible(true)]
    public class MyNotificationActivator : NotificationActivator
    {
        public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (Application.Current.Windows.Count == 0)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    Application.Current.Windows[0].Show();
                }

                Application.Current.Windows[0].ShowInTaskbar=true;

                // Activate the window, bringing it to focus
                Application.Current.Windows[0].Activate();

                // And make sure to maximize the window too, in case it was currently minimized
                Application.Current.Windows[0].WindowState = WindowState.Normal;
            });
        }
    }*/
}
