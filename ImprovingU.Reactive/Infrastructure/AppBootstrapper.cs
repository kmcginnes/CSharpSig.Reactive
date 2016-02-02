using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using PoC.NuGetWpf;
using ReactiveUI;
using Splat;

namespace ImprovingU.Reactive.Infrastructure
{
    public class AppBootstrapper
    {
        public AppBootstrapper(IMutableDependencyResolver dependencyResolver)
        {
            InitializeLogging();
            RegisterExceptionHandlers();
            RegisterTypesInContainer(dependencyResolver);
        }

        private void InitializeLogging()
        {
            // Set the main thread's name to make it clear in the logs.
            Thread.CurrentThread.Name = "Main";

            // Sets my logger to the console, which goes to the debug output.
            Log.InitializeWith<ConsoleLog>();

            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            LogExtensions.Log(this).Info(@" _______       _______             ");
            LogExtensions.Log(this).Info(@"(_______)     (_______)        _   ");
            LogExtensions.Log(this).Info(@" _     _ _   _ _   ___ _____ _| |_ ");
            LogExtensions.Log(this).Info(@"| |   | | | | | | (_  | ___ (_   _)");
            LogExtensions.Log(this).Info(@"| |   | | |_| | |___) | ____| | |_ ");
            LogExtensions.Log(this).Info(@"|_|   |_|____/ \_____/|_____)  \__)");
            LogExtensions.Log(this).Info(@"");

            // Show some basic information about the assembly.
            var assemblyLocation = GetAssemblyDirectory();
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var principal = WindowsIdentity.GetCurrent().IfNotNull(x => x.Name, "[Unknown]");
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            var machineName = String.Format("{0} ({1})", Environment.MachineName, ipAddress);
            var windowsVersion = String.Format("{0} {1}", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "64bit" : "32bit");

            LogExtensions.Log(this).Info("Assembly location: {0}", assemblyLocation);
            LogExtensions.Log(this).Info(" Assembly version: {0}", assemblyVersion);
            LogExtensions.Log(this).Info("     File version: {0}", fileVersion);
            LogExtensions.Log(this).Info("  Product version: {0}", productVersion);
            LogExtensions.Log(this).Info("       Running as: {0}", principal);
            LogExtensions.Log(this).Info("     Network Host: {0}", machineName);
            LogExtensions.Log(this).Info("  Windows Version: {0}", windowsVersion);
        }

        private static string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private void RegisterExceptionHandlers()
        {
            // If we are running this app in Visual Studio then do not handle
            // any of the unhandled exceptions. Let Visual Studio catch them.
            if (AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe"))
            {
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var loggerTarget = sender ?? this;
                var exception = args.ExceptionObject as Exception;
                LogExtensions.Log(loggerTarget).Fatal(exception, "Unhandled exception in the app domain.");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                var loggerTarget = sender ?? this;
                LogExtensions.Log(loggerTarget).Fatal(args.Exception, "Unhandled exception in the task scheduler.");
            };

            Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                var loggerTarget = sender ?? this;
                LogExtensions.Log(loggerTarget).Fatal(args.Exception, "Unhandled exception in the application dispatcher.");
            };
        }

        private void RegisterTypesInContainer(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new CreateUserView(), typeof(IViewFor<CreateUserViewModel>));
        }
    }
}