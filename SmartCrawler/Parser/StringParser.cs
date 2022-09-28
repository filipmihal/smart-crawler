using System.Text.RegularExpressions;

namespace SmartCrawler.Parser;

public class StringParser
{
    private int _index = 0;
    private string _text;

    public bool IsEndOfString { get; private set; }

    public StringParser(string text)
    {
        _text = text;
        IsEndOfString = _text.Length == 0;
    }

    /// <summary>
    /// Equivalent to Stream Next
    /// </summary>
    public bool Next()
    {
        if (_index >= _text.Length - 1)
        {
            IsEndOfString = true;
            return false;
        }
        _index += 1;
        return true;
    }

    /// <summary>
    /// Returns the current character or throws an exception
    /// </summary>
    public char Peek()
    {
        if (_index >= _text.Length)
        {
            throw new EndOfStringException();
        }
        return _text[_index];
    }

    /// <summary>
    /// Reads all characters that satisfy the regex expression until it reaches one that does not
    /// Then returns those characters as a string
    /// <param name="condition">Constraint according to which characters are selected</param>
    /// </summary>
    public string ReadAll(Regex condition)
    {
        string result = "";
        while (condition.IsMatch(Peek().ToString()))
        {
            result += Peek();
            if (!Next())
            {
                return result;
            }
        }
        return result;
    }

    /// <summary>
    /// Reads all characters until one of the characters matches the condition
    /// <param name="condition">Constraint according to which characters are selected</param>
    /// </summary>
    public string ReadUntil(Regex condition)
    {
        string result = "";
        while (!condition.IsMatch(Peek().ToString()))
        {
            result += Peek();
            if (!Next())
            {
                return result;
            }
        }
        return result;
    }

    /// <summary>
    /// Skips all characters that satisfy the regex expression until it reaches one that does not
    /// <param name="skip">Skip rule</param>
    /// </summary>
    public void SkipAll(Regex skip)
    {
        while (skip.IsMatch(Peek().ToString()))
        {
            if (!Next())
            {
                return;
            }
        }
    }
}

class EndOfStringException : Exception { }