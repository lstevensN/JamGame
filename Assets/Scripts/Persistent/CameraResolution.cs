// Taken from https://discussions.unity.com/t/force-camera-aspect-ratio-16-9-in-viewport/616421/2, thank you very much!!

using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;

    private Camera camera;

    private void Awake() { camera = GetComponent<Camera>(); }

    private void Start() { RescaleCamera(); }

    private void Update() { RescaleCamera(); }

    private void OnPreCull()
    {
        if (Application.isEditor) return;

        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);

        Camera.main.rect = nr;
        GL.Clear(true, true, Color.black);

        Camera.main.rect = wp;
    }

    private void RescaleCamera()
    {
        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

        float targetAspect = 16f / 9f;
        float windowAspect = (float) Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }
}
