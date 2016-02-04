using System.Windows;
using ImprovingU.Reactive.Infrastructure;
using Splat;

namespace ImprovingU.Reactive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppBootstrapper Bootstrapper { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Bootstrapper = new AppBootstrapper(Locator.CurrentMutable);
            base.OnStartup(e);
        }
    }
}
