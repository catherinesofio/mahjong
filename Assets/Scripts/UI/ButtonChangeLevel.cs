using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeLevel : ButtonChangeScreen
{
    [SerializeField]
    private int _level;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private GameObject _star;

    protected override void TriggerChangeScreen()
    {
        var dataManager = GameObject.FindObjectOfType<DataManager>();
        dataManager.Level = _level;

        base.TriggerChangeScreen();
    }

    public ButtonChangeLevel SetLevel(int level)
    {
        _level = level;

        return this;
    }

    public ButtonChangeLevel SetText(string text)
    {
        _text.text = text;

        return this;
    }

    public ButtonChangeLevel ShowStar(bool show)
    {
        _star.SetActive(show);

        return this;
    }
}
