using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ParamKey
{
    MapClickTile,
    HttpServer
} 

public class SceneChangeHelper
{
    public static AsyncOperation loadMapOperation;
    public static Dictionary<ParamKey, object> sceneOneshotData = new Dictionary<ParamKey, object>();

    public static async ETTask PreChangeSceneAsync(string sceneName)
    {
        // 加载map
        loadMapOperation = SceneManager.LoadSceneAsync(sceneName);
        loadMapOperation.allowSceneActivation = false;
        Debug.Log("fifnishet===");
        await ETTask.CompletedTask;
        Debug.Log("fifnishet");
    }

    public static void AddParam(ParamKey key, object value)
    {
        sceneOneshotData.Add(key, value);
    }

    public static object GetParam(ParamKey key)
    {
        object value = null;
        sceneOneshotData.TryGetValue(key, out value);
        sceneOneshotData.Remove(key);
        return value;
    }

    public static async ETTask ChangeSceneAsync()
    {
        loadMapOperation.allowSceneActivation = true;
        await ETTask.CompletedTask;
    }
}