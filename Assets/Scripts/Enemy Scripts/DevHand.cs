using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class DevHand : MonoBehaviour
{
    [SerializeField] EventChannelSO PlayerGrabbedEvent;
    [SerializeField] BoolDataSO playerDead;
    [SerializeField] BoolDataSO InDialogue;
    [SerializeField] Sprite lightHand;
    [SerializeField] Sprite darkHand;



    public GameObject player;
    public GameObject playerSpawn;

    public bool playerCaught = false;
    public bool wait = false;

    float waitTimer = 0.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead.Value || InDialogue.Value) return;
        if (playerCaught)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, playerSpawn.transform.position, 0.01f);
        }
        else if (!wait) this.transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, 0.001f);

        waitTimer -= Time.deltaTime;
        if(waitTimer < 0.0f)
        {
            wait = false;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (InDialogue.Value) return;
        if (collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = darkHand;
            playerCaught = true;
        }

        if (collision.gameObject.tag == "PlayerSpawn")
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = lightHand;
            playerCaught = false;
            waitTimer = 2.0f;
        }
    }
}
