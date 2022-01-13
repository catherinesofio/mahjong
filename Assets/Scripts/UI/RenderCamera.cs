using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class RenderCamera : MonoBehaviour
{
    private void Start()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }
}
