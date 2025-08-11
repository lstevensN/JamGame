using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScript : MonoBehaviour
{
    public void TryAgain()
    {
        SceneManager.LoadSceneAsync("Level_4");
    }
}
