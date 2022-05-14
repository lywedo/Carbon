using System;
using System.Collections.Generic;
using Carbon.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Carbon
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DragGO;
        public GameObject Tile;
        public GameObject BubblePrefab;
        public ItemSerilize ItemSerilize;
        public GameObject BuildingBtn;
        public GameObject TreeBtn;
        public GameObject FlowerBtn;
        public GameObject FacilityBtn;
        private GameObject _CurrentDrag;
        private Dictionary<string, string> _cacheTileCover = new Dictionary<string, string>();
        private int _DragSortOrder = 0;
        private string _tileName;
        private GameObject _CurrentBunble;
        private bool _isMenuBtnClicked = false;
        private string _CurrentMenu = string.Empty;
        private bool _MovingDrag = false;

        private void Awake()
        {
            _tileName = (string) SceneChangeHelper.GetParam(ParamKey.MapClickTile);
            if (ES3.KeyExists(DateTimeHelper.GetToday()))
            {
                _cacheTileCover = ES3.Load<Dictionary<string, string>>(DateTimeHelper.GetToday());
                // ES3.LoadInto(DateTimeHelper.GetToday(), _cacheTileCover);
            }

            
        }
        

        public void BubbleItemLongClick(Item item, Vector3 pos)
        {
            Debug.Log($"BubbleItemLongClick:{item.CoverSprite} {pos}");
            _CurrentBunble.GetComponent<BubbleController>().HideItems();
            Destroy(_CurrentBunble);
            ModifyMenu();
            _CurrentDrag = Instantiate(DragGO, Tile.transform);
            var scale = item.CoverSprite.bounds.size.y / item.CoverSprite.bounds.size.x;
            var width = item.ShowWidth;
            var height = width * scale;
            
            _CurrentDrag.GetComponent<DragController>().SetSprite(item.CoverSprite, new Vector2(width, height), item.Collider2DParam);
            // _CurrentDrag.transform.localScale = Vector3.one;
            _CurrentDrag.transform.position = pos;
            // _CurrentDrag.GetComponent<SortingGroup>().sortingOrder = _DragSortOrder;
            _MovingDrag = true;
        }


        private void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     _CurrentDrag = Instantiate(DragGO, Tile.transform);
            //     _CurrentDrag.GetComponent<SortingGroup>().sortingOrder = _DragSortOrder;
            // }
            //
            if (Input.GetMouseButton(0) && _MovingDrag)
            {
                Vector3 dis = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dis.z = 0;
                _CurrentDrag.transform.position = dis;
                float yPos = _CurrentDrag.transform.position.y;
                _CurrentDrag.GetComponent<SortingGroup>().sortingOrder = -Mathf.RoundToInt(yPos * 1000);
            }
            
            if (Input.GetMouseButtonUp(0) && _MovingDrag)
            {
                _MovingDrag = false;
                if (!_CurrentDrag.GetComponent<DragController>().CanBuild())
                {
                    Destroy(_CurrentDrag);
                }
                else
                {
                    _CurrentDrag.GetComponent<DragController>().Build();
                    // _DragSortOrder++;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToMap();
            }
        }

        private void ModifyMenu()
        {
            BuildingBtn.SetActive(true);
            TreeBtn.SetActive(true);
            FlowerBtn.SetActive(true);
            FacilityBtn.SetActive(true);
            _CurrentMenu = String.Empty;
        }

        public void ClickMenu(GameObject menu)
        {
            if (null != _CurrentBunble)
            {
                _CurrentBunble.GetComponent<BubbleController>().BubbleLongPress -= BubbleItemLongClick;
                Destroy(_CurrentBunble);
            }
            
            if (_CurrentMenu.Equals(string.Empty) || _CurrentMenu != menu.name)
            {
                _CurrentBunble = Instantiate(BubblePrefab, menu.transform);
                _CurrentBunble.GetComponent<BubbleController>().BubbleLongPress += BubbleItemLongClick;
                switch (menu.name)
                {
                    case "building":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(false);
                        FlowerBtn.SetActive(false);
                        FacilityBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.BuildingItems);
                        break;
                    case "tree":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(false);
                        FacilityBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.TreeItems);
                        break;
                    case "flower":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(true);
                        FacilityBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.FlowerItems);
                        break;
                    case "facility":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(true);
                        FacilityBtn.SetActive(true);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.FacilityItems);
                        break;
                }

                _CurrentMenu = menu.name;
            }
            else
            {
                ModifyMenu();
            }
            
        }

        private async void BackToMap()
        {
            _cacheTileCover.Add(_tileName, _tileName);
            ES3.Save(DateTimeHelper.GetToday(), _cacheTileCover);
            await SceneChangeHelper.PreChangeSceneAsync("Map");
            await SceneChangeHelper.ChangeSceneAsync();
        }
    }
}