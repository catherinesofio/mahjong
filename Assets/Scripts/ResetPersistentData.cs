using System.IO;
using UnityEngine;

public class ResetPersistentData : MonoBehaviour
{
    private void Start()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
        foreach (string filePath in filePaths) {
            File.Delete(filePath);
        }
    }
}
