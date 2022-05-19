using System;
using System.Net;
using Server;
using UnityEngine;
using UnityEngine.UI;

namespace Carbon
{
    public class InitManager : MonoBehaviour
    {
        public UnityHttpServer HttpServer;
        public Text BtnText;

        private void Start()
        {
            BtnText.text = GetLocalIp();
        }

        public async void JumpMap()
        {
            await SceneChangeHelper.PreChangeSceneAsync("Map");
            SceneChangeHelper.ChangeSceneAsync().Coroutine();
        }
        
        public string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

    }
}