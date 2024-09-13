namespace MovieQuotes.Application.Enums;


public enum ErrorCode
{
    NotFound = 404,

    // Validation Error 1000 - 1099
    ValidationError = 1001,

    // Infrastructure 2000 - 2099

    // Application Errors 3000- 3099

    // Other Error more than 4000
    UnKnownError = 4001,
}
