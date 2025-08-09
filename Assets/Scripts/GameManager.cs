using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerRef;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] bool SpawnPlayer = true;
    
    [SerializeField] GameObject DevRef;
    [SerializeField] GameObject DevSpawnPoint;
    [SerializeField] bool SpawnDev = true;



    GameObject Player;
    GameObject Dev;

    bool Respawn = false;
    bool PrevPlayerGrabbed = false;
    bool PlayerGrabbed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if(SpawnPlayer)
        {
            Player = Instantiate(PlayerRef, new Vector2(PlayerSpawnPoint.transform.position.x, PlayerSpawnPoint.transform.localPosition.y), PlayerSpawnPoint.transform.rotation);
        }
        
        if(SpawnDev)
        {
            Dev = Instantiate(DevRef, new Vector2(DevSpawnPoint.transform.position.x, DevSpawnPoint.transform.localPosition.y), DevSpawnPoint.transform.rotation);
            Dev.GetComponent<DevHand>().player = Player;
            Dev.GetComponent<DevHand>().playerSpawn = PlayerSpawnPoint;
        }
    }

 

    // Update is called once per frame
    void Update()
    {
        //respawn
        if(Input.GetKeyDown(KeyCode.R)) Respawn = true;

        //player grabbed
        if(Dev.GetComponent<DevHand>().playerCaught != PlayerGrabbed)
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
}
