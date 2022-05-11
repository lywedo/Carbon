using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Carbon
{
    public class TileController : MonoBehaviour
    {
        public TileType Type = TileType.None;
        public GameObject Cover;
        public GameObject Up;
        private Vector3 _originVector3;

        private void Awake()
        {
            _originVector3 = transform.localScale;
        }

        private void Start()
        {
            NotifyTileType();
        }

        public void NotifyTileType()
        {
            if (Type == TileType.None)
            {
                gameObject.transform.localScale = Vector3.zero;
            }else if (Type == TileType.Unbuild)
            {
                Up.SetActive(true);
                Cover.SetActive(false);
                gameObject.transform.localScale = _originVector3;
            }else if (Type == TileType.builded)
            {
                Up.SetActive(false);
                Cover.SetActive(true);
                gameObject.transform.localScale = _originVector3;
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