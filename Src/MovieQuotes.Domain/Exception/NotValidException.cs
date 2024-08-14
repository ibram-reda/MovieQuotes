namespace MovieQuotes.Domain.Exception;

using System;
using System.Collections.Generic;


public class NotValidException : Exception
{
    protected NotValidException()
    {
    }

    protected NotValidException(string? message) : base(message)
    {
    }

    protected NotValidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public List<string> ValidationErrors { get; } = [];
}
