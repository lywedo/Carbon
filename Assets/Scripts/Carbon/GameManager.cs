using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Carbon.Model;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
        public Slider Slider;
        private GameObject _CurrentBunble;
        private bool _isMenuBtnClicked = false;
        private string _CurrentMenu = string.Empty;
        private bool _MovingDrag = false;
        private float _cacheSliderValue = 0;
        public Camera TileCamera;
        public Camera CaptureCamera;
        public RectTransform CaptureRect;

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
                Vector3 dis = TileCamera.ScreenToWorldPoint(Input.mousePosition);
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
            if (Math.Abs(_cacheSliderValue - Slider.value) > 0.1)
            {
                
                _cacheSliderValue = Slider.value;
                // TileRoot.transform.localScale = new Vector2(_cacheSliderValue, _cacheSliderValue);
                TileCamera.orthographicSize = _cacheSliderValue;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToMap();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                _cacheTileCover.Add(_tileName, SaveRenderTexture(CaptureCamera.targetTexture));
                ES3.Save(DateTimeHelper.GetToday(), _cacheTileCover);
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
        
        private string SaveRenderTexture(RenderTexture rt)
        {
            string key = $"{DateTimeHelper.GetMillisecond()}.png";
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            png.Apply();
            ES3.SaveImage(png, key);
            Destroy(png);
            Debug.Log($"保存成功！{key}");
            return key;
        }

        /// <summary>
        /// 对相机截图
        /// </summary>
        /// <param name="camera">Camera.要被截屏的相机</param>
        /// <param name="rect">Rect.截屏的区域</param>
        /// <returns>The screenshot2.</returns>
        // Texture2D CaptureByCamera(Camera camera, Rect rect)
        // {
        //     Texture2D savedTexture = new Texture2D((int) rect.width, (int) rect.height);
        //     var cameraTargetTexture = camera.targetTexture;
        //     Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.RGBA32, false);
        //     newTexture.SetPixels(0, 0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
        //     newTexture.Apply();
        //
        //
        //     byte[] bytes = screenShot.EncodeToPNG();//最后将这些纹理数据，成一个png图片文件
        //     string filename = Application.dataPath + "/Screenshot.png";
        //     System.IO.File.WriteAllBytes(filename, bytes);
        //     Debug.Log(string.Format("截屏了一张照片: {0}", filename));
        //     System.Diagnostics.Process.Start(filename);
        //     return screenShot;
        // }

        
        IEnumerator GetScreenTexture(RectTransform rectT)
        {
            yield return new WaitForEndOfFrame();
            Texture2D screenShot = new Texture2D((int)rectT.rect.width, (int)rectT.rect.height, TextureFormat.RGB24, true);
            float x = rectT.localPosition.x + (Screen.width - rectT.rect.width) / 2;
            float y = rectT.localPosition.y + (Screen.height - rectT.rect.height) / 2;
            Rect position = new Rect(x, y, rectT.rect.width, rectT.rect.height);
            screenShot.ReadPixels(position, 0, 0, true);//按照设定区域读取像素；注意是以左下角为原点读取
            screenShot.Apply();

            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
            string filePath = "";
            filePath = Application.persistentDataPath + "/HeadFold";
            string scrPathName = filePath + "/" + fileName;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //二进制转换
            byte[] byt = screenShot.EncodeToPNG();
            File.WriteAllBytes(scrPathName, byt);
            System.Diagnostics.Process.Start(scrPathName);
        }

        private async void BackToMap()
        {
            await SceneChangeHelper.PreChangeSceneAsync("Map");
            await SceneChangeHelper.ChangeSceneAsync();
        }
    }
}