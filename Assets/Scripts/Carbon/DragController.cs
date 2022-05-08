using System;
using System.Collections;
using UnityEngine;

namespace Carbon
{
    public class DragController : MonoBehaviour
    {
        public bool isInArea = false;
        public bool isTrigger = false;
        private SpriteRenderer _spriteRenderer;
        private bool _buildFinish = false;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // IEnumerator CheckTrigger()
        // {
        //     while (!_buildFinish)
        //     {
        //         
        //     }
        // }

        public bool CanBuild()
        {
            return isInArea && ! isTrigger;
        }

        public void Build()
        {
            _buildFinish = true;
            // StopAllCoroutines();
        }

        private void Update()
        {
            if (CanBuild())
            {
                _spriteRenderer.color = Color.white;
            }
            else
            {
                _spriteRenderer.color = Color.gray;
            }
            
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_buildFinish)
            {
                return;
            }
            Debug.Log($"stay {other.gameObject.name}");
            if (!other.gameObject.tag.Equals("Tile"))
            {
                isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log($"enter {other.gameObject.name}");
            if (other.gameObject.tag.Equals("Tile"))
            {
                isInArea = true;
            }
            
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Debug.Log($"exit {other.gameObject.name}");
            if (other.gameObject.tag.Equals("Tile"))
            {
                isInArea = false;
            }
            else
            {
                isTrigger = false;
            }
            
        }
    }
}