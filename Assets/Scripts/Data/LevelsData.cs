using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/LevelsData")]
public class LevelsData : ScriptableObject
{
    public string folder;
    public LevelModel[] data;
}
