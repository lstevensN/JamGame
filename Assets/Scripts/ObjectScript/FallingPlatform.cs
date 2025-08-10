using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]


public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float FallTime = 2f;

    private float timer = 0.0f;
    private bool isActivated = false;

    private void Update()
    {
        if(isActivated)
        {
            timer += Time.deltaTime;
        }

        if(timer > FallTime)
        {
            isActivated = false;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isActivated = true;
        }
    }
}
