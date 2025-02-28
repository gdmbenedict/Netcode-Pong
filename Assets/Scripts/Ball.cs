using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float startSpeed;
    [SerializeField] private float ballRadius;
    [SerializeField][Range(0, 1)] private float speedIncrease;
    [SerializeField] private float playerSize;
    [SerializeField] private float playerHitDeviation = 1f;

    private float p1GoalPos; //assumed left player
    private float p2GoalPos; //assumed right player

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckCollision();
    }

    private void Launch()
    {
        //determining which direction to launch the ball
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        rb.velocity = new Vector2(x,y).normalized * startSpeed;
    }

    //Method that checks for collisions since ball could be moving too fast for regular collision detections
    private void CheckCollision()
    {
        //setting up ray
        Vector2 startPos = transform.position;
        Vector2 direction = rb.velocity.normalized;
        float checkDistance = rb.velocity.magnitude * Time.fixedDeltaTime + ballRadius;
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, checkDistance);

        //checking for collisions
        if (hit)
        {
            //determining which bounce mechanism to call
            if (hit.collider.tag == "Player")
            {
                PlayerBounce(hit);
            }
            else
            {
                Bounce(hit);
            }
        }
    }

    //Method that performs a reflection of the ball given normal reflection conditions
    private void Bounce(RaycastHit2D hitInfo)
    {
        Vector2 surfaceNormal = hitInfo.normal;
        Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, surfaceNormal);
        rb.velocity = newDirection.normalized * rb.velocity.magnitude;
    }

    //Method that perform a reflection of the ball using player as randomizations
    private void PlayerBounce(RaycastHit2D hitInfo)
    {
        //getting the regular reflection to use as a base 
        Vector2 surfaceNormal = hitInfo.normal;
        Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, surfaceNormal);

        //make impact point affect the bounce
        float difference = (hitInfo.point.y - hitInfo.transform.position.y)/playerSize;
        if (hitInfo.point.y > hitInfo.transform.position.y)
        {
            newDirection.y += difference * playerHitDeviation;
        }
        else
        {
            newDirection.y -= difference * playerHitDeviation;
        }

        //increasing speed each player bounce to make constantly relfectinn the ball harder
        rb.velocity = newDirection.normalized * rb.velocity.magnitude * (1+speedIncrease);
    }
}
