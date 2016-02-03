using ImprovingU.Reactive.Infrastructure.Validation;
using ReactiveUI;

namespace ImprovingU.Reactive
{
    public class CreateUserViewModel : ReactiveObject
    {
        public CreateUserViewModel()
        {
            PasswordValidator = ReactivePropertyValidator.For(this, x => x.Password)
                .IfNullOrEmpty("Must enter a password");
            ConfirmPasswordValidator = ReactivePropertyValidator.For(this, x => x.ConfirmPassword)
                .IfNullOrEmpty("Must enter the same password again");

            var canSave = this.WhenAny(
                x => x.PasswordValidator.ValidationResult.IsValid,
                x => x.ConfirmPasswordValidator.ValidationResult.IsValid,
                (pass, conf) => pass.Value && conf.Value);

            Save = ReactiveCommand.Create(canSave);
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

        public ReactivePropertyValidator<string> PasswordValidator { get; }

        string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { this.RaiseAndSetIfChanged(ref _confirmPassword, value); }
        }

        public ReactivePropertyValidator<string> ConfirmPasswordValidator { get; }
    }
}
