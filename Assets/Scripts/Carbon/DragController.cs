using System;
using System.Collections;
using Carbon.Model;
using UnityEngine;

namespace Carbon
{
    public class DragController : MonoBehaviour
    {
        public bool isInArea = false;
        public bool isTrigger = false;
        public SpriteRenderer _spriteRenderer;
        public BoxCollider2D Collider2D;
        private bool _buildFinish = false;
        public Item DragItem;

        // private void Awake()
        // {
        //     _spriteRenderer = GetComponent<SpriteRenderer>();
        // }

        // IEnumerator CheckTrigger()
        // {
        //     while (!_buildFinish)
        //     {
        //         
        //     }
        // }

        public void SetSprite(Sprite sprite, Vector2 size, BoxCollider2DParam collider2DParam)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.size = size;
            Collider2D.size = collider2DParam.Size;
            Collider2D.offset = collider2DParam.Offest;

        }

        public bool CanBuild()
        {
            return isInArea && ! isTrigger;
        }

        public void Build()
        {
            _buildFinish = true;
            GlobalVariable.Energy -= DragItem.Price;
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
            // Debug.Log($"stay {other.gameObject.name}");
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