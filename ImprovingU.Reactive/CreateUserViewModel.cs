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
            Email = String.Empty;
            FirstName = String.Empty;
            LastName = String.Empty;
            Password = String.Empty;
            ConfirmPassword = String.Empty;

            EmailValidator = ReactivePropertyValidator.For(this, x => x.Email)
                .IfNullOrEmpty("Must enter an email")
                .IfNotEmail("Must be a valid email");
            PasswordValidator = ReactivePropertyValidator.For(this, x => x.Password)
                .IfNullOrEmpty("Must enter a password")
                .IfFalse(val => val.Length > 5, "Must be at least 5 characters");
            ConfirmPasswordValidator = ReactivePropertyValidator.For(this, x => x.ConfirmPassword)
                .IfNullOrEmpty("Must enter the same password again")
                .IfNotMatch(Password, "Must match password");

            var canSave = this.WhenAny(
                x => x.EmailValidator.ValidationResult.IsValid,
                x => x.PasswordValidator.ValidationResult.IsValid,
                x => x.ConfirmPasswordValidator.ValidationResult.IsValid,
                (email, pass, conf) => email.Value && pass.Value && conf.Value);

            Save = ReactiveCommand.CreateAsyncObservable(canSave, SaveUser);
            Save.IsExecuting.ToProperty(this, x => x.IsSaving, out _isSaving);

            Cancel = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Email = String.Empty;
                FirstName = String.Empty;
                LastName = String.Empty;
                Password = String.Empty;
                ConfirmPassword = String.Empty;
                await EmailValidator.ResetAsync();
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

        string _email;
        public string Email
        {
            get { return _email; }
            set { this.RaiseAndSetIfChanged(ref _email, value); }
        }

        public ReactivePropertyValidator<string> EmailValidator { get; }

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
