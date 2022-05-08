namespace Carbon
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;


//把这个脚本挂在想使用的gameobject上即可
    public class TileRootDragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Transform Target;

        private Vector3 _cacheV3 = Vector3.zero;
        //点击和拖拽总的触发顺序
        //01 OnPointerDown
        //02 OnBeginDrag
        //03 OnDrag
        //04 OnPointerUp
        //05 OnPointerClick
        //06 OnEndDrag

        //只处理拖拽的触发顺序，先触发OnBeginDrag, 再触发OnDrag，最后触发OnEndDrag
        //开始拖动
        public void OnBeginDrag(PointerEventData eventData)
        {
            //PointerEventData.delta的含义是自上次更新以来的指针坐标增量变化变化。
            // this.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
            _cacheV3.x = eventData.delta.x;
            _cacheV3.y = eventData.delta.y;
            Target.localPosition = _cacheV3;
            //目前设的世界坐标和屏幕坐标一致，所以此时anchoredPosition左下角为(0,0)。右上角为屏幕的最大值。
            Debug.Log("OnBeginDrag anchoredPosition.x = " + this.GetComponent<RectTransform>().anchoredPosition.x);
        }

        //拖动中
        public void OnDrag(PointerEventData eventData)
        {
            // this.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
            _cacheV3.x = eventData.delta.x;
            _cacheV3.y = eventData.delta.y;
            Target.localPosition = _cacheV3;
            //目前设的世界坐标和屏幕坐标一致，所以此时anchoredPosition左下角为(0,0)。右上角为屏幕的最大值。
            Debug.Log("OnDrag anchoredPosition.x = " + this.GetComponent<RectTransform>().anchoredPosition.x);

        }

        //拖动结束后
        public void OnEndDrag(PointerEventData eventData)
        {
            // this.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
            _cacheV3.x = eventData.delta.x;
            _cacheV3.y = eventData.delta.y;
            Target.localPosition = _cacheV3;
            //目前设的世界坐标和屏幕坐标一致，所以此时anchoredPosition左下角为(0,0)。右上角为屏幕的最大值。
            Debug.Log("OnEndDrag anchoredPosition.x = " + this.GetComponent<RectTransform>().anchoredPosition.x);
        }


        // //只处理点击的触发顺序，先触发OnPointerDown, 再触发OnPointerUp，最后触发OnPointerClick
        // //手指按下
        // public void OnPointerDown(PointerEventData eventData)
        // {
        //     Debug.Log("OnPointerDown");
        // }
        // //手指抬起
        // public void OnPointerUp(PointerEventData eventData)
        // {
        //     Debug.Log("OnPointerUp");
        // }
        // //点击
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     Debug.Log("OnPointerClick");
        // }
    }



}