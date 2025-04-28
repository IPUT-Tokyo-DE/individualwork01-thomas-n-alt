using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Camera Camera;

    private Collider2D col;
    private Rigidbody2D rb2d;
    private float x;
    private bool move=true;
    private float lapsetime;
    private bool isOnGround = false;
    private bool GameClear = false;

    private List<Vector3> cameraPositions = new List<Vector3>();
    private int cameraPositionNumber = 0;

    public AudioSource clearAudio;



    [SerializeField]
    Vector2 jump = new Vector2(4,0);

    [SerializeField]
    float sideForce = 0;

    [SerializeField] float sideForceAir = 0;

    [SerializeField]
    float minX = -11.0f;
    [SerializeField]
    float maxX = 11.0f;

    [SerializeField]
    private Collider2D ignoreCollider;
    [SerializeField]
    private Collider2D ignoreCollider2;

    private Collider2D ignoreFloorCollider;
    private bool ignoringfloor = false;

    [SerializeField]
    private GameObject endScreen;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        lapsetime = 0.0f;

        col = GetComponent<Collider2D>();

        cameraPositions.Add(new Vector3(0, 0, -10));
        cameraPositions.Add(new Vector3(0, 10, -10));
        cameraPositions.Add(new Vector3(0,20,-10));
        cameraPositions.Add(new Vector3(0, 30, -10));
        cameraPositions.Add(new Vector3(0, 40, -10));
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.IgnoreCollision(ignoreCollider, col);
        Physics2D.IgnoreCollision(ignoreCollider2, col);

        var pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;

        Camera.transform.position = cameraPositions[cameraPositionNumber];

        if (move==true&&GameClear==false)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (isOnGround == true)
                {
                    Vector2 force = new Vector2(sideForce, 0);
                    rb2d.AddForce(force);
                    move = false;
                }
                else if (isOnGround == false)
                {
                    Vector2 force = new Vector2(sideForceAir, 0);
                    rb2d.AddForce(force);
                    move = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (isOnGround == true)
                {
                    Vector2 force = new Vector2(sideForce*-1.0f, 0);
                    rb2d.AddForce(force);
                    move = false;
                }
                else if (isOnGround == false)
                {
                    Vector2 force = new Vector2(sideForceAir*-1.0f, 0);
                    rb2d.AddForce(force);
                    move = false;
                }

            }
        }

        if (move == false) 
        {
            lapsetime += Time.deltaTime;
            if (lapsetime >= 0.5)
            {
                move = true;
                lapsetime = 0.0f;
            }
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;
        }
        //Debug.Log("onGround");
        if(collision.gameObject.tag == "PlatformEnds")
        {
            Physics2D.IgnoreCollision(collision.collider, col);
        }
    }

    

    

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isOnGround = false;
            //Debug.Log("notOnGround");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "JumpPad")
        {
            rb2d.velocity=new Vector2(rb2d.velocity.x,0);
            rb2d.AddForce(jump);
;       }
        if (collision.gameObject.tag == "IgnoreFloor")
        {
            var floor = collision.transform.parent.gameObject;
            ignoreFloorCollider = floor.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(ignoreFloorCollider, col);
            ignoringfloor = true;
            //Debug.Log(ignoringfloor);
        }
        if (collision.gameObject.tag == "StickyFloor"&&ignoringfloor==false)
        {
            transform.SetParent(collision.transform);
            //Debug.Log("onFloor");
        } 

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CameraBorder")
        {
            if (rb2d.velocity.y < 0 && cameraPositionNumber != 0)
            {
                cameraPositionNumber--;
            }

            else if (rb2d.velocity.y > 0 && cameraPositionNumber != cameraPositions.Count - 1)
            {
                cameraPositionNumber++;
            }
        }
        if (collision.gameObject.tag == "IgnoreFloor")
        {
            Physics2D.IgnoreCollision(ignoreFloorCollider, col,false);
            ignoringfloor = false;
        }
        if(collision.gameObject.tag == "StickyFloor")
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Goal"){
            clearAudio.Play();
            GameClear = true;
            endScreen.SetActive(true);
        }
    }
}
