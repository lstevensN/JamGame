using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] GameManager gameManager;

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameManager.OnDialogueEnter(id);
        }
    }
}
