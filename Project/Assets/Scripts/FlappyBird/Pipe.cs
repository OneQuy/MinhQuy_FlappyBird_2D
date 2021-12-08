using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class Pipe : MonoBehaviour
    {
        // Variables

        [SerializeField]
        private GameObject obstacle_1 = null;
        
        [SerializeField]
        private GameObject obstacle_2 = null;

        [SerializeField]
        private float minLocalPosX = -0.8f;
        
        [SerializeField]
        private float maxLocalPosX = 0.5f;

        [SerializeField]
        private float maxLocalPosY_Obstacle_1 = 1.1f;
        
        [SerializeField]
        private float minLocalPosY_Obstacle_1 = 0.7f;
        
        [SerializeField]
        private float maxLocalPosY_Obstacle_2 = -0.5f;
        
        [SerializeField]
        private float minLocalPosY_Obstacle_2 = -1.2f;

        // Methods

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            // Auto hide the pipe when it fars from the bird

            if (transform.localPosition.x < Bird.Instance.transform.localPosition.x - 20)
                PipeManager.Instance.ReturnPipeToPool(gameObject);
        }

        // Calculate and create Orange obstacles 

        public void GenerateObstacle(bool forceNotCreateObstacle)
        {
            // Quantity number for obstacle will appear

            int quantity;

            if (forceNotCreateObstacle)
                quantity = 0;
            else
                quantity = Utilities.RandomBool ? 0 : (Utilities.RandomBool ? 1 : 2);

            // Random position

            if (quantity == 0)
            {
                obstacle_1.CheckAndActiveGo(false);
                obstacle_2.CheckAndActiveGo(false);
            }
            else if (quantity == 1)
            {
                obstacle_1.CheckAndActiveGo(true);
                obstacle_2.CheckAndActiveGo(false);

                var x = Random.Range(minLocalPosX, maxLocalPosX);
                var y = Random.Range(minLocalPosY_Obstacle_2, maxLocalPosY_Obstacle_1);
                obstacle_1.SetPos_X(x);
                obstacle_1.SetPos_Y(y);
            }
            else // 2
            {
                obstacle_1.CheckAndActiveGo(true);
                obstacle_2.CheckAndActiveGo(true);

                var x = Random.Range(minLocalPosX, maxLocalPosX);
                var y = Random.Range(minLocalPosY_Obstacle_1, maxLocalPosY_Obstacle_1);
                obstacle_1.SetPos_X(x);
                obstacle_1.SetPos_Y(y);
                
                x = Random.Range(minLocalPosX, maxLocalPosX);
                y = Random.Range(minLocalPosY_Obstacle_2, maxLocalPosY_Obstacle_2);
                obstacle_2.SetPos_X(x);
                obstacle_2.SetPos_Y(y);
            }
        }
    }
}