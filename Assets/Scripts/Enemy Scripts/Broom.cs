using UnityEngine;

public class Broom : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] public float lifespan = 20;

    [SerializeField] BoolDataSO playerDead;


    public Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead.Value) return;

        lifespan -= Time.deltaTime;
        if (lifespan < 0 )
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (playerDead.Value) return;

        transform.position = Vector2.MoveTowards(transform.position, direction, speed);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Die();
        }

        if(collision.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);

        }
    }
}
