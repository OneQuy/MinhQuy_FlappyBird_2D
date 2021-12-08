using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class Bird : SingletonPersistent<Bird>
    {
        [SerializeField]
        private float jumpFactor = 2f;

        private new Rigidbody2D rigidbody = null;

        protected override void Awake()
        {
            base.Awake();

            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (Utilities.IsPressed_Space)
                Jump();
        }

        private void Jump()
        {
            rigidbody.velocity = Vector2.up * jumpFactor;
        }

        public void OnStartGame()
        {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}