using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

namespace FlappyBird
{
    public class ShootManager : SingletonPersistent<ShootManager>
    {
        // Variables

        public float bulletSpeed = 300f;

        [SerializeField]
        private GameObject prefab = null;
        
        private Pool<GameObject> firePool = null;

        // Methods

        protected override void Awake()
        {
            base.Awake();

            // Setup for first time

            GameManager.Instance.onInit += OnInit;            
            firePool = new Pool<GameObject>(true, prefab);
            firePool.OnBeforeReturn = go => go.CheckAndActiveGo(false);
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            // Shoot

            if (!GameManager.Instance.IsPhone && Utilities.IsPressed_Return)
                Shoot();
        }

        /// <summary>
        /// Hide all bullets when not playing
        /// </summary>
        public void OnInit()
        {
            firePool.ReturnAll();
        }

        /// <summary>
        /// Start shoot a bullet
        /// </summary>
        public void Shoot()
        {
            var bullet = firePool.Pick();
            bullet.transform.localPosition = Bird.Instance.transform.localPosition;
            bullet.CheckAndActiveGo(true);
        }

        public void ReturnPipeToPool(GameObject go)
        {
            firePool.Return(go);
        }
    }
}