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

        private void Awake()
        {
            _tileName = (string) SceneChangeHelper.GetParam(ParamKey.MapClickTile);
            if (ES3.KeyExists(DateTimeHelper.GetToday()))
            {
                _cacheTileCover = ES3.Load<Dictionary<string, string>>(DateTimeHelper.GetToday());
                // ES3.LoadInto(DateTimeHelper.GetToday(), _cacheTileCover);
            }
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _CurrentDrag = Instantiate(DragGO, Tile.transform);
                _CurrentDrag.GetComponent<SortingGroup>().sortingOrder = _DragSortOrder;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 dis = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dis.z = 0;
                _CurrentDrag.transform.position = dis;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!_CurrentDrag.GetComponent<DragController>().CanBuild())
                {
                    Destroy(_CurrentDrag);
                }
                else
                {
                    _CurrentDrag.GetComponent<DragController>().Build();
                    _DragSortOrder++;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToMap();
            }
        }

        private void ModifyMenu()
        {
        }

        public void ClickMenu(GameObject menu)
        {
            Destroy(_CurrentBunble);
            if (_CurrentMenu.Equals(string.Empty) || _CurrentMenu != menu.name)
            {
                _CurrentBunble = Instantiate(BubblePrefab, menu.transform);
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
                BuildingBtn.SetActive(true);
                TreeBtn.SetActive(true);
                FlowerBtn.SetActive(true);
                FacilityBtn.SetActive(true);
                _CurrentMenu = String.Empty;
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