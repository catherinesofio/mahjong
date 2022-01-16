using UnityEngine;
using UnityEngine.UI;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField]
    private Transform _scrollViewContent;
    [SerializeField]
    private GameObject _prefabButtonLevel;

    [SerializeField]
    private Text _soundOn;
    [SerializeField]
    private Text _musicOn;

    private void Start()
    {
        ToggleMusic();
        ToggleSound();

        CreateLevelButtons();

        EventManager.AddEventListener(EventId.TOGGLE_MUSIC, ToggleMusic);
        EventManager.AddEventListener(EventId.TOGGLE_SOUND, ToggleSound);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.TOGGLE_MUSIC, ToggleMusic);
        EventManager.RemoveEventListener(EventId.TOGGLE_SOUND, ToggleSound);
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

    private void ToggleMusic(object obj = null)
    {
        _musicOn.enabled = !AudioManager.MusicOn;
    }

    private void ToggleSound(object obj = null)
    {
        _soundOn.enabled = !AudioManager.SoundOn;
    }
}
