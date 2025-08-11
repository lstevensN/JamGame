using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public float FadeSpeed = 1.0f;
    private CanvasGroup cg;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    private void Update()
    {
        if (GameManager.Singleton.playerDead.Value && cg.alpha < 1.0f)
        {
            cg.alpha += Time.deltaTime * FadeSpeed;

            if (cg.alpha >= 1.0f) SceneManager.LoadSceneAsync("DeathScreen");
        }
    }
}
