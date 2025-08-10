using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] public string scene;
    private bool loading = false;


    public void OnClickPlay()
    {
        //GameManager.Singleton.onscene
    

        if(!loading)
        {
            loading = true;
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        }
    }
}
