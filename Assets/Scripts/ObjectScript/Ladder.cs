using UnityEngine;
using UnityEngine.InputSystem;

public class Ladder : MonoBehaviour
{
    [SerializeField] BoolDataSO playerIsClimbing;
    [SerializeField] FloatDataSO playerClimbingSpeed;



    Rigidbody2D rb;


    //CharacterController2D characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (rb != null) rb.gravityScale = 0;
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.TryGetComponent<Rigidbody2D>(out rb);
            playerIsClimbing.Value = true;
            //collision.gameObject.TryGetComponent<CharacterController2D>(out characterController);
        }
        //rb.gravityScale = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.spaceKey.isPressed)//  .zKey.wasPressedThisFrame)
        {
            playerClimbingSpeed.Value = 2.5f;
        }
        else if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.leftCtrlKey.isPressed)
        {
            playerClimbingSpeed.Value = -2.5f;
        }
        else
        {
            playerClimbingSpeed.Value = 0;
        }
    }

    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.gravityScale = 1;

            rb = null;
            //characterController.isClimbing = false;
            playerIsClimbing.Value = false;

        }
    }

    //private void OnTriggerEnter(Collider2D collision)
    //{
    //    
    //}

    //private void OnTriggerExit(Collider2D collision)
    //{
    //    rb.gravityScale = 1;
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        rb = null; 
    //    }
    //}
}
