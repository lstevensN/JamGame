using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class DevHand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collision");

        if (collision.gameObject.tag == "Player")
        {
            print("Player Caught");
        }
    }
}
