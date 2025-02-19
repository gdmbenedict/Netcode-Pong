using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private float moveforce;
    [SerializeField] private float maxSpeed;
    [SerializeField][Range(0,1)] private float velocityDecay;
    [SerializeField] private Rigidbody2D rb;
    private float forceDirection;
    private float minSpeed = 0.0001f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(forceDirection);
        HandleVelocity();
    }

    private void HandleVelocity()
    {
        //check if paddle is moving at max speed
        if (rb.velocity.magnitude < maxSpeed)
        {
            //calculate force & add to paddle
            Vector2 force = Vector2.up * forceDirection * moveforce * Time.deltaTime;
            rb.AddForce(force, ForceMode2D.Force);
        }
        
        //make velocity decay
        rb.velocity *= (1 - velocityDecay);

        //clamp to zero if under min speed
        if (rb.velocity.magnitude < minSpeed && rb.velocity.magnitude > 0f)
        {
            rb.velocity = Vector3.zero;
        }
    }

    //Method that updates the current force direction
    public void OnMove(InputValue input)
    {
        Debug.Log("move input called");
        forceDirection = input.Get<float>();
    }

}
