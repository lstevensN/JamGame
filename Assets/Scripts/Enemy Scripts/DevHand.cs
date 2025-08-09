using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class DevHand : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawn;

    bool playerCaught = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, playerSpawn.transform.position, 0.02f);
        }
        else this.transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, 0.001f);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collision");

        if (collision.gameObject.tag == "Player")
        {
            playerCaught = true;
        }
    }
}
