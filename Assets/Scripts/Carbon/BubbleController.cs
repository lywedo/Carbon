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

        public void SetItems(List<Item> items)
        {
            for (var i = 0; i < Content.childCount; i++)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
            
            foreach (var item in items)
            {
                var instantiate = Instantiate(BubbleItemPrefab, Content);
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
            Debug.Log($"bubble: {Content.rect.width}");
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.rect.width + 150, Content.rect.height + 150);
            // Debug.Log($"bubble: {Content.rect.width}");
            // gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.rect.width + 150, Content.rect.height + 150);
        }
    }
}