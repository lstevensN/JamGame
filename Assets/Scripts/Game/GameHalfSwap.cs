using UnityEngine;

public class GameHalfSwap : MonoBehaviour
{
    public bool secondHalf = false;

    private void Start()
    {
        if (secondHalf && GameManager.Singleton.GameHalfData.Value == 1) Destroy(gameObject);
        if (!secondHalf && GameManager.Singleton.GameHalfData.Value == 2) Destroy(gameObject);
    }
}
