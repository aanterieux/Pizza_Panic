using System;
using System.Linq;
using UnityEngine;

public class CommonUtils : MonoBehaviour
{
    public static bool IsInRangeInclusive<T>(T _value, T _min, T _max)
        where T : IComparable<T>
    {
        if (_min.CompareTo(_max) > 0)
        {
            (_min, _max) = (_max, _min);
        }

        return (
            _value.CompareTo(_min) >= 0 &&
            _value.CompareTo(_max) <= 0
        );
    }
    public static bool IsInRangeExclusive<T>(T _value, T _min, T _max)
        where T : IComparable<T>
    {
        if (_min.CompareTo(_max) > 0)
        {
            (_min, _max) = (_max, _min);
        }

        return (
            _value.CompareTo(_min) > 0 &&
            _value.CompareTo(_max) < 0
        );
    }

    public static string NormaliseString(string _string)
    {
        return new string(
            _string
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpperInvariant)
                .ToArray()
        );
    }
}
