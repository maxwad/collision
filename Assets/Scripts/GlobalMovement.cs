using System;
using UnityEngine;



public class GlobalMovement : MonoBehaviour
{

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

    protected static Ball[] allBalls;
    protected static GameObject[] balloons;

    public void CollisionOfBalloons()
    {
        for (int i = 0; i < allBalls.Length; i++)
        {
            for (int j = i; j < allBalls.Length; j++)
            {
                float distance = Vector2.Distance(allBalls[i].position, allBalls[j].position);
                if (i != j)
                {
                    if (distance <= allBalls[i].radius + allBalls[j].radius)
                    {
                        // Right way... Fifth try

                        float iComponentMass = (2 * allBalls[j].mass) / (allBalls[i].mass + allBalls[j].mass);
                        float jComponentMass = (2 * allBalls[i].mass) / (allBalls[i].mass + allBalls[j].mass);

                        Vector2 iBetweenCenter = allBalls[i].position - allBalls[j].position;
                        Vector2 jBetweenCenter = allBalls[j].position - allBalls[i].position;

                        float iDotProduct = Vector2.Dot((allBalls[i].movement - allBalls[j].movement), iBetweenCenter);
                        float jDotProduct = Vector2.Dot((allBalls[j].movement - allBalls[i].movement), jBetweenCenter);

                        Vector2 iFinalyMovement = allBalls[i].movement - iComponentMass * (iDotProduct / (iBetweenCenter.sqrMagnitude)) * iBetweenCenter;
                        Vector2 jFinalyMovement = allBalls[j].movement - jComponentMass * (jDotProduct / (jBetweenCenter.sqrMagnitude)) * jBetweenCenter;

                        allBalls[i].movement = iFinalyMovement;
                        allBalls[j].movement = jFinalyMovement;                        
                    }
                }
            }
        }
    }

    void Awake()
    {        
        balloons = GameObject.FindGameObjectsWithTag("Player");
        allBalls = new Ball[balloons.Length];
    }

    private void FixedUpdate()
    {
        CollisionOfBalloons();
    }
}