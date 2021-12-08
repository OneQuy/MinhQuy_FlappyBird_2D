using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class Bullet : MonoBehaviour
    {
        // Methods

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            // Move the bullet forward

            gameObject.SetPos_X(transform.localPosition.x + ShootManager.Instance.bulletSpeed * Time.deltaTime);

            // If the position fars from the bird will auto disappear

            if (Mathf.Abs(transform.localPosition.x - Bird.Instance.transform.localPosition.x) > 20)
                ShootManager.Instance.ReturnPipeToPool(gameObject);
        }

        /// <summary>
        /// Handle if bullet hit the obstacle and hit them
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Obstacle"))
            {
                collision.gameObject.CheckAndActiveGo(false);
                ShootManager.Instance.ReturnPipeToPool(gameObject);
            }
        }
    }
}