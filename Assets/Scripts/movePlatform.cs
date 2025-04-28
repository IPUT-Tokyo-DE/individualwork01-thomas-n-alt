using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class movePlatform : MonoBehaviour
{

    [SerializeField]
    private float speed = 0.02f;

    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;

    private float xDistance;
    private float yDistance;
    private float distance;

    private Vector2 startPos;
    private Vector2 endPos;
    private bool movingForward = true;

    BoxCollider2D boxCollider;
    Collider2D[] results = new Collider2D[5];

    

    // Start is called before the first frame update
    void Start()
    {
        startPos = start.transform.position;
        endPos = end.transform.position;

        xDistance = endPos.x - startPos.x;
        yDistance = endPos.y - startPos.y;
        distance = Mathf.Sqrt(xDistance * xDistance + yDistance * yDistance);

        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;

        if (reachedEnd())
        {
            movingForward = !movingForward;
            //Debug.Log("reached end");
        }

        if (movingForward == true)
        {
            pos.x += speed * xDistance / distance;
            pos.y += speed * yDistance / distance;
            transform.position = pos;
        }
        else if (movingForward == false)
        {
            pos.x -= speed * xDistance / distance;
            pos.y -= speed * yDistance / distance;
            transform.position = pos;
        }
    }

    bool reachedEnd()
    {
        int hitCount = boxCollider.OverlapCollider(new ContactFilter2D(), results);

        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if(results[i].tag=="PlatformEnds")
                {
                    return true;
                }
            }
        }

        return false;
    }


    
}
