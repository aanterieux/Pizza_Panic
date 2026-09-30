using UnityEngine;

public static class LogUtils
{
    private const string INFO_COLOUR = "cyan";
    private const string WARNING_COLOUR = "yellow";
    private const string ERROR_COLOUR = "red";

    private const string SUCCESS_COLOUR = "green";
    private const string FAILURE_COLOUR = "red";

    private static string ResolveColour(Color? _customColour, string _fallbackColour)
    {
        return
            (_customColour.HasValue)
                ? ColourUtils.ColorToHex(_customColour.Value)
                : _fallbackColour;
    }

    private static string GetColouredText(object _text, string _colour)
    {
        return $"<color={_colour.ToLower()}>{_text}</color>";
    }


    public static void LogInfo(object _text, Color? _colour = null)
    {
        string colour = ResolveColour(_colour, INFO_COLOUR);

        Debug.Log(GetColouredText(_text, colour));
    }
    public static void LogWarning(object _text, Color? _colour = null)
    {
        string colour = ResolveColour(_colour, WARNING_COLOUR);

        Debug.LogWarning(GetColouredText(_text, colour));
    }
    public static void LogError(object _text, Color? _colour = null)
    {
        string colour = ResolveColour(_colour, ERROR_COLOUR);

        Debug.LogError(GetColouredText(_text, colour));
    }
    public static void LogCondition(bool _condition, object _text = null, Color? _successColour = null, Color? _failureColour = null)
    {
        object text =
            (_text == null)
                ? _condition.ToString()
                : _text;

        string successColour = ResolveColour(_successColour, SUCCESS_COLOUR);
        string failureColour = ResolveColour(_failureColour, FAILURE_COLOUR);
        string colour =
            (_condition == true)
                ? successColour
                : failureColour;

        Debug.Log(GetColouredText(text, colour));
    }

}
