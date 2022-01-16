using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonChangeScreen : MonoBehaviour
{
    [SerializeField]
    private bool _allowSceneActivation;
    [SerializeField]
    private ScreenId _screenId;

    protected virtual void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(TriggerChangeScreen);
    }

    protected virtual void TriggerChangeScreen()
    {
        var screenManager = GameObject.FindObjectOfType<ScreenManager>();
        screenManager.ChangeScreen(_screenId, _allowSceneActivation);
    }
}
