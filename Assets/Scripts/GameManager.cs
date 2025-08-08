using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerRef;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] bool SpawnPlayer = true;


    GameObject Player;

    bool Respawn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SpawnPlayer)
        {
            Player = Instantiate(PlayerRef, PlayerSpawnPoint.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) Respawn = true;
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
    }
}
