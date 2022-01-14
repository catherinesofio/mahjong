using UnityEngine;
using UnityEngine.UI;

public class CanvasGame : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    private void Awake()
    {
        UpdateScore(0);
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = score.ToString();
    }
}
