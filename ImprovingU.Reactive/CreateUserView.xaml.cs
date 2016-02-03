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
                d(this.Bind(ViewModel, vm => vm.FirstName, v => v.FirstName.Text));
                d(this.Bind(ViewModel, vm => vm.LastName, v => v.LastName.Text));

                d(this.Bind(ViewModel, vm => vm.Email, v => v.Email.Text));
                d(this.OneWayBind(ViewModel, vm => vm.EmailValidator, v => v.EmailValidation.ReactiveValidator));

                d(this.BindPassword(ViewModel, vm => vm.Password, v => v.Password.Text, Password));
                d(this.OneWayBind(ViewModel, vm => vm.PasswordValidator, v => v.PasswordValidation.ReactiveValidator));

                d(this.BindPassword(ViewModel, vm => vm.ConfirmPassword, v => v.ConfirmPassword.Text, ConfirmPassword));
                d(this.OneWayBind(ViewModel, vm => vm.ConfirmPasswordValidator, v => v.ConfirmPasswordValidation.ReactiveValidator));

                d(this.BindCommand(ViewModel, vm => vm.Save, v => v.Save));
                d(this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel));

                d(this.OneWayBind(ViewModel, vm => vm.IsSaving, v => v.IsEnabled, value => !value));
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
