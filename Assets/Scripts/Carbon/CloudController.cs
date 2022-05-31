using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ET;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Carbon
{
    public class CloudController : MonoBehaviour
    {
        private static CloudController Instance;
        public GameObject SolidCloud;
        private Image[] _solidClouds;
        private Dictionary<Image, float> _initCloudPos = new Dictionary<Image, float>();
        public Canvas TranslucentCanvas;

        public Image[] _translucentClouds;
        
        // public Camera cloudCamera;
        private Random _random;
        
        public static CloudController GetInstance()
        {
            return Instance;
        }
        
        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _random = new Random();
            // ShowTranlucentCloud(cloudCamera);
        }
        
        private void Start()
        {
            _solidClouds = SolidCloud.GetComponentsInChildren<Image>();
            foreach (var cloud in _solidClouds)
            {
                _initCloudPos.Add(cloud, cloud.transform.localPosition.x);
            }
            _translucentClouds = TranslucentCanvas.GetComponentsInChildren<Image>();
            Disspear();
        }

        public void ShowTranslucentCloud(Camera cloudCamera)
        {
            TranslucentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            TranslucentCanvas.worldCamera = cloudCamera;

            foreach (var image in _translucentClouds)
            {
                image.DOFade(0.8f, 3);

                image.transform.DOLocalMoveX(image.transform.localPosition.x + _random.Next(-200, 200), 1.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void HideTranslucentCloud()
        {
            foreach (var cloud in _translucentClouds)
            {
                cloud.DOFade(0, 3);
            }
        }

        public void Disspear()
        {
            foreach (var cloud in _solidClouds)
            {
                if (cloud.transform.localPosition.x < 0)
                {
                    cloud.transform.DOLocalMoveX(-Screen.width / 2 - 1000, 2);
                }
                else
                {
                    cloud.transform.DOLocalMoveX(Screen.width / 2 + 1000, 2);
                }
            }
        }

        public void Assemble(Action action = null)
        {
            StartCoroutine(RealAssemble(action));
        }

        private IEnumerator RealAssemble(Action action)
        {
            float time = 2;
            foreach (var cloud in _solidClouds)
            {
                cloud.transform.DOLocalMoveX(_initCloudPos[cloud], time);
            }
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

    }
}