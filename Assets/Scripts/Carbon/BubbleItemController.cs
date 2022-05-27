using System;
using Carbon.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class BubbleItemController : MonoBehaviour
    {
        public Item Item;
        public delegate void OnBubbleItemLongClickDelegate(Item item, Vector3 pos);

        public OnBubbleItemLongClickDelegate OnBubbleItemLongClick;

        private void Start()
        {
            // GetComponent<XButton>().OnLongPress.AddListener(() =>
            // {
            //     OnBubbleItemLongClick?.Invoke(Item, GetUIToWordPos(gameObject));
            // });
            
            GetComponent<XButton>().OnPointDown.AddListener(() =>
            {
                OnBubbleItemLongClick?.Invoke(Item, GetUIToWordPos(gameObject));
            });
            // GetComponent<XButton>().OnLongPress.AddListener(() =>
            // {
            //     Debug.Log($"BubbleItemLongClickListener: ");
            //     OnBubbleItemLongClick?.Invoke(Item);
            // });
        }

        private void OnDestroy()
        {
            OnBubbleItemLongClick = null;
        }
        
        public Vector3 GetUIToWordPos(GameObject uiObj)
        {
            Vector3 ptScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, uiObj.transform.position);
            ptScreen.z = 0;
            ptScreen.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 ptWorld = Camera.main.ScreenToWorldPoint(ptScreen);
            return ptWorld;
        }
        
    }
    
}