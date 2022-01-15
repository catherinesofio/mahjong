using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TilesData")]
public class TilesData : ScriptableObject
{
    public Color selectedColor;
    public Color unselectedColor;
    public Sprite[] sprites;
}
