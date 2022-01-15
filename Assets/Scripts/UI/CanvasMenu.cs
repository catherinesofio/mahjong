using UnityEngine;

public class CanvasMenu : MonoBehaviour
{
    [Min(0)]
    [SerializeField]
    private int _buttonLevelSize;
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
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,  _buttonLevelSize * - i);
            go.GetComponent<ButtonChangeLevel>()
              .SetText((i + 1).ToString())
              .SetLevel(i)
              .ShowStar(dataManager.IsLevelCompleted(i));
        }
    }
}
