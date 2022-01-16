using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonChangeScreen : MonoBehaviour
{
    [SerializeField]
    private ScreenId _screenId;

    protected virtual void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(TriggerChangeScreen);
    }

    protected virtual void TriggerChangeScreen()
    {
        EventManager.DispatchEvent(EventId.CHANGE_SCREEN, (object)_screenId);
        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.BUTTON_PRESS);
    }
}
