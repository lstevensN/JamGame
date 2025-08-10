using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Singleton { get { return instance; } }

    [SerializeField] GameObject PlayerRef;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] bool SpawnPlayer = true;
    
    [SerializeField] GameObject DevRef;
    [SerializeField] GameObject DevSpawnPoint;
    [SerializeField] bool SpawnDev = true;

    [SerializeField] Canvas dialogueScreen;
    [SerializeField] TMP_Text dialogue;

    [SerializeField] BoolDataSO playerDead;
    [SerializeField] BoolDataSO inDialogue;




    GameObject Player;
    GameObject Dev;

    bool Respawn = false;
    bool PrevPlayerGrabbed = false;
    bool PlayerGrabbed = false;

    private int currentDialogueSequence = 0;
    private int currentDialogue = 0;


    //Dialogue Lists
    static string[] dialogueTest1 = new string[] { "Test 1", "Test 2", "Test 3" };
    static string[] dialogueTest2 = new string[] { "New Dialogue", "Hope This Works" };
    static string[] dialogueTest3 = new string[] { "Only One Dialogue Box Here" };


    //All Dialogue
    string[][] dialogueList = new string[][] {dialogueTest1,  dialogueTest2, dialogueTest3 };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to scene load event
        SceneManager.sceneLoaded += OnSceneLoad;

        //Hide Dialogue Screen
        dialogueScreen.gameObject.SetActive(false);

        // Spawn Player
        Player = Instantiate(PlayerRef, new Vector2(PlayerSpawnPoint.transform.position.x, PlayerSpawnPoint.transform.localPosition.y), PlayerSpawnPoint.transform.rotation);
    }

    void OnDestroy()
    {
        // Unsubscribe to scene load event
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    // Update is called once per frame
    void Update()
    {
        //Check Dialogue Swap
        if(inDialogue.Value && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            SwapDialogue();
        }

        //respawn
        if(Input.GetKeyDown(KeyCode.R) && !inDialogue.Value) Respawn = true;

        //player grabbed
        if(Dev && Dev.GetComponent<DevHand>().playerCaught != PlayerGrabbed)
        {
            PlayerGrabbed = Dev.GetComponent<DevHand>().playerCaught;
            if (PlayerGrabbed)
            {
                Player.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            else
            {
                Player.GetComponent<Rigidbody2D>().gravityScale = 1;
            }
        }

        // Adjust spawn point (kinda silly, but it works! :D)
        PlayerSpawnPoint.transform.position = new Vector2(
            Player.transform.position.x > 0 ? -15 : 15,
            Player.transform.position.y > 0 ? 2 : -5
            );
    }
 
    private void LateUpdate()
    {
        if(Respawn)
        {
            if(SpawnPlayer)
            {
                Player.GetComponent<Transform>().position = PlayerSpawnPoint.transform.position;
            }
            Respawn = false;
        }

        if(PlayerGrabbed)
        {
            Player.transform.position = Dev.transform.position;
        }
    }


    private void OnPlayerPickedUp()
    {
        print("Player Picked Up");
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // Get level "data", set SpawnPlayer & SpawnDev variables accordingly

        if (SpawnPlayer)
        {
            Destroy(Player);
            Player = Instantiate(PlayerRef, new Vector2(PlayerSpawnPoint.transform.position.x, PlayerSpawnPoint.transform.localPosition.y), PlayerSpawnPoint.transform.rotation);
        }

        if (SpawnDev)
        {
            Destroy(Dev);

            Dev = Instantiate(DevRef, new Vector2(DevSpawnPoint.transform.position.x, DevSpawnPoint.transform.localPosition.y), DevSpawnPoint.transform.rotation);
            Dev.GetComponent<DevHand>().player = Player;
            Dev.GetComponent<DevHand>().playerSpawn = PlayerSpawnPoint;
        }
    }


    public void OnDialogueEnter(int id)
    {
        if(id < dialogueList.Length && dialogueList[id].Length > 0)
        {
            currentDialogueSequence = id;
            currentDialogue = 0;
            inDialogue.Value = true;
            dialogueScreen.gameObject.SetActive(true);
            dialogue.SetText(dialogueList[id][0]);
        }
    }

    public void OnDialogueExit()
    {
        currentDialogueSequence = 0;
        currentDialogue = 0;
        inDialogue.Value = false;
        dialogueScreen.gameObject.SetActive(false);
        dialogue.SetText("");
    }

    
    private void SwapDialogue()
    {
        if(!inDialogue.Value) return;

        currentDialogue++;

        if (currentDialogue >= dialogueList[currentDialogueSequence].Length)
        {
            OnDialogueExit();
            return;
        }

        dialogue.SetText(dialogueList[currentDialogueSequence][currentDialogue]);
    }
}
