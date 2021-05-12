using System;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Transform player;
    public Transform ground;

    public float mass;    

    // other Force and direction
    private float force = 3f;
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


    private void DrawNormal()
    {
        Vector2 deltaVector = new Vector2(cosGroundAngle * (ground.localScale.x), sinGroundAngle * (ground.localScale.x)) / 2;
        Vector2 groundLevel = (Vector2)ground.position - deltaVector;
        Vector2 poitOfCollision = new Vector2(player.position.x - groundLevel.x, (player.position.x - groundLevel.x) * (sinGroundAngle / cosGroundAngle));
        Vector2 giz = new Vector2(player.position.x, groundLevel.y + poitOfCollision.y);

        Debug.DrawLine(giz, giz - normal, Color.green);
    }



    // +++++++++++++++++++++++++
    // +++++++++++++++++++++++++
    public class Ball
    {
        public string name;
        public Vector2 movement;
        public Vector2 position;
        public float mass;
        public float radius;       

        public Ball(GameObject obj, Vector2 mov, float weigth)
        {
            name     = obj.name;
            radius   = obj.transform.localScale.x / 2;
            position = obj.transform.position;
            movement = mov;
            mass     = weigth;
        }

    }

    public static Ball[] allBalls;

    public static GameObject[] balloons;

    private void CollisionOfBalloons()
    {
        Vector2 first, second;
        for (int i = 0; i < allBalls.Length; i++)
        {            
            for (int j = i; j < allBalls.Length; j++)
            {
                if (i != j)
                {
                    if (Vector2.Distance(allBalls[i].position, allBalls[j].position) <= allBalls[i].radius + allBalls[j].radius)
                    {
                        //Debug.Log("allBalls[i].movement" + allBalls[i].movement);
                        //Debug.Log("allBalls[j].movement" + allBalls[j].movement);                        
                        first = (allBalls[j].mass * allBalls[j].movement) / allBalls[i].mass;
                        second = (allBalls[i].mass * allBalls[i].movement) / allBalls[j].mass;
                        allBalls[i].movement = first;
                        allBalls[j].movement = second;                        
                        //Debug.Log("!!!!!!  " + i + " --- " + j);
                        //Debug.Log("allBalls[i].movement" + allBalls[i].movement);
                        //Debug.Log("allBalls[j].movement" + allBalls[j].movement);
                    }
                }
            }           
        }
    }

    // +++++++++++++++++++++++++
    // +++++++++++++++++++++++++

    void Start()
    {
        Vector2 gravity = new Vector2(0, -10);

        groundAngle    = ground.transform.rotation.eulerAngles.z * (float)Math.PI / 180;
        cosGroundAngle = (float)Math.Cos(groundAngle);
        sinGroundAngle = (float)Math.Sin(groundAngle);
        leftEdge       = ground.position.x - (ground.localScale.x * cosGroundAngle) / 2;
        rightEdge      = ground.position.x + (ground.localScale.x * cosGroundAngle) / 2;
        normal         = new Vector2(1 * sinGroundAngle, -1 * cosGroundAngle);

        // Components before start
        acceleration = new Vector2(
            (force / mass) * (float)Math.Cos(directionForceRad), 
            (force / mass) * (float)Math.Sin(directionForceRad) + gravity.y
            );


        // +++++++++++++++++++++++++
        // +++++++++++++++++++++++++

        balloons = GameObject.FindGameObjectsWithTag("Player");
        allBalls = new Ball[balloons.Length];
        foreach (var item in balloons)
        {
            int ind = Array.IndexOf(balloons, item);

            allBalls[ind] = new Ball(balloons[ind], acceleration, mass);
            if (item.name == player.name)
            {
                allBalls[ind].mass = mass;
            }
        }
        for (int i = 0; i < allBalls.Length; i++)
        {
            Debug.Log("index = " + i + "; name  = " + allBalls[i].name  + "; mass  = " + allBalls[i].mass + " (player.name = " + player.name + ")");
        }
        // +++++++++++++++++++++++++
        // +++++++++++++++++++++++++
    }


    void FixedUpdate()
    {
        foreach (var item in allBalls)
        {
            if (item.name == player.name)            
                movementSpeed = item.movement;            
        }

        // speed and distance
        movementSpeed   += acceleration * Time.deltaTime;
        delta           = movementSpeed * Time.deltaTime;        
        player.position = (Vector2)player.position + delta;


        // +++++++++++++++++++++++++
        // +++++++++++++++++++++++++

        foreach (var item in allBalls)
        {
            if (item.name == player.name)
            {
                item.position = player.position;
                item.movement = movementSpeed;
            }
                                      
        }
      

        CollisionOfBalloons();

        // +++++++++++++++++++++++++
        // +++++++++++++++++++++++++


        //DrawNormal();                     

        // check collision and change direction
        if ((player.position.x > (leftEdge - player.localScale.x / 2)) && 
            (player.position.x < (rightEdge + player.localScale.x / 2)) &&
            IsGrounded())
        {
            foreach (var item in allBalls)
            {
                if (item.name == player.name)
                {
                    item.movement -= 2 * (Vector2.Dot(movementSpeed, normal) * normal);
                }

            }
            //movementSpeed -= 2 * (Vector2.Dot(movementSpeed, normal) * normal); 
        }
    }
}