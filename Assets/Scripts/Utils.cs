using System.IO;
using UnityEngine;

public class Utils
{
    internal static string ReadTextFile(string path)
    {
        var fullPath = $"{Application.dataPath}/{path}";
        if (File.Exists(fullPath))
        {
            var streamReader = new StreamReader(fullPath);
            var text = streamReader.ReadToEnd();

            return text;
        }

        return "";
    }

    internal static T ReadJson<T>(string path)
    {
        var fullPath = $"{Application.persistentDataPath}/{path}";
        if (File.Exists(fullPath))
        {
            var data = JsonUtility.FromJson<T>(fullPath);

            return data;
        }

        return default;
    }

    internal static void WriteJson<T>(T data, string path)
    {
        var fullPath = $"{Application.persistentDataPath}/{path}";
        var jsonData = JsonUtility.ToJson(data);

        System.IO.File.WriteAllText(fullPath, jsonData);
    }
}
