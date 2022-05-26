using System;
using System.Collections;
using DG.Tweening;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class MapManager : MonoBehaviour
    {
        // public GameObject CloudRoot;
        public Slider Slider;
        // public GameObject TileRoot;
        public Camera TileCamera;

        private float _cacheSliderValue = 0;
        private UnityHttpServer _server;

        public Camera CloudCamera;
        // public TileRootDragController TileRootDragController;

        private void Start()
        {
            GlobalVariable.DragLock = true;
            _cacheSliderValue = 0;
            _server = UnityHttpServer.Instance;
            if (_server != null) _server.ReceiveEnergyListener += ReceiveEnergy;
        }

        private void ReceiveEnergy(int energy)
        {
            GlobalVariable.Energy = energy;
            DispearCloud();
            GlobalVariable.DragLock = false;
            Debug.Log($"recv:{energy}");
        }

        private void OnDestroy()
        {
            if (_server?.ReceiveEnergyListener != null) _server.ReceiveEnergyListener -= ReceiveEnergy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // StartCoroutine(DispearCloud());
                DispearCloud();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                CloudController.GetInstance().Assemble();
            }

            if (Math.Abs(_cacheSliderValue - Slider.value) > 0.1)
            {
                
                _cacheSliderValue = Slider.value;
                // TileRoot.transform.localScale = new Vector2(_cacheSliderValue, _cacheSliderValue);
                TileCamera.orthographicSize = _cacheSliderValue;
            }
        }

        private void DispearCloud()
        {
            // foreach (var sprite in CloudRoot.GetComponentsInChildren<SpriteRenderer>())
            // {
            //     if (sprite.transform.localPosition.x < 0)
            //     {
            //         sprite.transform.DOLocalMoveX(-16, 2);
            //     }
            //     else
            //     {
            //         sprite.transform.DOLocalMoveX(16, 2);
            //     }
            //     sprite.DOFade(0, 2);
            // }
            CloudController.GetInstance().Disspear();

            // yield return 0;
        }
    }
}