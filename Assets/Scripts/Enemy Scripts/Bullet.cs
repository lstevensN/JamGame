using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] public float lifespan = 20;

    [SerializeField] BoolDataSO playerDead;
    [SerializeField] BoolDataSO InDialogue;



    public Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead.Value || InDialogue.Value) return;

        lifespan -= Time.deltaTime;
        if (lifespan < 0 )
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (playerDead.Value || InDialogue.Value) return;

        transform.position += new Vector3(direction.x * speed, direction.y * speed) * Time.deltaTime;
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
