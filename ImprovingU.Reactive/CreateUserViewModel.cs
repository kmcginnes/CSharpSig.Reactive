using System;
using System.Reactive;
using System.Reactive.Linq;
using ImprovingU.Reactive.Infrastructure.Validation;
using ReactiveUI;

namespace ImprovingU.Reactive
{
    public class CreateUserViewModel : ReactiveObject
    {
        public CreateUserViewModel()
        {
            Username = String.Empty;
            FirstName = String.Empty;
            LastName = String.Empty;
            Password = String.Empty;
            ConfirmPassword = String.Empty;

            PasswordValidator = ReactivePropertyValidator.For(this, x => x.Password)
                .IfNullOrEmpty("Must enter a password")
                .IfFalse(val => val.Length > 5, "Must be at least 5 characters");
            ConfirmPasswordValidator = ReactivePropertyValidator.For(this, x => x.ConfirmPassword)
                .IfNullOrEmpty("Must enter the same password again")
                .IfNotMatch(Password, "Must match password");

            var canSave = this.WhenAny(
                x => x.PasswordValidator.ValidationResult.IsValid,
                x => x.ConfirmPasswordValidator.ValidationResult.IsValid,
                (pass, conf) => pass.Value && conf.Value);

            Save = ReactiveCommand.CreateAsyncObservable(canSave, SaveUser);
            Save.IsExecuting.ToProperty(this, x => x.IsSaving, out _isSaving);

            Cancel = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Username = String.Empty;
                FirstName = String.Empty;
                LastName = String.Empty;
                Password = String.Empty;
                ConfirmPassword = String.Empty;
                await PasswordValidator.ResetAsync();
                await ConfirmPasswordValidator.ResetAsync();
            });
        }

        IObservable<Unit> SaveUser(object _)
        {
            // Fake some async work
            return Observable.Defer(() => Observable.Timer(TimeSpan.FromMilliseconds(3000)).ToUnit());
        }

        public ReactiveCommand<Unit> Save { get; }
        public ReactiveCommand<Unit> Cancel { get; }

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

        readonly ObservableAsPropertyHelper<bool> _isSaving;
        public bool IsSaving => _isSaving.Value;
    }
}
