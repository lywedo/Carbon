using System;
using System.Collections.Generic;
using Carbon.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class BubbleController : MonoBehaviour
    {
        public RectTransform Content;
        public GameObject BubbleItemPrefab;

        public delegate void BubbleLongPressDelegate(Item item, Vector3 pos);

        public BubbleLongPressDelegate BubbleLongPress;

        public void BubbleItemLongClickListener(Item item, Vector3 pos)
        {
            // Debug.Log($"BubbleItemLongClickListener: {item.CoverSprite}");
            BubbleLongPress?.Invoke(item, pos);
        }

        private void OnDestroy()
        {
            HideItems();
        }

        public void HideItems()
        {
            for (var i = 0; i < Content.childCount; i++)
            {
                Content.GetChild(i).gameObject.GetComponent<BubbleItemController>().OnBubbleItemLongClick -=
                    BubbleItemLongClickListener;
                Destroy(Content.GetChild(i).gameObject);
            }
        }

        public void SetItems(List<Item> items)
        {
            HideItems();
            
            foreach (var item in items)
            {
                var instantiate = Instantiate(BubbleItemPrefab, Content);
                instantiate.GetComponent<BubbleItemController>().Item = item;
                instantiate.GetComponent<BubbleItemController>().OnBubbleItemLongClick += BubbleItemLongClickListener;
                
                foreach (var image in instantiate.GetComponentsInChildren<Image>())
                {
                    if (image.name.Equals("cover"))
                    {
                        image.sprite = item.CoverSprite;
                    }
                }

                foreach (var text in instantiate.GetComponentsInChildren<Text>())
                {
                    text.text = $"{item.Price}g";
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
            // Debug.Log($"bubble: {Content.rect.width}");
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.rect.width + 150, Content.rect.height + 150);
            // Debug.Log($"bubble: {Content.rect.width}");
            // gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.rect.width + 150, Content.rect.height + 150);
        }
    }
}