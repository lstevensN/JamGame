using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] int id = 0;
    [SerializeField] IntDataSO gameHalf;

    [SerializeField] bool RepeatDialogue = false;
    [SerializeField] bool GameHalfDependent = false;
    [SerializeField] int RequiredHalf = 0; //False = 1st half, True = 2nd half


    private bool dialogueTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dialogueTriggered && !RepeatDialogue) return;

        if(GameHalfDependent && (RequiredHalf != gameHalf.Value)) return;

        if(collision.gameObject.tag == "Player")
        {
            GameManager.Singleton.OnDialogueEnter(id);
        }

        dialogueTriggered = true;
    }
}
