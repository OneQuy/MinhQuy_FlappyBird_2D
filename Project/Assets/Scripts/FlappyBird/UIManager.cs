using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class UIManager : SingletonPersistent<UIManager>
    {
        public void OnPressed_StartGame()
        {
            GameManager.Instance.SetStartGame();
        }
    }
}