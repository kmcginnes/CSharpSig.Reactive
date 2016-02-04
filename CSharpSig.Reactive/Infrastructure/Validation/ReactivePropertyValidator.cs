using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ReactiveUI;

namespace ImprovingU.Reactive.Infrastructure.Validation
{
    public abstract class ReactivePropertyValidator : ReactiveObject
    {
        public static ReactivePropertyValidator<TProp> For<TObj, TProp>(TObj This, Expression<Func<TObj, TProp>> property)
        {
            return new ReactivePropertyValidator<TObj, TProp>(This, property);
        }

        public static ReactivePropertyValidator<TProp> ForObservable<TProp>(IObservable<TProp> propertyObservable)
        {
            return new ReactivePropertyValidator<TProp>(propertyObservable);
        }

        public abstract ReactivePropertyValidationResult ValidationResult { get; }

        public abstract bool IsValidating { get; }

        public abstract Task<ReactivePropertyValidationResult> ExecuteAsync();

        public abstract Task ResetAsync();
    }
    
    public class ReactivePropertyValidator<TProp> : ReactivePropertyValidator
    {
        readonly ReactiveCommand<ReactivePropertyValidationResult> _validateCommand;
        ValidationParameter _currentValidationParameter;

        readonly ObservableAsPropertyHelper<ReactivePropertyValidationResult> _validationResult;
        public override ReactivePropertyValidationResult ValidationResult => _validationResult.Value;

        public override Task<ReactivePropertyValidationResult> ExecuteAsync()
        {
            return _validateCommand.ExecuteAsyncTask(_currentValidationParameter);
        }

        public override Task ResetAsync()
        {
            return _validateCommand.ExecuteAsync(new ValidationParameter { RequiresReset = true })
                .Select(_ => Unit.Default)
                .ToTask();
        }

        readonly List<Func<TProp, IObservable<ReactivePropertyValidationResult>>> _validators =
            new List<Func<TProp, IObservable<ReactivePropertyValidationResult>>>();

        readonly ObservableAsPropertyHelper<bool> _isValidating;
        public override bool IsValidating => _isValidating.Value;

        public ReactivePropertyValidator(IObservable<TProp> propertyChangeSignal)
        {
            _validateCommand = ReactiveCommand.CreateAsyncObservable(param =>
            {
                var validationParams = (ValidationParameter)param ?? new ValidationParameter();

                if (validationParams.RequiresReset)
                {
                    return Observable.Return(ReactivePropertyValidationResult.Unvalidated);
                }

                TProp value = validationParams.PropertyValue;

                var currentValidators = _validators.ToList();

                // HEAR YE, HEAR YE

                // This .ToList() is here to ignore changes to the validator collection,
                // and thus avoid fantastically vague exceptions about 
                // "Collection was modified, enumeration operation may not execute"
                // bubbling up to tear the application down

                // Thus, the collection will be correct when the command executes,
                // which should be fine until we need to do more complex validation

                if (!currentValidators.Any())
                    return Observable.Return(ReactivePropertyValidationResult.Unvalidated);

                return currentValidators.ToObservable()
                    .SelectMany(v => v(value))
                    .FirstOrDefaultAsync(x => x.Status == ValidationStatus.Invalid)
                    .Select(x => x ?? ReactivePropertyValidationResult.Success);
            });

            _isValidating = _validateCommand.IsExecuting.ToProperty(this, x => x.IsValidating);

            _validationResult = _validateCommand.ToProperty(this, x => x.ValidationResult);
            propertyChangeSignal
                .Select(x => new ValidationParameter { PropertyValue = x, RequiresReset = false })
                .Do(validationParameter => _currentValidationParameter = validationParameter)
                .Subscribe(validationParameter => _validateCommand.Execute(validationParameter));
        }

        public ReactivePropertyValidator<TProp> IfTrue(Func<TProp, bool> predicate, string errorMessage)
        {
            return Add(predicate, errorMessage);
        }

        public ReactivePropertyValidator<TProp> IfFalse(Func<TProp, bool> predicate, string errorMessage)
        {
            return Add(x => !predicate(x), errorMessage);
        }

        ReactivePropertyValidator<TProp> Add(Func<TProp, bool> predicate, string errorMessage)
        {
            return Add(x => predicate(x) ? errorMessage : null);
        }

        public ReactivePropertyValidator<TProp> Add(Func<TProp, string> predicateWithMessage)
        {
            _validators.Add(value => Observable.Defer(() => Observable.Return(Validate(value, predicateWithMessage))));
            return this;
        }

        public ReactivePropertyValidator<TProp> IfTrueAsync(Func<TProp, IObservable<bool>> predicate, string errorMessage)
        {
            AddAsync(x => predicate(x).Select(result => result ? errorMessage : null));
            return this;
        }

        public ReactivePropertyValidator<TProp> IfFalseAsync(Func<TProp, IObservable<bool>> predicate, string errorMessage)
        {
            AddAsync(x => predicate(x).Select(result => result ? null : errorMessage));
            return this;
        }

        public ReactivePropertyValidator<TProp> AddAsync(Func<TProp, IObservable<string>> predicateWithMessage)
        {
            _validators.Add(value => Observable.Defer(() =>
            {
                return predicateWithMessage(value)
                    .Select(result => String.IsNullOrEmpty(result)
                        ? ReactivePropertyValidationResult.Success
                        : new ReactivePropertyValidationResult(ValidationStatus.Invalid, result));

            }));

            return this;
        }

        static ReactivePropertyValidationResult Validate(TProp value, Func<TProp, string> predicateWithMessage)
        {
            var result = predicateWithMessage(value);

            if (String.IsNullOrEmpty(result))
                return ReactivePropertyValidationResult.Success;

            return new ReactivePropertyValidationResult(ValidationStatus.Invalid, result);
        }

        class ValidationParameter
        {
            public TProp PropertyValue { get; set; }
            public bool RequiresReset { get; set; }
        }
    }

    public class ReactivePropertyValidator<TObj, TProp> : ReactivePropertyValidator<TProp>
    {
        protected ReactivePropertyValidator() : base(Observable.Empty<TProp>())
        {
        }

        public ReactivePropertyValidator(TObj This, Expression<Func<TObj, TProp>> property)
            : base(This.WhenAny(property, x => x.Value))
        { }
    }
}