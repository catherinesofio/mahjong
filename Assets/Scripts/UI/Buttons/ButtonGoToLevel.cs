using UnityEngine;
using UnityEngine.UI;

public class ButtonGoToLevel : ButtonChangeScreen
{
    [SerializeField]
    private int _level;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private GameObject _star;

    protected override void Start()
    {
        base.Start();

        EventManager.AddEventListener(EventId.DATA_RESET, RemoveStar);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.DATA_RESET, RemoveStar);
    }

    private void RemoveStar(object obj = null)
    {
        ShowStar(false, false);
    }

    protected override void TriggerChangeScreen()
    {
        DataManager.Level = _level;

        base.TriggerChangeScreen();
    }

    public ButtonGoToLevel SetLevel(int level)
    {
        _level = level;

        return this;
    }

    public ButtonGoToLevel SetText(string text)
    {
        _text.text = text;

        return this;
    }

    public ButtonGoToLevel ShowStar(bool show, bool firstTime)
    {
        _star.SetActive(show);

        if (show && firstTime)
        {
            _star.GetComponent<Animation>().Play();
        }

        return this;
    }
}
