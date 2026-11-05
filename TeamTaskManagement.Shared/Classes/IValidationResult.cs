namespace TeamTaskManagement.Shared.Classes {

    public interface IValidationResult {
        public static readonly Error ValidationError = new(
            "ValidationError",
            "A validation problem occurred.",
            "یک خطای اعتبارسنجی رخ داد");

        Error[] Errors { get; }
    }
}
