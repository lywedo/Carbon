using System;
using UnityEngine;

namespace Carbon
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DragGO;
        public GameObject Tile;
        private GameObject _CurrentDrag;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _CurrentDrag = Instantiate(DragGO, Tile.transform);
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
                }
            }
        }
    }
}