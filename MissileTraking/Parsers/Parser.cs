namespace MissileTracking.Parsers;

using System;
using System.Collections.Generic;

public class Parser
{
    /// <summary>
    /// Parses a command string into a dictionary containing the command type and arguments.
    /// Expected format: "CommandType@Arguments"
    /// </summary>
    /// <param name="input">The input command string.</param>
    /// <returns>A dictionary with keys "CommandType" and optionally "args".</returns>
    public static Dictionary<string, string> Parse(string input)
    {
        var result = new Dictionary<string, string>();

        // Validate input: Ensure it's not null, empty, or just whitespace
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null, empty, or whitespace.");
        }

        // Split input based on '@' separator
        string[] parts = input.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);

        // Validate that a command type exists
        if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
        {
            throw new FormatException("Invalid command format: Missing command type.");
        }

        // Assign command type
        result["CommandType"] = parts[0].Trim();

        // Assign arguments if they exist
        if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
        {
            result["args"] = parts[1].Trim();
        }

        return result;
    }
}