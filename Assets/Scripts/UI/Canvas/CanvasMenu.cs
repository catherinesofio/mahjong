using UnityEngine;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField]
    private Transform _scrollViewContent;
    [SerializeField]
    private GameObject _prefabButtonLevel;

    private void Start()
    {
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        var levelCount = DataManager.LevelsCount;
        for (var i = 0; i < levelCount; i++)
        {
            var go = GameObject.Instantiate(_prefabButtonLevel, _scrollViewContent);

            var levelData = DataManager.GetLevelData(i);
            var btn = go.GetComponent<ButtonGoToLevel>()
                .SetLevel(i)
                .SetText((i + 1).ToString())
                .ShowStar(levelData.Item1, levelData.Item2);
        }
    }
}
