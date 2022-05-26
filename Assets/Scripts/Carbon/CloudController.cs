using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ET;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class CloudController : MonoBehaviour
    {
        private static CloudController Instance;
        public GameObject SolidCloud;
        private Image[] _solidClouds;
        private Dictionary<Image, float> _initCloudPos = new Dictionary<Image, float>();

        public static CloudController GetInstance()
        {
            return Instance;
        }
        
        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            _solidClouds = SolidCloud.GetComponentsInChildren<Image>();
            foreach (var cloud in _solidClouds)
            {
                _initCloudPos.Add(cloud, cloud.transform.localPosition.x);
            }
            Disspear();
        }

        public void Disspear()
        {
            foreach (var cloud in _solidClouds)
            {
                if (cloud.transform.localPosition.x < 0)
                {
                    cloud.transform.DOLocalMoveX(-Screen.width / 2 - 400, 2);
                }
                else
                {
                    cloud.transform.DOLocalMoveX(Screen.width / 2 + 400, 2);
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