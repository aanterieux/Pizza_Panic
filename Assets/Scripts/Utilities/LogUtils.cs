using System.Diagnostics;
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


    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(object _text, Color? _colour = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        string colour = ResolveColour(_colour, INFO_COLOUR);

        UnityEngine.Debug.Log(GetColouredText(_text, colour));
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object _text, Color? _colour = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        string colour = ResolveColour(_colour, WARNING_COLOUR);

        UnityEngine.Debug.LogWarning(GetColouredText(_text, colour));
#endif
    }
    
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object _text, Color? _colour = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        string colour = ResolveColour(_colour, ERROR_COLOUR);

        UnityEngine.Debug.LogError(GetColouredText(_text, colour));
#endif
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogCondition(bool _condition, object _text = null, Color? _successColour = null, Color? _failureColour = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
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

        UnityEngine.Debug.Log(GetColouredText(text, colour));
#endif
    }
}
