using System.IO;
using UnityEngine;

public class Utils
{
    public static bool GetBoolPlayerPref(string name)
    {
        var pref = PlayerPrefs.GetInt(name, 1);

        return (pref > 0);
    }

    public static void SetBoolPlayerPref(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    public static string ReadTextFile(string path)
    {
        var fullPath = $"{Application.dataPath}/{path}";
        if (File.Exists(fullPath))
        {
            var text = File.ReadAllText(fullPath);

            return text;
        }

        return "";
    }

    public static T ReadJson<T>(string path)
    {
        var fullPath = $"{Application.persistentDataPath}/{path}";
        if (File.Exists(fullPath))
        {
            var jsonData = File.ReadAllText(fullPath);
            var data = JsonUtility.FromJson<T>(jsonData);

            return data;
        }

        return default;
    }

    public static void WriteJson<T>(T data, string path)
    {
        var fullPath = $"{Application.persistentDataPath}/{path}";
        var jsonData = JsonUtility.ToJson(data);

        File.WriteAllText(fullPath, jsonData);
    }

    public static void DeletePersistentData()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
        foreach (string filePath in filePaths)
        {
            File.Delete(filePath);
        }
    }
}
