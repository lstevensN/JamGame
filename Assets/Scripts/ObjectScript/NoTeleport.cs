using Unity.VisualScripting;
using UnityEngine;

public class NoTeleport : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TeleportBack();
        }
    }
}
