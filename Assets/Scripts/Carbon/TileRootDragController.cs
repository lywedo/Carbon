using System;

namespace Carbon
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;


//把这个脚本挂在想使用的gameobject上即可
    public class TileRootDragController : MonoBehaviour
    {
        private Vector3 _vec3TargetScreenSpace; // 目标物体的屏幕空间坐标  
        private Vector3 _vec3TargetWorldSpace; // 目标物体的世界空间坐标  
        private Transform _trans; // 目标物体的空间变换组件  
        private Vector3 _vec3MouseScreenSpace; // 鼠标的屏幕空间坐标  
        private Vector3 _vec3Offset; // 偏移 
        public Camera TargetCamera;
        public int TouchLayer = 0;
        private Vector3 InitVector3;

        void Awake()
        {
            _trans = transform;
            // Debug.Log("当前触摸在UI上");
        }

        private void Start()
        {
            InitVector3 = _trans.position;
        }

        public void ResetPos()
        {
            Debug.Log($"ResetPos: {InitVector3}");
            _trans.position = InitVector3;
        }

        void Update()
        {
            // if (IsTouchedUI())
            // {
            //     Debug.Log("当前触摸在UI上");
            // }
            // else
            // {
            //     Debug.Log("当前没有触摸在UI上");
            // }
            if (GlobalVariable.DragLock)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(TargetCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 500, 1<<TouchLayer);
                if(hit.collider != null){
                    StartCoroutine(OnMouseDown());
                }
                
            }
        }
        // void OnMouseDown()
        // {
        //     if(IsTouchedUI())
        //     {
        //         Debug.Log("当前触摸在UI上");
        //     }
        //     else
        //     {
        //         Debug.Log("当前没有触摸在UI上");
        //     }
        // }

        bool IsTouchedUI()
        {
            bool touchedUI = false;
            if (Application.isMobilePlatform)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    touchedUI = true;
                }
            }
            else if (EventSystem.current.IsPointerOverGameObject())
            {
                touchedUI = true;
            }

            return touchedUI;
        }

        IEnumerator OnMouseDown()

        {
            if (IsTouchedUI())
            {
                // Debug.Log("当前触摸在UI上");
            }
            else
            {
                // Debug.Log("当前没有触摸在UI上");
                // Debug.Log("onmousedown");
                // 把目标物体的世界空间坐标转换到它自身的屏幕空间坐标   

                _vec3TargetScreenSpace = TargetCamera.WorldToScreenPoint(_trans.position);

                // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）   

                _vec3MouseScreenSpace =
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);

                // 计算目标物体与鼠标物体在世界空间中的偏移量   

                _vec3Offset = _trans.position - TargetCamera.ScreenToWorldPoint(_vec3MouseScreenSpace);

                // 鼠标左键按下   

                while (Input.GetMouseButton(0))
                {
                    // 存储鼠标的屏幕空间坐标（Z值使用目标物体的屏幕空间坐标）  

                    _vec3MouseScreenSpace =
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);

                    // 把鼠标的屏幕空间坐标转换到世界空间坐标（Z值使用目标物体的屏幕空间坐标），加上偏移量，以此作为目标物体的世界空间坐标  

                    _vec3TargetWorldSpace = TargetCamera.ScreenToWorldPoint(_vec3MouseScreenSpace) + _vec3Offset;

                    // 更新目标物体的世界空间坐标   

                    _trans.position = _vec3TargetWorldSpace;

                    // 等待固定更新   

                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}