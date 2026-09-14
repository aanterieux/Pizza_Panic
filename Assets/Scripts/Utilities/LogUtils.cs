using UnityEngine;

public static class LogUtils
{
    private const string INFO_COLOUR = "cyan";
    private const string WARNING_COLOUR = "yellow";
    private const string ERROR_COLOUR = "red";

    private const string SUCCESS_COLOUR = "green";
    private const string FAILURE_COLOUR = "red";

    private static string GetColouredText(in string _text, in string _colour)
    {
        return $"<color={_colour}>{_text}</color>";
    }

    public static void LogInfo(in string _text)
    {
        Debug.Log(GetColouredText(_text, INFO_COLOUR));
    }
    public static void LogWarning(in string _text)
    {
        Debug.LogWarning(GetColouredText(_text, WARNING_COLOUR));
    }
    public static void LogError(in string _text)
    {
        Debug.LogError(GetColouredText(_text, ERROR_COLOUR));
    }

    public static void LogCondition(in bool _condition, in string _text = "")
    {
        string colour =
            (_condition == true)
                ? SUCCESS_COLOUR
                : FAILURE_COLOUR;
        string text =
            (_text == "")
                ? _condition.ToString()
                : _text;

        Debug.Log(GetColouredText(text, colour));
    }
}
