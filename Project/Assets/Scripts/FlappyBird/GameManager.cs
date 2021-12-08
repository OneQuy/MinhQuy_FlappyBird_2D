using System;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public enum GameState 
    { 
        Ready = 0, // Wait for playing
        Playing = 1, // Playing!
        GameOver = 2, // When game over
    }

    public class GameManager : SingletonPersistent<GameManager>
    {
        // Variables

        public GameState State { get; private set; } = GameState.Ready;
        public int Score { get; private set; } = 0;

        // Gameplay Callbacks

        public Action onInit = null;
        public Action onStartGame = null;
        public Action onGameOver = null;

        // Methods

        private void Start()
        {
            SetInit();
        }

        /// <summary>
        /// Check if running on device or not
        /// </summary>
        public bool IsPhone
        {
            get 
            {
                return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            }
        }

        /// <summary>
        /// Save info for highest score
        /// </summary>
        public int HighestScore
        {
            get 
            {
                return PlayerPrefs.GetInt("HighestScore", 0);
            }
            
            set 
            {
                PlayerPrefs.SetInt("HighestScore", value);
            }
        }

        /// <summary>
        /// When start the game or replay
        /// </summary>
        public void SetInit()
        {
            State = GameState.Ready;
            Score = 0;
            onInit.SafeCall();
        }

        /// <summary>
        /// Start control the bird and play game!
        /// </summary>
        public void SetStartGame()
        {
            State = GameState.Playing;
            onStartGame.SafeCall();
        }

        /// <summary>
        /// Passed a pipe
        /// </summary>
        public void SetPassedPipe()
        {
            Score++;

            if (Score > HighestScore)
                HighestScore = Score;

            UIManager.Instance.UpdatePlayingScoreText();
        }

        /// <summary>
        /// Game over setups
        /// </summary>
        public void SetGameOver()
        {
            State = GameState.GameOver;
            onGameOver.SafeCall();
        }
    }
}