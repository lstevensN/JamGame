using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    public string scene;

    public bool destroyOnGlitch = false;

    private bool loading = false;

    private void Awake()
    {
        if (destroyOnGlitch && GameManager.Singleton.GameHalfData.Value == 2) Destroy(gameObject);
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
