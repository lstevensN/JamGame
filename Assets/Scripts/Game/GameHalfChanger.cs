using UnityEngine;

public class GameHalfChanger : MonoBehaviour
{
    [SerializeField] IntDataSO gameHalf;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameHalf.Value = 2;
        }
    }
}
