using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ExtensionMethods
{

    public static List<int> AllIndexesOf(this string str, string value)
    {
        if (String.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }

}

public static class DialogueUtilities
{
    public static float DecreasingFunction(float input)
    {
        return 1 / input;
    }

    public static float FloatToByte(float valueToConvert)
    {
        float b = (valueToConvert >= 1.0 ? 255 : (valueToConvert <= 0.0 ? 0 : (int)Mathf.Floor(valueToConvert * 256.0f)));
        b = Mathf.FloorToInt(b);
        return b;
    }

    public static string ReplaceWords(string originalString, DialogueDictionary dictionary)
    {

        Regex reg = new Regex(@"<.*?/>");
        MatchCollection matches = reg.Matches(originalString);
        string interpolatedString = originalString;
        foreach (Match match in matches)
        {
            GroupCollection group = match.Groups;
            foreach (Group key in group)
            {
                if (key.Value.Length <= 3 || string.IsNullOrWhiteSpace(key.Value.Substring(1, key.Value.Length - 3)))
                {
                    Debug.LogWarning("Empty dictionary key found in text.");
                    continue;
                }
                else
                {
                    string keyValue = key.Value.Substring(1, key.Value.Length - 3);
                    string dictionaryValue = dictionary.GetEntry(keyValue);
                    if (dictionaryValue != null)
                    {
                        interpolatedString = interpolatedString.Replace(key.Value, dictionaryValue);
                    }
                }
            }
        }

        return interpolatedString;
    }

}

