using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject m_door;
    [SerializeField] GameObject m_validation;
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
        if(collision.gameObject.tag == "Player")
        {
            if(collision.gameObject.GetComponent<PlayerController>().hasKey)
            {
                m_door.GetComponent<BoxCollider2D>().isTrigger = true;
                m_door.GetComponent<SpriteRenderer>().enabled = false;
                m_validation.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().hasKey)
            {
                m_door.GetComponent<BoxCollider2D>().isTrigger = false;
                m_door.GetComponent<SpriteRenderer>().enabled = true;
                m_validation.GetComponent<NoTeleport>().enabled = true;

            }
        }
    }
}
