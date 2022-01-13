using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonChangeScreen : MonoBehaviour
{
    [SerializeField]
    private int _level;
    [SerializeField]
    private bool _allowSceneActivation;
    [SerializeField]
    private ScreenId _screenId;

    [SerializeField]
    private Text _text;

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(TriggerChangeScreen);
    }

    private void TriggerChangeScreen()
    {
        var screenManager = GameObject.FindObjectOfType<ScreenManager>();
        screenManager.ChangeScreen(_screenId, _allowSceneActivation, _level);
    }

    internal ButtonChangeScreen SetLevel(int level)
    {
        _level = level;

        return this;
    }

    internal ButtonChangeScreen SetText(string text)
    {
        _text.text = text;
     
        return this;
    }
}
