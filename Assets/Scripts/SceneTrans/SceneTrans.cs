using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
    public string scene;

    private bool loading = false;

    private void OnEnable() { SceneManager.sceneLoaded += Trans; }

    private void OnDisable() { SceneManager.sceneLoaded -= Trans; }

    private void Trans(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == this.scene)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(this.scene));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (scene != "" && !loading && collision.CompareTag("Player"))
        {
            loading = true;
            print("Scene Trans! Destination: " + scene);
            SceneManager.LoadSceneAsync(scene);
        }
    }
}
