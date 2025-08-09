using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    bool playerSpotted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerSpotted)
        {
            if(player.transform.position.x >)
            Attack();
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
        Vector2 direction = player.transform.position - transform.position;

    }
}
