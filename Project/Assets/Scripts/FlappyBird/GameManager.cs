using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public enum GameState 
    { 
        Ready = 0,
        Playing = 1,
        GameOver = 2,
    }

    public class GameManager : SingletonPersistent<GameManager>
    {
        public GameState State { get; private set; } = GameState.Ready;

        public void SetStartGame()
        {
            State = GameState.Playing;
            Bird.Instance.OnStartGame();
        }
    }
}