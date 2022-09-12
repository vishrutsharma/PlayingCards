using System;
using UnityEngine;

namespace JungleeCards.Common
{
    public class JSONLoader : MonoBehaviour
    {
        public void LoadJSON<T>(string resourcePath, Action<T> callback)
        {
            var data = Resources.Load<TextAsset>(resourcePath);
            var jsonData = JsonUtility.FromJson<T>(data.text);
            callback.Invoke(jsonData);
        }
    }
}
