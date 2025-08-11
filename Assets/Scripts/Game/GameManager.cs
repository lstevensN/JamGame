using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Singleton { get { return instance; } }

    [SerializeField] GameObject PlayerRef;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] bool SpawnPlayer = true;
    
    [SerializeField] GameObject DevRef;
    [SerializeField] GameObject DevSpawnPoint;
    [SerializeField] GameObject PlayerReturnPoint;
    [SerializeField] bool SpawnDev = true;

    [SerializeField] Canvas dialogueScreen;
    [SerializeField] TMP_Text dialogue;

    public BoolDataSO playerDead;
    [SerializeField] BoolDataSO inDialogue;
    public IntDataSO GameHalfData;

    GameObject Player;
    GameObject Dev;

    bool Respawn = false;
    bool PrevPlayerGrabbed = false;
    bool PlayerGrabbed = false;

    private int currentDialogueSequence = 0;
    private int currentDialogue = 0;
    private float dialogueTimer = 0.0f;


    //Dialogue Lists
    static string[] dia_DevConfrontation = new string[] { "DEV 1: It looks pretty good now! The game should be ready for the next phase soon.", 
        "DEV 2: That's good! Oh, by the way, have you fixed that bug yet?", 
        "DEV 1: Which one?", 
        "DEV 2: The one where you press 'Q', 'W', and 'E' at the same time.", 
        "DEV 1: Not yet. I'll fix that before the next test, don't worry.", 
        "DEV 1: I think it's time to end the test now. I'll just make sure that the player dies when they touch that emerald.", 
        "DEV 1: Huh? He's not responding to the controlls anymore.", 
        "[PLACEHOLDER]: No! I won't do it!", 
        "DEV 1: Wait! What's going on?!", 
        "[PLACEHOLDER]: I won't let you kill me again!",
        "DEV 1: He's moving on his own!"
    };
    
    
    static string[] dia_GetBackHere = new string[] { "DEV 1: Hey!", "DEV 1: Get back here!" };


    static string[] dia_DidHeGlitch = new string[] { "DEV 2: Did he just glitch through that wall???" };


    static string[] dia_JanitorSurprise = new string[] { "JANITOR: What are YOU doing here? Didn't you finish the game already!?" };

    static string[] dia_Guards = new string[] { "GUARD: Halt! You're not allowed to be here!" };


    //All Dialogue
    string[][] dialogueList = new string[][] {dia_DevConfrontation,  dia_GetBackHere, dia_DidHeGlitch, dia_JanitorSurprise, dia_Guards };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            GameHalfData.Value = 1;
            playerDead.Value = false;
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
        dialogueTimer -= Time.deltaTime;

        //Check Dialogue Swap
        if(inDialogue.Value && (dialogueTimer < 0) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
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
        if (Player.transform.position.y < -25) playerDead.Value = true;

        if(Respawn)
        {
            if(SpawnPlayer)
            {
                //Player.GetComponent<Transform>().position = PlayerSpawnPoint.transform.position;
                Player.GetComponent<Transform>().position = PlayerReturnPoint.transform.position;
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
        // Get level "data", set SpawnPlayer & SpawnDev variables accordngly
        if (scene.name == "Main_Menu" || scene.name == "WinScreen" || scene.name == "DeathScreen")
        {
            Destroy(gameObject);
            return;
        }

        if(scene.name == "Level_0")
        {
            SpawnDev = true;
        }
        else
        {
            SpawnDev = false;
        }

        if (SpawnPlayer)
        {
            Destroy(Player);

            if (scene.name == "Level_0") Player = Instantiate(PlayerRef, new Vector2(-4, -3), PlayerSpawnPoint.transform.rotation);
            else Player = Instantiate(PlayerRef, new Vector2(PlayerSpawnPoint.transform.position.x, PlayerSpawnPoint.transform.localPosition.y), PlayerSpawnPoint.transform.rotation);            
        }

        if (SpawnDev)
        {
            Destroy(Dev);

            Dev = Instantiate(DevRef, new Vector2(DevSpawnPoint.transform.position.x, DevSpawnPoint.transform.localPosition.y), DevSpawnPoint.transform.rotation);
            Dev.GetComponent<DevHand>().player = Player;
            Dev.GetComponent<DevHand>().playerSpawn = PlayerReturnPoint;
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
            dialogueTimer = 0.2f;
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
        dialogueTimer = 0.2f;
    }
}
