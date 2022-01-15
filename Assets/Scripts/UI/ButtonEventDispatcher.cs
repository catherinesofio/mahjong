using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEventDispatcher : MonoBehaviour
{
    [SerializeField]
    private EventId _eventId;

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(CallEvent);
    }

    private void CallEvent()
    {
        EventManager.DispatchEvent(_eventId);
    }
}
