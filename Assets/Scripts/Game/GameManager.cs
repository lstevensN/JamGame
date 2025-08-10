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
    [SerializeField] IntDataSO GameHalf;




    GameObject Player;
    GameObject Dev;

    bool Respawn = false;
    bool PrevPlayerGrabbed = false;
    bool PlayerGrabbed = false;

    private int currentDialogueSequence = 0;
    private int currentDialogue = 0;


    //Dialogue Lists
    static string[] dia_DevConfrontation = new string[] { "DEV 1: It looks pretty good now! The game should be ready for the next phase soon.", 
        "DEV 2: That's good! Oh, by the way, have you fixed that bug yet?", 
        "DEV 1: Which one?", 
        "DEV 2: The one where you press 'Q', 'W', and 'E' at the same time.", 
        "DEV 1: Not yet. I'll fix that before the next test, don't worry.", 
        "DEV 1: I think it's time to end the test now. I'll just make sure that the player dies when they touch that emerald.", 
        "DEV 1: Huh? He's not responding to the controlls anymore.", 
        "[Placeholder]: No! I won't do it!", 
        "DEV 1: Wait! What's going on?!", 
        "[Placeholder]: I won't let you kill me again!",
        "DEV 1: He's moving on his own!"
    };
    
    
    static string[] dia_GetBackHere = new string[] { "DEV 1: Hey!", "DEV 1: Get back here!" };


    static string[] dia_DidHeGlitch = new string[] { "DEV 2: Did he just glitch through that wall???" };


    //All Dialogue
    string[][] dialogueList = new string[][] {dia_DevConfrontation,  dia_GetBackHere, dia_DidHeGlitch };


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
            Player.transform.position.y > 0 ? 3 : -4
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
