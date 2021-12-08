using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System;
//using System.Collections.Generic;

namespace SteveRogers
{
    public class Twinkle : MonoBehaviour
    {
        [SerializeField]
        private float interval = 0.1f;

        private float cur = 0;

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private GameObject go = null;

        public void DisableButTurnCanvasOn()
        {
            enabled = false;

            if (canvas)
                canvas.enabled = true;
            else
                go.CheckAndActiveGo(true);
        }

        private void Update()
        {
            cur += Time.deltaTime;

            if (cur > interval)
            {
                cur = 0;

                if (canvas)
                    canvas.enabled = !canvas.enabled;
                else
                    go.CheckAndActiveGo(!go.activeSelf);
            }
        }
    }
}