using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class MapManager : MonoBehaviour
    {
        public GameObject CloudRoot;
        public Slider Slider;
        public GameObject TileRoot;

        private float _cacheSliderValue = 0;

        private void Start()
        {
            _cacheSliderValue = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(DispearCloud());
            }

            if (Math.Abs(_cacheSliderValue - Slider.value) > 0)
            {
                _cacheSliderValue = Slider.value;
                TileRoot.transform.localScale = new Vector2(_cacheSliderValue, _cacheSliderValue);
            }
        }

        IEnumerator DispearCloud()
        {
            foreach (var sprite in CloudRoot.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sprite.transform.localPosition.x < 0)
                {
                    sprite.transform.DOLocalMoveX(-16, 2);
                }
                else
                {
                    sprite.transform.DOLocalMoveX(16, 2);
                }
                sprite.DOFade(0, 2);
            }

            yield return 0;
        }
    }
}