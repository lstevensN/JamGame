using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    public string scene;

    public bool destroyOnGlitch = false;

    [SerializeField] private IntDataSO halfData;

    private bool loading = false;

    private void Awake()
    {
        if (destroyOnGlitch && halfData.Value == 2) Destroy(gameObject);
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
