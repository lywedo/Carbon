using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carbon
{
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
        private Dictionary<string, TileController> _tileDic = new Dictionary<string, TileController>();
        private Dictionary<string, string> _cacheTileCover = new Dictionary<string, string>(); 
        private void Start()
        {
            if (ES3.KeyExists(DateTimeHelper.GetToday()))
            {
                _cacheTileCover = ES3.Load<Dictionary<string, string>>(DateTimeHelper.GetToday());
                foreach (var tileCover in _cacheTileCover)
                {
                    Debug.Log($"es3 {tileCover.Key} {tileCover.Value}");
                }
                // ES3.LoadInto(DateTimeHelper.GetToday(), _cacheTileCover);
            }
            StartCoroutine(CheckTile());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown (0)) { //检测鼠标左键是否点击
                RaycastHit2D hit = Physics2D.Raycast(TileCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 500, 1<<3);
                if(hit.collider != null)
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
                RaycastHit2D hit = Physics2D.Raycast(TileCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 500, 1<<3);
                if(hit.collider != null && _press){
                    Debug.Log ($"Hitmouseup! {hit.collider.name}");
                    _tileDic[hit.collider.name].OnClickHandler();
                }
            }
        }

        IEnumerator CheckTile()
        {
            var tileControllers = gameObject.GetComponentsInChildren<TileController>();
            var tileTransforms = gameObject.GetComponentsInChildren<Transform>();
            Debug.Log($"tilecontrollers:{tileControllers.Length}");
            foreach (var tileController in tileControllers)
            {
                Debug.Log($"checktile: {tileController.name}");
                if (_cacheTileCover.ContainsKey(tileController.name))
                {
                    tileController.Type = TileType.builded;
                    tileController.NotifyTileType();
                }
                _tileDic.Add(tileController.name, tileController);
                if (tileController.Type == TileType.None)
                {
                    if (GetNearestGameObjectTileType(tileController.transform, tileTransforms))
                    {
                        tileController.Type = TileType.Unbuild;
                        tileController.NotifyTileType();
                    }
                }
                
                yield return new WaitForSeconds(0.0001f);
            }

            
        }
        
        /// <summary> 筛选出最佳物体 </summary> 
        private bool GetNearestGameObjectTileType(Transform player, Transform[] objects)
        {
            // List<Transform> nearGameObjects = new List<Transform>();

            bool tileType = false;
            for (int i = 1; i < objects.Length; i++)
            { 
                if (Vector3.Distance(player.position, objects[i].position) < 4)
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