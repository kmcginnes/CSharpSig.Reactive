using System;
using System.Windows;

namespace ImprovingU.Reactive
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Events().Loaded.Subscribe(_ => CreateUserContainer.ViewModel = new CreateUserViewModel());
        }
    }
}
