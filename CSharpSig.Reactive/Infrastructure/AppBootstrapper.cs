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
using NLog;
using ReactiveUI;
using Splat;
using ILogger = Splat.ILogger;
using LogLevel = Splat.LogLevel;

namespace ImprovingU.Reactive.Infrastructure
{
    public class AppBootstrapper : IEnableLogger
    {
        public AppBootstrapper(IMutableDependencyResolver dependencyResolver)
        {
            InitializeLogging();
            RegisterExceptionHandlers();
            RegisterTypesInContainer(dependencyResolver);
        }

        void InitializeLogging()
        {
            // Set the main thread's name to make it clear in the logs.
            Thread.CurrentThread.Name = "UI";

            // Sets my logger to the console, which goes to the debug output.
            Locator.CurrentMutable.Register(() => new ConsoleLogger {Level = LogLevel.Debug}, typeof(ILogger));
            
            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            this.Log().Info(@" _______       _______             ");
            this.Log().Info(@"(_______)     (_______)        _   ");
            this.Log().Info(@" _     _ _   _ _   ___ _____ _| |_ ");
            this.Log().Info(@"| |   | | | | | | (_  | ___ (_   _)");
            this.Log().Info(@"| |   | | |_| | |___) | ____| | |_ ");
            this.Log().Info(@"|_|   |_|____/ \_____/|_____)  \__)");
            this.Log().Info(@"");

            // Show some basic information about the assembly.
            var assemblyLocation = GetAssemblyDirectory();
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var principal = WindowsIdentity.GetCurrent().IfNotNull(x => x.Name, "[Unknown]");
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            var machineName = $"{Environment.MachineName} ({ipAddress})";
            var windowsVersion = $"{Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64bit" : "32bit")}";

            this.Log().Info($"Assembly location: {assemblyLocation}");
            this.Log().Info($" Assembly version: {assemblyVersion}");
            this.Log().Info($"     File version: {fileVersion}");
            this.Log().Info($"  Product version: {productVersion}");
            this.Log().Info($"       Running as: {principal}");
            this.Log().Info($"     Network Host: {machineName}");
            this.Log().Info($"  Windows Version: {windowsVersion}");
        }

        static string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        void RegisterExceptionHandlers()
        {
            // If we are running this app in Visual Studio then do not handle
            // any of the unhandled exceptions. Let Visual Studio catch them.
            if (AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe"))
            {
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                
                LogHost.Default.FatalException("Unhandled exception in the app domain.", exception);
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                LogHost.Default.FatalException("Unhandled exception in the task scheduler.", args.Exception);
            };

            Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                LogHost.Default.FatalException("Unhandled exception in the application dispatcher.", args.Exception);
            };
        }

        void RegisterTypesInContainer(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new CreateUserView(), typeof(IViewFor<CreateUserViewModel>));
        }
    }
}