using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] GameManager gameManager;

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
            gameManager.OnDialogue(id);
        }
    }
}
