using System;
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
                var loadImage = ES3.LoadImage(coverSprite);
                // loadImage.Reinitialize(loadImage.width, loadImage.height);
                var sprite = Sprite.Create(loadImage,
                    new Rect(0, 0, loadImage.width, loadImage.height), 
                    new Vector2(0.5f,0.5f));
             
                Cover.sprite = sprite;
                // Cover.size = new Vector2(100, 100);
                Cover.gameObject.SetActive(true);
                gameObject.transform.localScale = _originVector3;
                Debug.Log($"NotifyTileType:{Cover.size}");
            }
        }

        public async void OnClickHandler()
        {
            if (Type == TileType.Unbuild)
            {
                SceneChangeHelper.AddParam(ParamKey.MapClickTile, name);
                await SceneChangeHelper.PreChangeSceneAsync("Game");
                await SceneChangeHelper.ChangeSceneAsync();
            }
        }
    }
}