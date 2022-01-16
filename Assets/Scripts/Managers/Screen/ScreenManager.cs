using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager _instance;

    private bool _isLoading;

    public static ScreenId Screen { get; private set; }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
            _isLoading = false;
            Screen = ScreenId.MENU;

            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        EventManager.AddEventListener(EventId.CHANGE_SCREEN, ChangeScreen);
    }

    private void ChangeScreen(object obj)
    {
        var screenId = (ScreenId)obj;

        if (!_isLoading)
        {
            Screen = screenId;

            _isLoading = true;
            StartCoroutine(LoadScreen((int)screenId));
        }
    }

    private IEnumerator LoadScreen(int screenId)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(screenId, LoadSceneMode.Single);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        _isLoading = false;
    }
}
