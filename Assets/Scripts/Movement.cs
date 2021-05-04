using System;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Transform player;
    public Transform ground;

    public float mass = 1;    

    // other Force and direction
    private float force = 5f;
    private float directionForceRad = 45 * (float)Math.PI / 180;
    private Vector2 acceleration;
    private Vector2 movementSpeed;
    private Vector2 delta;
    private Vector2 normal;

    // parameters of ground
    private float leftEdge;
    private float rightEdge;
    private float groundAngle;
    private float cosGroundAngle;
    private float sinGroundAngle;

    private Boolean IsGrounded()
    {
        bool isGrounded = false;
                
        float dX = cosGroundAngle * (ground.localScale.x / 2);
        float dY = sinGroundAngle * (ground.localScale.x / 2);
        Vector2 distance = new Vector2(dX, dY) + (Vector2)ground.position - (Vector2)player.position;
        Vector2 projectionDistance = Vector2.Dot(distance, normal) * normal;

        if (projectionDistance.magnitude - 0.1 <= player.localScale.y / 2)
        {            
            isGrounded = true;
        }
        return isGrounded;
    }

    void Start()
    {
        float gravity = 10;
        float angleGravityRad = -90 * (float)Math.PI / 180;

        groundAngle    = ground.transform.rotation.eulerAngles.z * (float)Math.PI / 180;
        cosGroundAngle = (float)Math.Cos(groundAngle);
        sinGroundAngle = (float)Math.Sin(groundAngle);
        leftEdge       = ground.position.x - (ground.localScale.x * cosGroundAngle) / 2;
        rightEdge      = ground.position.x + (ground.localScale.x * cosGroundAngle) / 2;
        normal         = new Vector2(1 * sinGroundAngle, -1 * cosGroundAngle);

        // Components before start
        acceleration = new Vector2(
            (force / mass) * (float)Math.Cos(directionForceRad), 
            gravity * (float)Math.Sin(angleGravityRad) + (force / mass) * (float)Math.Sin(directionForceRad)
            );
    }


    void FixedUpdate()
    {       
        // speed and distance (divide by 5 for fractionality)
        movementSpeed   += acceleration * Time.deltaTime/5;
        delta           = movementSpeed * Time.deltaTime;        
        player.position = (Vector2)player.position + delta;

        // check collision and change direction
        if ((player.position.x > (leftEdge - player.localScale.x / 2)) && 
            (player.position.x < (rightEdge + player.localScale.x / 2)) &&
            IsGrounded())
        {           
             movementSpeed -= 2 * (Vector2.Dot(movementSpeed, normal) * normal); 
        }
    }
}