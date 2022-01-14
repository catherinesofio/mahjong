using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    private int _level;
    private bool _isLoading;
    private static ScreenManager _instance;

    public int Level
    {
        get
        {
            return _level;
        }
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
        }

        _isLoading = false;

        DontDestroyOnLoad(this);
    }

    public void ChangeScreen(ScreenId screenId, bool allowSceneActivation = true, int level = -1)
    {
        if (!_isLoading)
        {
            _level = level;
            _isLoading = true;
            StartCoroutine(LoadScreen((int)screenId, allowSceneActivation));
        }
    }

    private IEnumerator LoadScreen(int screenId, bool allowSceneActivation)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(screenId, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = allowSceneActivation;

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        _isLoading = false;
    }
}
