using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// string.isNullOrEmpty(string value)와 동일함.
    /// </summary>
    /// <param name="value">검사할 문자열</param>
    /// <returns>value가 null이거나 비어 있으면 true, 아니면 false</returns>
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
    
}
