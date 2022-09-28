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

    public char Peek()
    {
        if (_index >= _text.Length)
        {
            throw new EndOfStringException();
        }
        return _text[_index];
    }

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