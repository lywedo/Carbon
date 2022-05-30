﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Carbon.Model;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using Random = System.Random;

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
        public GameObject RoadBtn;
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
        public GameObject NormalCanvasRoot;
        public GameObject ShareCanvasRoot;
        public Image ScreenShot;
        public Text EnergyValue;
        public GameObject NotEnoughEnergy;
        public RawImage qrcode;
        public Text QrcodeText;
        public Text Temperture;
        public Text Date;
        public Text ID;
        public TileRootDragController TileRootDragController;
        private Texture2D _encode;
        private long UUID = 1;
        private float InitSliderValue;
        private List<DragController> _CurrentDrags = new List<DragController>();
        private bool _recyclerMode = false;
        public GameObject Recycler_Notice;
        public Camera UpperUICamera;
        public Camera CloudCamera;
        private Sequence mScoreSequence;
        public Text OverlapNotice;
        public GameObject InputCanvas;
        public InputField InputField;


        public delegate void RecycleModeChangeDelegate(bool mode);

        public RecycleModeChangeDelegate RecycleModeChangeListener;

        private void Awake()
        {
            GlobalVariable.DragLock = true;
            InitSliderValue = Slider.value;
            _tileName = (string) SceneChangeHelper.GetParam(ParamKey.MapClickTile);
            if (ES3.KeyExists(DateTimeHelper.GetToday()))
            {
                _cacheTileCover = ES3.Load<Dictionary<string, string>>(DateTimeHelper.GetToday());
                // ES3.LoadInto(DateTimeHelper.GetToday(), _cacheTileCover);
            }
            _encode = new Texture2D(256, 256);
            if (ES3.KeyExists("UUID"))
            {
                UUID = ES3.Load<long>("UUID");
                UUID++;
                ES3.Save("UUID", UUID);
            }
            else
            {
                ES3.Save("UUID", UUID);
            }
            
            InitUI();
            //函数内初始化
            mScoreSequence = DOTween.Sequence();
    //函数内设置属性
            mScoreSequence.SetAutoKill(false);
        }

        public void ShowOverlapNotice(bool show)
        {
            if (show)
            {
                OverlapNotice.transform.localScale = Vector3.one;
            }
            else
            {
                OverlapNotice.transform.localScale = Vector3.zero;
            }
        }

        private void Start()
        {
            CloudController.GetInstance()?.Disspear();
            CloudController.GetInstance()?.ShowTranslucentCloud(CloudCamera);
            InputCanvas.SetActive(true);
        }

        public void ConfirmEnterMapName()
        {
            if (InputField.text.Equals(string.Empty))
            {
                Debug.Log("输入为空");
            }
            else
            {
                InputCanvas.SetActive(false);
                Debug.Log(InputField.text);
            }
        }

        private String FormatUUID(long uuid)
        {
            if (uuid > 100000)
            {
                return uuid.ToString();
            }
            else
            {
                return uuid.ToString().PadLeft(6, '0');
            }
        }
        

        private void InitUI()
        {
            RefreshEnergyText();
            Temperture.text = new Random().Next(10, 30).ToString();
            Date.text = DateTimeHelper.GetToday().Replace("-", "/");
            ID.text = FormatUUID(UUID);
        }

        private void ShowShare()
        {
            TileCamera.gameObject.SetActive(false);
            NormalCanvasRoot.SetActive(false);
            ShareCanvasRoot.SetActive(true);
            UpperUICamera.gameObject.SetActive(false);
        }

        public void HideNotice()
        {
            NotEnoughEnergy.SetActive(false);
        }

        public void GeneralCaptureOnclick()
        {
            if (GlobalVariable.RecycleMode)
            {
                return;
            }
            CloudController.GetInstance().HideTranslucentCloud();
            Slider.value = InitSliderValue;
            TileRootDragController.ResetPos();
            Debug.Log($"ResetPos: {InitSliderValue}");
            StartCoroutine(GeneralCapture());
            // StartCoroutine(GeneralShare());
        }

        // IEnumerator GeneralShare()
        // {
        //     
        // }

        IEnumerator GeneralCapture()
        {
            yield return new WaitForEndOfFrame();
            var path = SaveRenderTexture(CaptureCamera.targetTexture);
            _cacheTileCover.Add(_tileName, path.key);
            ES3.Save(DateTimeHelper.GetToday(), _cacheTileCover);
            var loadImage = ES3.LoadImage(path.key);
            ScreenShot.sprite = Sprite.Create(loadImage,
                new Rect(0, 0, loadImage.width, loadImage.height), 
                new Vector2(0.5f,0.5f));
            StartCoroutine(IRequestPic(path.key, path.pngBuffer));
            ShowShare();
        }
        

        public void BubbleItemLongClick(Item item, Vector3 pos)
        {
            Debug.Log($"BubbleItemLongClick:{item.CoverSprite} {pos}");
            if (GlobalVariable.Energy - item.Price < 0)
            {
                NotEnoughEnergy.SetActive(true);
                Debug.Log("能量不足");
                return;
            }
            _CurrentBunble.GetComponent<BubbleController>().HideItems();
            Destroy(_CurrentBunble);
            ModifyMenu();
            _CurrentDrag = Instantiate(DragGO, Tile.transform);
            _CurrentDrag.GetComponent<DragController>().SetGameManagerAccess(this);
            var scale = item.CoverSprite.bounds.size.y / item.CoverSprite.bounds.size.x;
            var width = item.ShowWidth;
            var height = width * scale;
            // _CurrentDrag.GetComponent<DragController>().DragItem = item;
            _CurrentDrag.GetComponent<DragController>().SetSprite(item, new Vector2(width, height), item.Polygon2DParam);
            // _CurrentDrag.transform.localScale = Vector3.one;
            _CurrentDrag.transform.position = pos;
            // _CurrentDrag.GetComponent<SortingGroup>().sortingOrder = _DragSortOrder;
            _MovingDrag = true;
        }

        public void RefreshEnergyText()
        {
            mScoreSequence.Append(DOTween.To(delegate (float value) {
                //向下取整
                var temp = Math.Floor(value);
                //向Text组件赋值
                EnergyValue.text = temp + "";
            }, Convert.ToSingle(EnergyValue.text), GlobalVariable.Energy, 0.4f));
//将更新后的值记录下来, 用于下一次滚动动画
            // mOldScore = newScore;
            // EnergyValue.text = GlobalVariable.Energy.ToString();
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
                    var dragController = _CurrentDrag.GetComponent<DragController>();
                    dragController.Build();
                    // dragController.SetGameManagerAccess(this);
                    // _CurrentDrags.Add(dragController);
                    RefreshEnergyText();
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
                _cacheTileCover.Add(_tileName, SaveRenderTexture(CaptureCamera.targetTexture).key);
                ES3.Save(DateTimeHelper.GetToday(), _cacheTileCover);
                BackToMap();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                GlobalVariable.Energy = 9999;
                RefreshEnergyText();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Slider.value = InitSliderValue;
                TileRootDragController.ResetPos();
            }
        }

        private void ModifyMenu()
        {
            BuildingBtn.SetActive(true);
            TreeBtn.SetActive(true);
            FlowerBtn.SetActive(true);
            FacilityBtn.SetActive(true);
            RoadBtn.SetActive(true);
            _CurrentMenu = String.Empty;
        }

        public void AddDrags(DragController dragController)
        {
            _CurrentDrags.Add(dragController);
        }

        public void RemoveDrags(DragController dragController)
        {
            _CurrentDrags.Remove(dragController);
        }

        public void ClickRecycle()
        {
            _recyclerMode = !_recyclerMode;
            GlobalVariable.RecycleMode = _recyclerMode;
            foreach (var dragController in _CurrentDrags)
            {
                dragController.SetRecycleMode(_recyclerMode);
            }
            Recycler_Notice.SetActive(_recyclerMode);
        }

        public void ClickMenu(GameObject menu)
        {
            if (GlobalVariable.RecycleMode)
            {
                return;
            }
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
                        RoadBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.BuildingItems);
                        break;
                    case "tree":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(false);
                        FacilityBtn.SetActive(false);
                        RoadBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.TreeItems);
                        break;
                    case "flower":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(true);
                        FacilityBtn.SetActive(false);
                        RoadBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.FlowerItems);
                        break;
                    case "facility":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(true);
                        FacilityBtn.SetActive(true);
                        RoadBtn.SetActive(false);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.FacilityItems);
                        break;
                    case "road":
                        BuildingBtn.SetActive(true);
                        TreeBtn.SetActive(true);
                        FlowerBtn.SetActive(true);
                        FacilityBtn.SetActive(true);
                        RoadBtn.SetActive(true);
                        _CurrentBunble.GetComponent<BubbleController>().SetItems(ItemSerilize.RoadItems);
                        break;
                }

                _CurrentMenu = menu.name;
            }
            else
            {
                ModifyMenu();
            }
            
        }

        private (string key, byte[] pngBuffer) SaveRenderTexture(RenderTexture rt)
        {
            string key = $"{DateTimeHelper.GetMillisecond()}.png";
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            png.Apply();
            var pngBuffer = png.EncodeToPNG();
            ES3.SaveImage(png, key);
            Destroy(png);
            Debug.Log($"保存成功！{key}");
            return (key, pngBuffer);
        }

        IEnumerator IRequestPic(string imgName, byte[] buffer)
        {

            string url = "http://80.209.226.147:8882/sys/file/v1/upload/";
            WWWForm form = new WWWForm();
            form.AddField("folder", "upload");
            var encodeToPng = new Texture2D(200, 200).EncodeToPNG();
            form.AddBinaryData("file", buffer, imgName + ".png", "image/png");
            UnityWebRequest req = UnityWebRequest.Post(url, form);
            yield return req.SendWebRequest();
            Debug.Log($"res: {req.downloadHandler.text}");
            if (req.isHttpError || req.isNetworkError)
            {
                Debug.Log($"res: 上传失败");
                QrcodeText.text = "网络异常";
            }
            if (req.isDone && !req.isHttpError)
            {
                Debug.Log($"res: 上传成功 {req.downloadHandler.text}");
                var res = JsonConvert.DeserializeObject<MinioRes>(req.downloadHandler.text);
                var color32s = Encode(res.result, 256, 256);
                _encode.SetPixels32(color32s);
                _encode.Apply();
                qrcode.texture = _encode;
                qrcode.transform.DOScale(1, 0.5f);
            }
        }
        
        private static Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
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

         public async void BackToMap()
        {
            await SceneChangeHelper.PreChangeSceneAsync("Map");
            CloudController.GetInstance().Assemble(() =>
            {
                SceneChangeHelper.ChangeSceneAsync().Coroutine();
            });
            
        }
    }
}