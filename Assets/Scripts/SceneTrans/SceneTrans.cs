using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    public string scene;

    public bool showAfterGlitch = false;
    public bool destroyAfterGlitch = false;

    private bool loading = false;

    private void Start()
    {
        if (showAfterGlitch && GameManager.Singleton.GameHalfData.Value == 1) Destroy(gameObject);
        if (destroyAfterGlitch && GameManager.Singleton.GameHalfData.Value == 2) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (scene != "" && !loading && collision.CompareTag("Player"))
        {
            loading = true;
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        }
    }
}
