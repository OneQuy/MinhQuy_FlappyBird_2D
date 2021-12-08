using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class PipeManager : SingletonPersistent<PipeManager>
    {
        // Variables

        [SerializeField]
        private float maxPositionY = 2.57f;

        [SerializeField]
        private float minPositionY = -2.57f;

        [SerializeField]
        private float maxDistanceX = 9;

        [SerializeField]
        private float minDistanceX = 6;

        [SerializeField]
        private GameObject prefabPipe = null;

        private Vector3 startPipeLocalPosition = Vector3.zero;
        private Vector3 lastPipeLocalPosition = Vector3.zero;
        private Pool<GameObject> pilePool = null; // Use objects pool for pipes

        // Methods

        protected override void Awake()
        {
            base.Awake();

            // Setups for first time

            GameManager.Instance.onInit += OnInit;
            startPipeLocalPosition = prefabPipe.transform.localPosition;
            pilePool = new Pool<GameObject>(true, prefabPipe);
            pilePool.OnBeforeReturn = go => go.CheckAndActiveGo(false);
        }

        /// <summary>
        /// Auto create the pipes when playing
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (lastPipeLocalPosition.x < Bird.Instance.transform.localPosition.x + 20)
                Create();
        }

        /// <summary>
        /// Create another pipe from the last pipe info
        /// </summary>
        private void Create()
        {
            var x = lastPipeLocalPosition.x + UnityEngine.Random.Range(minDistanceX, maxDistanceX);
            var y = Random.Range(minPositionY, maxPositionY);
            Create(new Vector3(x, y), false);
        }

        /// <summary>
        /// Create the pipe with the specific position
        /// </summary>
        private void Create(Vector3 localPosition, bool forceNotCreateObstacle)
        {
            var pile = pilePool.Pick();
            pile.transform.localPosition = localPosition;
            pile.CheckAndActiveGo(true);
            lastPipeLocalPosition = localPosition;
            pile.GetComponent<Pipe>().GenerateObstacle(forceNotCreateObstacle);
        }

        /// <summary>
        /// Create first pipe when start the game
        /// </summary>
        public void OnInit()
        {
            pilePool.ReturnAll();
            Create(startPipeLocalPosition, true);
        }

        public void ReturnPipeToPool(GameObject go)
        {
            pilePool.Return(go);
        }
    }
}