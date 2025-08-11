using UnityEngine;

public class Key : MonoBehaviour
{
    public bool destroyOnGlitch = true;

    private void Start()
    {
        if (destroyOnGlitch && GameManager.Singleton.GameHalfData.Value == 2) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().hasKey = true;
            Destroy(gameObject);
        }
    }
}
