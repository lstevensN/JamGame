using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    public Scene scene;

    private bool loading = false;

    private void OnEnable() { SceneManager.sceneLoaded += Trans; }

    private void OnDisable() { SceneManager.sceneLoaded -= Trans; }

    private void Trans(Scene scene, LoadSceneMode mode)
    {
        if (scene == this.scene)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.SetActiveScene(this.scene);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!loading && collision.CompareTag("Player"))
        {
            loading = true;
            print("Scene Trans! Destination: " + scene.name);
        }
    }
}
