using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync("Level_4");
    }
}
