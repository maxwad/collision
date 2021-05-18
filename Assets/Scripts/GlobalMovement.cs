using System;
using UnityEngine;



public class GlobalMovement : MonoBehaviour
{
    // other Force and direction
    public float force = 5.0f;
    private float directionForceRad = 135 * Mathf.PI / 180;

    public class Ball
    {
        // extension GameObject, additional necessary information about balls
        public string name;
        public Vector2 position;
        public Vector2 acceleration;
        public Vector2 movement;
        public float mass;
        public float radius;
        public float massMultiply = 4.0f;

        public Ball(GameObject obj)
        {
            name     = obj.name;
            radius   = obj.transform.localScale.x / 2;
            position = obj.transform.position;
            mass     = radius * massMultiply;
        }

    }

    private GameObject[] balloons;
    private Ball[] allBalls;

    public class Obstacle
    {
        // extension GameObject, additional necessary information about grounds
        public Vector2 position;
        public float groundAngle;
        public float cosGroundAngle;
        public float sinGroundAngle;
        public float scaleX;
        public float leftEdge;
        public float rightEdge;
        public Vector2 normal;

        public Obstacle(GameObject obj)
        {
            position       = obj.transform.position;
            groundAngle    = obj.transform.rotation.eulerAngles.z * Mathf.PI / 180; ;
            cosGroundAngle = Mathf.Cos(groundAngle);
            sinGroundAngle = Mathf.Sin(groundAngle);
            scaleX         = obj.transform.localScale.x;
            leftEdge       = obj.transform.position.x - (scaleX * cosGroundAngle) / 2;
            rightEdge      = obj.transform.position.x + (scaleX * cosGroundAngle) / 2;
            normal         = new Vector2(1 * sinGroundAngle, -1 * cosGroundAngle);
        }
    }

    private GameObject[] obstacles;
    private Obstacle[] allObstacles;


    public void CollisionOfBalloons()
    {
        for (int i = 0; i < allBalls.Length; i++)
        {
            for (int k = i; k < allBalls.Length; k++)
            {
                float distance = Vector2.Distance(allBalls[i].position, allBalls[k].position);
                if (i != k)
                {
                    if (distance <= allBalls[i].radius + allBalls[k].radius)
                    {
                        // Right way... Fifth try

                        float iComponentMass = (2 * allBalls[k].mass) / (allBalls[i].mass + allBalls[k].mass);
                        float kComponentMass = (2 * allBalls[i].mass) / (allBalls[i].mass + allBalls[k].mass);

                        Vector2 iBetweenCenter = allBalls[i].position - allBalls[k].position;
                        Vector2 kBetweenCenter = allBalls[k].position - allBalls[i].position;

                        float iDotProduct = Vector2.Dot((allBalls[i].movement - allBalls[k].movement), iBetweenCenter);
                        float kDotProduct = Vector2.Dot((allBalls[k].movement - allBalls[i].movement), kBetweenCenter);

                        Vector2 iFinalyMovement = allBalls[i].movement - iComponentMass * (iDotProduct / (iBetweenCenter.sqrMagnitude)) * iBetweenCenter;
                        Vector2 kFinalyMovement = allBalls[k].movement - kComponentMass * (kDotProduct / (kBetweenCenter.sqrMagnitude)) * kBetweenCenter;

                        allBalls[i].movement = iFinalyMovement;
                        allBalls[k].movement = kFinalyMovement;                        
                    }
                }
            }
        }
    }

    private Boolean IsGrounded(Ball pl, Obstacle gr)
    {
        bool isGrounded = false;
        float dX = gr.cosGroundAngle * (gr.scaleX / 2);
        float dY = gr.sinGroundAngle * (gr.scaleX / 2);
        Vector2 distance = new Vector2(dX, dY) + (Vector2)gr.position - (Vector2)pl.position;
        Vector2 projectionDistance = Vector2.Dot(distance, gr.normal) * gr.normal;

        if (projectionDistance.magnitude <= pl.radius)
            isGrounded = true;

        return isGrounded;
    }

    void Start()
    {
        Vector2 gravity = new Vector2(0, -10);

        // GameObjects before start
        balloons = GameObject.FindGameObjectsWithTag("Player");
        allBalls = new Ball[balloons.Length];

        for (int i = 0; i < balloons.Length; i++)
        {
            allBalls[i] = new Ball(balloons[i]);
            allBalls[i].acceleration = new Vector2(
            (force / allBalls[i].mass) * Mathf.Cos(directionForceRad),
            (force / allBalls[i].mass) * Mathf.Sin(directionForceRad) + gravity.y
            );
            allBalls[i].movement = allBalls[i].acceleration;
        }             

        obstacles = GameObject.FindGameObjectsWithTag("Ground");
        allObstacles = new Obstacle[obstacles.Length];

        for (int j = 0; j < obstacles.Length; j++)
        {
            allObstacles[j] = new Obstacle(obstacles[j]);
        }    
    }


    private void FixedUpdate()
    {
        CollisionOfBalloons();
        
        for (int i = 0; i < allBalls.Length; i++)
        {
            Vector2 movementSpeed;
            Vector2 delta;
            var player = allBalls[i];

            // speed and distance
            movementSpeed = player.movement;
            movementSpeed   += player.acceleration * (Time.deltaTime * 2);
            delta           = movementSpeed * Time.deltaTime;
            player.position += delta;
            player.movement = movementSpeed;

            for (int j = 0; j < balloons.Length; j++)
            {
                if (balloons[j].name == player.name)
                    balloons[j].transform.position += (Vector3)delta;
            }

            for (int j = 0; j < allObstacles.Length; j++)
            {
                var ground = allObstacles[j];

                // check collision and change direction
                if ((player.position.x > (ground.leftEdge - player.radius)) &&
                (player.position.x < (ground.rightEdge + player.radius)) &&
                IsGrounded(player, ground))
                    player.movement -= 2 * (Vector2.Dot(movementSpeed, ground.normal) * ground.normal);
            }
        }        
    }
}