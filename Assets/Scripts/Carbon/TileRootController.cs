using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Carbon
{
    public class Test
    {
        
    }

    public enum TileType
    {
        None,
        Unbuild,
        builded
    }

    public class TileRootController : MonoBehaviour
    {
        public Camera TileCamera;
        private bool _press = false;
        private Vector3 _cacheInputPos = Vector3.zero;
        public TileController FirstTile;
        private Dictionary<string, TileController> _tileDic = new Dictionary<string, TileController>();
        private Dictionary<string, string> _cacheTileCover = new Dictionary<string, string>();

        private void Start()
        {
            if (ES3.KeyExists(DateTimeHelper.GetToday()))
            {
                _cacheTileCover = ES3.Load<Dictionary<string, string>>(DateTimeHelper.GetToday());
                // foreach (var tileCover in _cacheTileCover)
                // {
                //     Debug.Log($"es3 {tileCover.Key} {tileCover.Value}");
                // }

                StartCoroutine(CheckTile());
            }
            else
            {
                FirstTile.Type = TileType.Unbuild;
                _tileDic.Add(FirstTile.name, FirstTile);
                FirstTile.NotifyTileType();
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //检测鼠标左键是否点击
                RaycastHit2D hit = Physics2D.Raycast(TileCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                    500, 1 << 3);
                if (hit.collider != null)
                {
                    _press = true;
                    _cacheInputPos = Input.mousePosition;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (Vector3.Distance(Input.mousePosition, _cacheInputPos) > 3)
                {
                    _press = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(TileCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                    500, 1 << 3);
                if (hit.collider != null && _press)
                {
                    Debug.Log($"Hitmouseup! {hit.collider.name}");
                    _tileDic[hit.collider.name].OnClickHandler();
                }
            }
        }

        IEnumerator CheckTile()
        {
            // _tileDic.Clear();
            var tileControllers = gameObject.GetComponentsInChildren<TileController>();
            var cacheTileTransforms = gameObject.GetComponentsInChildren<Transform>();
            List<Transform> tileTransforms = new List<Transform>();
            foreach (var cacheTileTransform in cacheTileTransforms)
            {
                if (cacheTileTransform.tag.Equals("Tile"))
                {
                    tileTransforms.Add(cacheTileTransform);
                }
            }

            // var tileTransforms = gameObject.GetComponentsInChildren<Transform>();
            // Debug.Log($"tilecontrollers:{tileControllers.Length}");
            foreach (var tileController in tileControllers)
            {
                if (_cacheTileCover.ContainsKey(tileController.name))
                {
                    tileController.Type = TileType.builded;
                    string coverSprite;
                    _cacheTileCover.TryGetValue(tileController.name, out coverSprite);
                    tileController.NotifyTileType(coverSprite);
                    yield return new WaitForSecondsRealtime(0.2f);
                    // Debug.Log($"checktile: {tileController.name} {tileController.Type} {coverSprite}");
                }

                _tileDic.Add(tileController.name, tileController);
            }

            foreach (var key in _cacheTileCover.Keys)
            {
                // NotifyNearestTileTypeNone(_tileDic[key].transform,
                //     _tileDic[key].NeighborTiles.Count == 0 ? tileTransforms : _tileDic[key].NeighborTiles);
                StartCoroutine(NotifyNearestTileTypeNoneIEnumerator(_tileDic[key].transform,
                    _tileDic[key].NeighborTiles.Count == 0 ? tileTransforms : _tileDic[key].NeighborTiles));
                yield return new WaitForSeconds(0.0001f);
            }
            Debug.Log($"calucateSum: {_caculateSum}");
        }

        private int _caculateSum = 0;

        IEnumerator NotifyNearestTileTypeNoneIEnumerator(Transform player, List<Transform> objects)
        {
            foreach (var o in objects)
            {
                var tileController = o.GetComponent<TileController>();
                if (tileController.Type == TileType.None)
                {
                    tileController.Type = TileType.Unbuild;
                    tileController.NotifyTileType();
                }
                _caculateSum++;
                yield return new WaitForSeconds(0.0001f);
                
            }
        }
        
        private void NotifyNearestTileTypeNone(Transform player, List<Transform> objects)
        {
            foreach (var o in objects)
            {
                var tileController = o.GetComponent<TileController>();
                if (tileController.Type == TileType.None)
                {
                    tileController.Type = TileType.Unbuild;
                    tileController.NotifyTileType();
                }

                _caculateSum++;
            }
        }

        /// <summary> 筛选出最佳物体 </summary> 
        private bool GetNearestGameObjectTileType(Transform player, List<Transform> objects)
        {
            // List<Transform> nearGameObjects = new List<Transform>();

            bool tileType = false;
            for (int i = 1; i < objects.Count; i++)
            {
                Debug.Log(
                    $"distance:{player.name} {objects[i].name} {Vector3.Distance(player.position, objects[i].position)}");
                if (Vector3.Distance(player.position, objects[i].position) < 4.5)
                {
                    // nearGameObjects.Add(objects[i]);
                    if (objects[i].GetComponent<TileController>()?.Type == TileType.builded)
                    {
                        tileType = true;
                    }
                }
            }

            // Debug.Log($"GetNearestGameObjectTileType: {player.name}{nearGameObjects.Count} {tileType}");
            return tileType;
        }
    }
}