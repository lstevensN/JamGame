using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject broom;
    [SerializeField] BoolDataSO playerDead;
    [SerializeField] BoolDataSO InDialogue;


    GameObject player;
    bool playerSpotted = false;

    [SerializeField] float attackCD = 2f;
    float attackTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead.Value || InDialogue.Value) return;

        attackTimer -= Time.deltaTime;

        if(playerSpotted)
        {
            //if(player.transform.position.x >)
            if(attackTimer <= 0)
            {
                Attack();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            playerSpotted = true;
        }
    }


    private void Attack()
    {
        if (InDialogue.Value) return;
        attackTimer = attackCD;

        ///Vector2 direction = player.transform.position - transform.position;
        //Vector2 direction = player.GetComponent<Transform>().position - transform.position;

        
        GameObject m_broom = Instantiate(broom, transform.position, Quaternion.identity);
        m_broom.GetComponent<Broom>().direction = player.transform.position;
    }

    public void Die()
    {
        if (InDialogue.Value) return;
        print("Enemy Died");
        Destroy(gameObject);
    }
}
