using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace ImprovingU.Reactive
{
    public partial class CreateUserView : UserControl, IViewFor<CreateUserViewModel>
    {
        public CreateUserView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(this.Bind(ViewModel, vm => vm.FirstName, v => v.FirstName));
                d(this.Bind(ViewModel, vm => vm.LastName, v => v.LastName));
                d(this.Bind(ViewModel, vm => vm.Username, v => v.UserName));
                d(this.BindPassword(ViewModel, vm => vm.Password, v => v.Password.Text, Password));
                d(this.BindPassword(ViewModel, vm => vm.ConfirmPassword, v => v.ConfirmPassword.Text, ConfirmPassword));

                d(this.BindCommand(ViewModel, vm => vm.Save, v => v.Save));
                d(this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel));
            });
        }

        public CreateUserViewModel ViewModel
        {
            get { return (CreateUserViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (CreateUserViewModel), typeof (CreateUserView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CreateUserViewModel) value; }
        }
    }
}
