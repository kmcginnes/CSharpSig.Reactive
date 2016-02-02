using ReactiveUI;

namespace ImprovingU.Reactive
{
    public class CreateUserViewModel : ReactiveObject
    {
        public CreateUserViewModel()
        {
            Save = ReactiveCommand.Create();
            Cancel = ReactiveCommand.Create();
        }

        public ReactiveCommand<object> Save { get; }
        public ReactiveCommand<object> Cancel { get; }

        string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { this.RaiseAndSetIfChanged(ref _firstName, value); }
        }

        string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { this.RaiseAndSetIfChanged(ref _lastName, value); }
        }

        string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { this.RaiseAndSetIfChanged(ref _confirmPassword, value); }
        }
    }
}
