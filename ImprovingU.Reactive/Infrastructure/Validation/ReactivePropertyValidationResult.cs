namespace ImprovingU.Reactive.Infrastructure.Validation
{
    public class ReactivePropertyValidationResult
    {
        /// <summary>
        /// Describes if the property passes validation
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Describes which state we are in - Valid, Not Validated, or Invalid
        /// </summary>
        public ValidationStatus Status { get; }

        /// <summary>
        /// An error message to display
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Describes if we should show this error in the UI
        /// We only show errors which have been marked specifically as Invalid
        /// and we do not show errors for inputs which have not yet been validated. 
        /// </summary>
        public bool DisplayValidationError { get; }
        
        public static readonly ReactivePropertyValidationResult Success = new ReactivePropertyValidationResult(ValidationStatus.Valid);
        public static readonly ReactivePropertyValidationResult Unvalidated = new ReactivePropertyValidationResult();

        public ReactivePropertyValidationResult() : this(ValidationStatus.Unvalidated, "")
        {
        }

        public ReactivePropertyValidationResult(ValidationStatus validationStatus) : this(validationStatus, "")
        {
        }

        public ReactivePropertyValidationResult(ValidationStatus validationStatus, string message)
        {
            Status = validationStatus;
            IsValid = validationStatus == ValidationStatus.Valid;
            DisplayValidationError = validationStatus == ValidationStatus.Invalid;
            Message = message;
        }
    }
}