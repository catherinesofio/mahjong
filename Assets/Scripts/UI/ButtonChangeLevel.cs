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

    public ButtonChangeLevel ShowStar(bool show, bool firstTime)
    {
        _star.SetActive(show);

        if (show && firstTime)
        {
            _star.GetComponent<Animation>().Play();
        }

        return this;
    }
}
