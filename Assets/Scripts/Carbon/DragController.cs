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

        public Animator Animator;
        // public BoxCollider2D Collider2D;
        public PolygonCollider2D Polygon;
        private bool _buildFinish = false;
        private Item DragItem;
        private bool _recyclerMode = false;
        private GameManager _manager;
        AnimatorOverrideController overrideController;


        // IEnumerator CheckTrigger()
        // {
        //     while (!_buildFinish)
        //     {
        //         
        //     }
        // }

        public void SetGameManagerAccess(GameManager manager)
        {
            _manager = manager;
            _manager.AddDrags(this);
        }

        private void OnDestroy()
        {
            _manager.RemoveDrags(this);
        }

        public void SetRecycleMode(bool mode)
        {
            _recyclerMode = mode;
            if (_recyclerMode)
            {
                Polygon.points = DragItem.Click2DParam.points;
            }
            else
            {
                Polygon.points = DragItem.Polygon2DParam.points;
            }
            
        }

        // private void ResetCollider()
        // {
        //     boxCollider.enabled = true;
        // }

        public void Recycle()
        {
            Debug.Log("recycle");
            if (_recyclerMode)
            {
                GlobalVariable.Energy += DragItem.Price;
                _manager.RefreshEnergyText();
                Destroy(gameObject);
            }
            
        }

        public void SetSprite(Item item, Vector2 size, PolygonCollider2DParam polygon2dparam)
        {
            DragItem = item;
            _spriteRenderer.sprite = item.CoverSprite;
            _spriteRenderer.size = size;
            // Collider2D.size = collider2DParam.Size;
            // Collider2D.offset = collider2DParam.Offest;
            Debug.Log($"setsprite:{polygon2dparam.points.Length}");
            Polygon.points = polygon2dparam.points;
        }

        public bool CanBuild()
        {
            return isInArea && ! isTrigger;
        }

        public void Build()
        {
            _buildFinish = true;
            GlobalVariable.Energy -= DragItem.Price;
            if (DragItem.AnimattionClip != null)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = Animator.runtimeAnimatorController;
                overrideController["1"] = DragItem.AnimattionClip;
                Animator.runtimeAnimatorController = overrideController;
                Debug.Log($"build:{DragItem.AnimattionClip}");
            }
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