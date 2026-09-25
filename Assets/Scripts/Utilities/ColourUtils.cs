using System;
using UnityEngine;

public class ColourUtils
{
    public static string RGBToHex(int _r, int _g, int _b)
    {
        _r = Mathf.Clamp(_r, 0, 255);
        _g = Mathf.Clamp(_g, 0, 255);
        _b = Mathf.Clamp(_b, 0, 255);

        return $"#{_r:X2}{_g:X2}{_b:X2}";
    }
    public static string ColorToHex(Color _colour)
    {
        return
            RGBToHex(
                (int)(_colour.r * 255),
                (int)(_colour.g * 255),
                (int)(_colour.b * 255)
            );
    }

    public static (int r, int g, int b) HexToRGB(string _hexValue)
    {
        if (string.IsNullOrWhiteSpace(_hexValue))
        {
            throw new ArgumentException("Hex string is null or empty");
        }

        _hexValue = _hexValue.TrimStart('#');

        if (_hexValue.Length != 6)
        {
            throw new ArgumentException("Hex string must be 6 characters long");
        }

        int r = Convert.ToInt32(_hexValue.Substring(0, 2), 16);
        int g = Convert.ToInt32(_hexValue.Substring(2, 2), 16);
        int b = Convert.ToInt32(_hexValue.Substring(4, 2), 16);

        return (r, g, b);
    }
    public static Color HexToColor(string _hexValue)
    {
        (int r, int g, int b) rgbValues = HexToRGB(_hexValue);

        return
            new Color(
                rgbValues.r,
                rgbValues.g,
                rgbValues.b
            );
    }

    public static Color RandomRGB01(float _minValue = 0f, float _maxValue = 1f)
    {
        _minValue = Mathf.Clamp(_minValue, 0f, 0.49f);
        _maxValue = Mathf.Clamp(_maxValue, 0.5f, 1f);

        return new Color(
            UnityEngine.Random.Range(_minValue, _maxValue),
            UnityEngine.Random.Range(_minValue, _maxValue),
            UnityEngine.Random.Range(_minValue, _maxValue)
        );
    }
    public static Color RandomRGBA01(float _minValue = 0f, float _maxValue = 1f)
    {
        _minValue = Mathf.Clamp(_minValue, 0f, 0.49f);
        _maxValue = Mathf.Clamp(_maxValue, 0.5f, 1f);

        Color randRGBA = RandomRGB01(_minValue, _maxValue);
        randRGBA.a = UnityEngine.Random.Range(_minValue, _maxValue);

        return randRGBA;
    }

    public static Color RandomRGB0255(int _minValue = 0, int _maxValue = 255)
    {
        _maxValue++;

        _minValue = Mathf.Clamp(_minValue, 0, 127);
        _maxValue = Mathf.Clamp(_maxValue, 128, 256);

        return new Color(
            UnityEngine.Random.Range(_minValue, _maxValue) / 255f,
            UnityEngine.Random.Range(_minValue, _maxValue) / 255f,
            UnityEngine.Random.Range(_minValue, _maxValue) / 255f
        );
    }
    public static Color RandomRGBA0255(int _minValue = 0, int _maxValue = 255)
    {
        Color randRGBA = RandomRGB0255(_minValue, _maxValue);
        int alphaMax = Mathf.Clamp(_maxValue + 1, 128, 256);
        int alphaMin = Mathf.Clamp(_minValue, 0, 127);

        randRGBA.a = UnityEngine.Random.Range(alphaMin, alphaMax) / 255f;
        return randRGBA;
    }

    public static Color Inverse(Color _colour, bool _inverseAlpha = false)
    {
        return new Color(
            1f - _colour.r,
            1f - _colour.g,
            1f - _colour.b,
            (_inverseAlpha)
                ? 1f - _colour.a
                : _colour.a
        );
    }

    public static (float r, float g, float b) Value01(int _r, int _g, int _b)
    {
        return (_r / 255f, _g / 255f, _b / 255f);
    }
    public static (float r, float g, float b) Value0255(float _r, float _g, float _b)
    {
        return (_r * 255f, _g * 255f, _b * 255f);
    }
}
