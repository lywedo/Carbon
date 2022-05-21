using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BestHTTP;
using UnityEngine;
using UnityEngine.Networking;

namespace Carbon
{
    public class MinIOHelper
    {
        private HTTPRequest request;
        
        public void UploadPicture()
        {
            Debug.Log($"UploadPicture");
            FileStream fs = new FileStream("C:\\Users\\admin\\Pictures\\mm_facetoface_collect_qrcode_1651482467494_[B@ea59683.png", FileMode.Open, FileAccess.Read);
            byte[] bytebuffer;
            bytebuffer = new byte[fs.Length];
            fs.Read(bytebuffer, 0, (int)fs.Length);
            Debug.Log($"UploadPicture {bytebuffer.Length}");
            Dictionary<string, byte[]> param = new Dictionary<string, byte[]>();
            param["file"] = bytebuffer;
            SendPostMethod("/sys/file/v1/upload/", param);
            fs.Close();

            
        }
        
        IEnumerator IRequestPic(string imgName)
        {
            string url = "http://10.23.105.222:8080/sys/file/v1/upload/";//这里需要注意一下phpStudy中的端口号
            WWWForm form = new WWWForm();
            form.AddField("folder", "upload");
            var encodeToPng = new Texture2D(200, 200).EncodeToPNG();
            form.AddBinaryData("file", encodeToPng, imgName + ".png", "image/png");
            UnityWebRequest req = UnityWebRequest.Post(url, form);
            yield return req.SendWebRequest();
            // if (req.isHttpError || req.isNetworkError)
            // {
            //     m_info = "上传失败";
            // }
            // if (req.isDone && !req.isHttpError)
            // {
            //     m_info = "上传成功";
            //     imgPath = req.downloadHandler.text;
            // }
        }

        
        void SendPostMethod(string url,Dictionary<string, byte[]> paramsDict) {
            string requestUrl = "http://10.23.105.222:8080" + url;
            Uri uri = new Uri(requestUrl);
            request = new HTTPRequest(uri,HTTPMethods.Post,OnRequestFinished);
            if (paramsDict!=null&&paramsDict.Count>0)
            {
                foreach (var item in paramsDict.Keys)
                {
                    string key = item;
                    byte[] value = paramsDict[key];
                    // request.AddField(key,value);
                    var encodeToPng = new Texture2D(200, 200).EncodeToPNG();
                    request.AddBinaryData(key,encodeToPng, "saoma");
                }
            }

            request.Send();
        }

        void OnRequestFinished(HTTPRequest _request, HTTPResponse response)
        {
            Debug.Log($"response: {response.Message} {response.StatusCode} {response.Data.ToString()}");
        }
    }
}