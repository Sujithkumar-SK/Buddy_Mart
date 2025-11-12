namespace Kanini.Ecommerce.Common;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Validation);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Validation
    );

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Database(string code, string description) =>
        new(code, description, ErrorType.Database);

    public static Error Unexpected(string code, string description) =>
        new(code, description, ErrorType.Unexpected);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Validation);
}
