using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float startSpeed;
    [SerializeField] private float ballRadius;
    [SerializeField][Range(0, 1)] private float speedIncrease;
    [SerializeField] private float playerRadius;
    [SerializeField] private float playerHitDeviation = 1f;
    [SerializeField] private LayerMask collisionDetectionMask;

    private float p1GoalPos; //assumed left player
    private float p2GoalPos; //assumed right player
    private NetworkVariable<Vector2> velocity = new NetworkVariable<Vector2>(Vector2.zero);

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        UpdatePosition();
    }

    private void Launch()
    {
        if (!IsOwner) return;

        //determining which direction to launch the ball
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        velocity.Value = new Vector2(x,y).normalized * startSpeed;
    }

    //Method that checks for collisions since ball could be moving too fast for regular collision detections
    private void CheckCollision()
    {
        //setting up ray
        Vector2 startPos = transform.position;
        Vector2 direction = velocity.Value.normalized;
        float checkDistance = velocity.Value.magnitude * Time.deltaTime + ballRadius;
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, checkDistance, collisionDetectionMask);
        //Debug.DrawLine(startPos, startPos + direction * checkDistance, Color.red);

        //checking for collisions
        if (hit)
        {
            //determining which bounce mechanism to call
            if (hit.collider.CompareTag("Player"))
            {
                PlayerBounce(hit);
            }
            else if (hit.collider.CompareTag("Goal"))
            {
                bool hostGoal = false;

                if (transform.position.x < 0)
                {
                    hostGoal = true;
                }

                GameManager.instance.Score(hostGoal);
                Destroy(gameObject);
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
        Vector2 newDirection = Vector2.Reflect(velocity.Value.normalized, surfaceNormal);
        velocity.Value = newDirection.normalized * velocity.Value.magnitude;
    }

    //Method that perform a reflection of the ball using player as randomizations
    private void PlayerBounce(RaycastHit2D hitInfo)
    {
        //getting the regular reflection to use as a base 
        Vector2 surfaceNormal = hitInfo.normal;
        Vector2 newDirection = Vector2.Reflect(velocity.Value.normalized, surfaceNormal);

        //make impact point affect the bounce
        float difference = (hitInfo.point.y - hitInfo.transform.position.y)/playerRadius;
        if (hitInfo.point.y > hitInfo.transform.position.y)
        {
            newDirection.y += difference * playerHitDeviation;
        }
        else
        {
            newDirection.y -= difference * playerHitDeviation;
        }

        //increasing speed each player bounce to make constantly relfectinn the ball harder
        velocity.Value = newDirection.normalized * velocity.Value.magnitude * (1+speedIncrease);
    }

    //Method that updates the position of the ball
    private void UpdatePosition()
    {
        transform.position += (Vector3)velocity.Value * Time.deltaTime;
    }
}