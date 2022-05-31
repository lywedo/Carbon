using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Carbon
{
    public class TileController : MonoBehaviour
    {
        public TileType Type = TileType.None;
        public SpriteRenderer Cover;
        public GameObject Up;
        private Vector3 _originVector3;
        public List<Transform> NeighborTiles = new List<Transform>();
        private Sprite _showSprite;
        private Texture2D _showTexture2d;

        private void Awake()
        {
            _originVector3 = transform.localScale;
            NotifyTileType();
        }

        public void NotifyTileType(string coverSprite = null)
        {
            if (Type == TileType.None)
            {
                gameObject.transform.localScale = Vector3.zero;
            }else if (Type == TileType.Unbuild)
            {
                Up.SetActive(true);
                Cover.gameObject.SetActive(false);
                gameObject.transform.localScale = _originVector3;
            }else if (Type == TileType.builded)
            {
                Up.SetActive(false);
                if (ES3.FileExists(coverSprite))
                {
                    _showTexture2d = ES3.LoadImage(coverSprite);
                    // loadImage.Reinitialize(loadImage.width, loadImage.height);
                    _showSprite = Sprite.Create(_showTexture2d,
                        new Rect(0, 0, _showTexture2d.width, _showTexture2d.height), 
                        new Vector2(0.5f,0.5f));
             
                    Cover.sprite = _showSprite;
                    // Cover.size = new Vector2(100, 100);
                    Cover.gameObject.SetActive(true);
                    gameObject.transform.localScale = _originVector3;
                }
                
                Debug.Log($"NotifyTileType:{Cover.size}");
            }
        }

        private void OnDestroy()
        {
            Destroy(_showSprite);
            Destroy(_showTexture2d);
        }

        public async void OnClickHandler()
        {
            if (Type == TileType.Unbuild && !GlobalVariable.DragLock)
            {
                SceneChangeHelper.AddParam(ParamKey.MapClickTile, name);
                await SceneChangeHelper.PreChangeSceneAsync("Game");
                CloudController.GetInstance().Assemble(() =>
                {
                    SceneChangeHelper.ChangeSceneAsync().Coroutine();
                });
                
                
            }
        }
    }
}