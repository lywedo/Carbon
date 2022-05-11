using System;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Carbon.Model
{
    public enum ItemType
    {
        Building,
        Tree,
        Flower,
        Facility
    }

    [CreateAssetMenu(fileName = "ItemSerilize", menuName = "CreateItemSerilize", order = 0)]
    public class ItemSerilize : ScriptableObject
    {
        public List<Item> BuildingItems = new List<Item>();
        public List<Item> TreeItems = new List<Item>();
        public List<Item> FlowerItems = new List<Item>();
        public List<Item> FacilityItems = new List<Item>();
        
    }
    
    [Serializable]
    public class Item
    {
        public ItemType Type;
        public int Price;
        public Sprite CoverSprite;
    }
}