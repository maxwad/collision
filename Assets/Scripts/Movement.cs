using System;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Transform player;
    public Transform ground;

    public float mass = 1;
    private float depthGround;
    private float groundAngle;
    private float groundZero;

    // gravity
    private float gravity = 10;
    private float angleGravity_Rad = -90 * (float)Math.PI / 180;

    // other Force and direction
    private float Force = 3f;
    private float directionForce_Rad = 225 * (float)Math.PI / 180;

    private float accelerationX;
    private float accelerationY;

    private float movementSpeedX;
    private float movementSpeedY;

    // edges of ground
    private float leftEdge;
    private float rightEdge;
    private float topEdge;


    void Start()
    {
        System.Random random = new System.Random();

        //player.position = new Vector2(random.Next(-5, 6), random.Next(1, 7));
        //player.position = new Vector2(8, 8);
        groundAngle = ground.transform.rotation.eulerAngles.z * (float)Math.PI / 180;

        // depthGround - толщина нашего ground; нужна, чтобы объекты ударялись о поверхность, а не о центр ground
        // h           - промежуточная величина, необходимая для определения абсолютного нулевого уровня ground
        // groundZero  - абсолютный нулевой уровень, самая нижняя координата всей системы
        depthGround = ground.localScale.y / (float)Math.Cos(groundAngle);        
        float h = (float)Math.Sqrt((float)Math.Pow(ground.localScale.x/2,2) - (float)Math.Pow(ground.localScale.x /2 * (float)Math.Cos(groundAngle),2));
        groundZero = ground.position.y - h;     


        leftEdge  = ground.position.x - (ground.localScale.x * (float)Math.Cos(groundAngle)) / 2;
        rightEdge = ground.position.x + (ground.localScale.x * (float)Math.Cos(groundAngle)) / 2;
        topEdge   = ground.position.y + (ground.localScale.y * (float)Math.Cos(groundAngle)) / 2;

        // Components before start
        accelerationX = (Force / mass) * (float)Math.Cos(directionForce_Rad);
        accelerationY = gravity * (float)Math.Sin(angleGravity_Rad) + (Force / mass) * (float)Math.Sin(directionForce_Rad);

    }

    
    void FixedUpdate()
    {

        groundAngle = ground.transform.rotation.eulerAngles.z * (float)Math.PI / 180;
        // speed and distance (divide by 2 for fractionality)
        movementSpeedX += accelerationX * Time.deltaTime;
        movementSpeedY += accelerationY * Time.deltaTime;

        float deltaX = movementSpeedX * Time.deltaTime;
        float deltaY = movementSpeedY * Time.deltaTime;

        
        player.position = (Vector2)player.position + new Vector2(deltaX, deltaY);

        // определение координат столкновения
        float collisionX = player.position.x - leftEdge;
        float collisionY = groundZero + collisionX * (float)Math.Tan(groundAngle) + depthGround / 2;

        // check collision and change direction
        if ((player.position.x > (leftEdge - player.localScale.x / 2)) && (player.position.x < (rightEdge + player.localScale.x / 2)))
        {
            
            if ((player.position.y - player.localScale.y / 2) <= collisionY)
            {
                // newVector = oldVector - 2 * ((oldVector*n) * n)
                // oldVector = (movementSpeedX, movementSpeedY)
                // n = (0, -1)
                // newVector = (X, Y) - 2* (((X,Y)*(0, -1) * (0, -1)))
                
                Vector2 old = new Vector2(movementSpeedX, movementSpeedY);
                Vector2 normal = new Vector2(1 * (float)Math.Sin(groundAngle), -1 * (float)Math.Cos(groundAngle));
                Vector2 newVector = old - 2 * (Vector2.Dot(old, normal) * normal);
                movementSpeedY = newVector.y;
                movementSpeedX = newVector.x;

                Debug.Log("");
                
            }
                        

        }
     
    }
}
