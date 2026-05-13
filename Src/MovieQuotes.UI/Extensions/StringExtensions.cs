namespace MovieQuotes.UI.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Input.TextInput;

public static class StringExtensions
{
    public static string? RemoveSpace(this string? input)
    {
        return Regex.Replace(input?? "", @"\s+", " ").Trim();
    }
    public static string? ReplaceNewLine(this string? input)
    {
        return input?.Replace(Environment.NewLine, "<br>").Replace("\n", "<br>");
    }

    public static string? GetMurkupList(this string? Example)
    {
        if (string.IsNullOrEmpty(Example)) return "";
        var items = Example?.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (items == null || items.Length == 0) return "";
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            // remove leading dashes
            var cleanItem = item.TrimStart().TrimStart('-').TrimStart();
            sb.Append($"<li>{cleanItem}</li>");
        }
        return $"<ul>{sb}</ul>";
    }

    public static string? GetOptionalMurkupList(this string? Example)
    {
        if (string.IsNullOrEmpty(Example)) return "";
        var items = Example?.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (items == null || items.Length == 0) return "";
        var sb = new StringBuilder();
        if(items.All(i => i.TrimStart().StartsWith("-")))
        {
            foreach (var item in items)
            {
                // remove leading dashes
                var cleanItem = item.TrimStart().TrimStart('-').TrimStart();
                sb.Append($"<li>{cleanItem}</li>");
            }
            return $"<ul>{sb.ToString()}</ul>";
        } 
         return Example;
    }

}