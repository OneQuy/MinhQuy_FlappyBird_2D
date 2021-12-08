using UnityEngine;
using UnityEngine.UI;
using SteveRogers;

namespace FlappyBird
{
    public class UIManager : SingletonPersistent<UIManager>
    {
        // Variables

        [SerializeField]
        private GameObject canvasGo = null;

        [SerializeField]
        private GameObject startMenuGo = null;

        [SerializeField]
        private GameObject playingMenuGo = null;

        [SerializeField]
        private GameObject gameOverMenuGo = null;

        [SerializeField]
        private GameObject jumpButtonGo = null;
        
        [SerializeField]
        private GameObject shootButtonGo = null;

        [SerializeField]
        private Text scoreText = null;

        [SerializeField]
        private Text highscoreText = null;

        [SerializeField]
        private Text playingScoreText = null;

        // Methods

        protected override void Awake()
        {
            // Register callbacks

            base.Awake();
            GameManager.Instance.onInit += OnInit;
            GameManager.Instance.onStartGame += OnStartGame;
            GameManager.Instance.onGameOver += OnGameOver;

            // If not play on devices, hide the buttons controller

            if (!GameManager.Instance.IsPhone)
            {
                jumpButtonGo.SetActive(false);
                shootButtonGo.SetActive(false);
            }
        }

        /// <summary>
        /// Handle menu when not playing
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.State == GameState.Playing)
                return;

            // Press space for quick enter the game

            if (Utilities.IsPressed_Space)
            {
                if (GameManager.Instance.State == GameState.Ready)
                    OnPressed_StartGame();
                else if (GameManager.Instance.State == GameState.GameOver)
                    OnPressed_Replay();
            }
        }

        /// <summary>
        ///  Update score text
        /// </summary>
        public void UpdatePlayingScoreText()
        {
            if (GameManager.IsInstanced)
                playingScoreText.text = "Score: " + GameManager.Instance.Score;
        }

        /// <summary>
        /// Setup menu when enter the game or replay
        /// </summary>
        public void OnInit()
        {
            startMenuGo.CheckAndActiveGo(true);
            gameOverMenuGo.CheckAndActiveGo(false);
            playingMenuGo.CheckAndActiveGo(false);
            UpdatePlayingScoreText();
        }

        /// <summary>
        /// Setup menu for starting play
        /// </summary>
        public void OnStartGame()
        {
            startMenuGo.CheckAndActiveGo(false);
            gameOverMenuGo.CheckAndActiveGo(false);
            playingMenuGo.CheckAndActiveGo(true);
        }

        /// <summary>
        /// Setup menu for game over
        /// </summary>
        public void OnGameOver()
        {
            startMenuGo.CheckAndActiveGo(false);
            gameOverMenuGo.CheckAndActiveGo(true);
            playingMenuGo.CheckAndActiveGo(false);

            highscoreText.text = "Highest Score: " + GameManager.Instance.HighestScore;
            scoreText.text = "Score: " + GameManager.Instance.Score;
        }

        public void OnPressed_StartGame()
        {
            GameManager.Instance.SetStartGame();
        }

        public void OnPressed_Shoot()
        {
            ShootManager.Instance.Shoot();
        }
        
        public void OnPressed_Jump()
        {
            Bird.Instance.Jump();
        }

        public void OnPressed_Replay()
        {
            GameManager.Instance.SetInit();
        }
    }
}