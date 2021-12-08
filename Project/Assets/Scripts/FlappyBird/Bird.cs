using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class Bird : SingletonPersistent<Bird>
    {
        // Variables

        [SerializeField]
        private float jumpFactor = 20f;

        [SerializeField]
        private float moveSpeed = 20f;

        [SerializeField]
        private GameObject lastDieBird = null;

        private new Rigidbody2D rigidbody = null;

        // Methods

        protected override void Awake()
        {
            base.Awake();

            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody2D>();

            // Register the callbacks

            GameManager.Instance.onInit += OnInit;
            GameManager.Instance.onStartGame += OnStartGame;
            GameManager.Instance.onGameOver += OnGameOver;
        }

        /// <summary>
        /// Highest Position X for the last best record die (for showing the ghost)
        /// </summary>
        private float HighestPositionX
        {
            get
            {
                return PlayerPrefs.GetFloat("HighestPositionX", -1);
            }

            set
            {
                PlayerPrefs.SetFloat("HighestPositionX", value);
            }
        }

        /// <summary>
        /// Highest Position Y for the last best record die (for showing the ghost)
        /// </summary>
        private float HighestPositionY
        {
            get
            {
                return PlayerPrefs.GetFloat("HighestPositionY", -1);
            }

            set
            {
                PlayerPrefs.SetFloat("HighestPositionY", value);
            }
        }

        /// <summary>
        /// Handle gameplay
        /// </summary>
        private void Update()
        {
            // Not need to handle if not playing

            if (GameManager.Instance.State != GameState.Playing)
                return;

            // Press space or tap screen by mouse if play on PC / Mac / Unity editor

            if (!GameManager.Instance.IsPhone && (Utilities.IsPressed_Space || Input.GetMouseButtonDown(0)))
                Jump();

            // Move the bird forward
            
            gameObject.SetPos_X(transform.localPosition.x + moveSpeed * Time.deltaTime);

            // Check the limit bird high

            if (transform.localPosition.y >= 5)
            {
                gameObject.SetPos_Y(5);
                rigidbody.velocity = Vector2.zero;
            }
            else if (transform.localPosition.y <= -5)
                gameObject.SetPos_Y(-5);
        }

        /// <summary>
        /// Handle when hit something
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Pipe") ||
                collision.tag.Equals("Obstacle")) // Hit obstacles
            {
                GameManager.Instance.SetGameOver();
            }
            else if (collision.tag.Equals("PassPipe")) // Passed a pipe
            {
                GameManager.Instance.SetPassedPipe();
            }
        }

        /// <summary>
        /// Jump the bird
        /// </summary>
        public void Jump()
        {
            rigidbody.velocity = Vector2.up * jumpFactor;
        }

        /// <summary>
        /// Init the bird when enter the app or replay
        /// </summary>
        public void OnInit()
        {
            // Set start position

            transform.localPosition = new Vector3(-3.738f, 0);

            // Set best record ghost bird

            if (HighestPositionX >= 0)
            {
                lastDieBird.SetPos_X(HighestPositionX);
                lastDieBird.SetPos_Y(HighestPositionY);
                lastDieBird.CheckAndActiveGo(true);
            }
            else // First time play not need to show
                lastDieBird.CheckAndActiveGo(false);
        }

        /// <summary>
        /// Start play!
        /// </summary>
        public void OnStartGame()
        {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        /// <summary>
        /// Handle the bird when game over
        /// </summary>
        public void OnGameOver()
        {
            // Not need to move more

            rigidbody.bodyType = RigidbodyType2D.Static;

            // Check for saving best record

            if (transform.localPosition.x > HighestPositionX)
            {
                HighestPositionX = transform.localPosition.x;
                HighestPositionY = transform.localPosition.y;
            }
        }
    }
}