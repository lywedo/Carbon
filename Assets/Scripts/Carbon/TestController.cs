using System;
using Carbon.Model;
using UnityEngine;

namespace Carbon
{
    public class TestController : MonoBehaviour
    {
        public ItemSerilize ItemSerilize;
        public BubbleController BubbleController;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                BubbleController.SetItems(ItemSerilize.BuildingItems);
            }
        }
    }
}