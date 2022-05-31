using System;
using System.Collections;
using System.IO;
using System.Net;
using Server;
using UnityEngine;
using UnityEngine.Networking;
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
            SceneChangeHelper.PreChangeSceneAsync("Map").Coroutine();
        }

        public async void JumpMap()
        {
            CloudController.GetInstance().Assemble(() =>
            {
                SceneChangeHelper.ChangeSceneAsync().Coroutine();
            });
            // await SceneChangeHelper.PreChangeSceneAsync("Map");
            
        }

        public void ClearCache()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);
        }
        
        IEnumerator IRequestPic(string imgName)
        {
            FileStream fs = new FileStream("C:\\Users\\admin\\Pictures\\mm_facetoface_collect_qrcode_1651482467494_[B@ea59683.png", FileMode.Open, FileAccess.Read);
            byte[] bytebuffer;
            bytebuffer = new byte[fs.Length];
            fs.Read(bytebuffer, 0, (int)fs.Length);
 
            
            string url = "http://10.23.105.222:8080/sys/file/v1/upload/";//这里需要注意一下phpStudy中的端口号
            WWWForm form = new WWWForm();
            form.AddField("folder", "upload");
            var encodeToPng = new Texture2D(200, 200).EncodeToPNG();
            form.AddBinaryData("file", bytebuffer, imgName + ".png", "image/png");
            UnityWebRequest req = UnityWebRequest.Post(url, form);
            yield return req.SendWebRequest();
            Debug.Log($"res: {req.downloadHandler.text}");
            fs.Close();
            if (req.isHttpError || req.isNetworkError)
            {
                Debug.Log($"res: 上传失败");
            }
            if (req.isDone && !req.isHttpError)
            {
                Debug.Log($"res: 上传成功 {req.downloadHandler.text}");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(IRequestPic("haha"));
            }
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