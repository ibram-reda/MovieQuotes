namespace MovieQuotes.Application.Common.Enums;


public enum ErrorCode
{
    NotFound = 404,

    // Validation Error 1000 - 1099
    ValidationError = 1001,

    // Infrastructure 2000 - 2099
    UpdateError = 2001,


    // Application Errors 3000- 3099
    TimeOutError = 3001,
    GenerateVideoClipError = 3003,
    FFMPEGError = 3007,



    // Other Error more than 4000
    UnKnownError = 4001,
    InvalidInput = 4002,
}
