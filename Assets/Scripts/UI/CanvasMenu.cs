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
        var dataManager = GameObject.FindObjectOfType<DataManager>();

        var levelCount = dataManager.LevelsCount;
        for (var i = 0; i < levelCount; i++)
        {
            var go = GameObject.Instantiate(_prefabButtonLevel, _scrollViewContent);

            var levelData = dataManager.GetLevelData(i);
            var btn = go.GetComponent<ButtonChangeLevel>()
                .SetLevel(i)
                .SetText((i + 1).ToString())
                .ShowStar(levelData.Item1, levelData.Item2);
        }
    }
}
