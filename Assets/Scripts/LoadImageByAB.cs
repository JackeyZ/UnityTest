using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoadImageByAB : MonoBehaviour {

    public T LoadObject<T>(string bundleName, string assetName) where T : UnityEngine.Object
    {
        string assetPath = this.GetAssetPath(bundleName, assetName);
        if (!string.IsNullOrEmpty(assetPath))
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        return (T)null;
    }

    public string GetAssetPath(string bundleName, string assetName)
    {
        string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, Path.GetFileNameWithoutExtension(assetName));
        if(paths.Length == 0)
        {
            Debug.LogError("AB包路径有误：" + bundleName + "/" + assetName);
            return null;
        }
        string extension = Path.GetExtension(assetName);
        foreach (string path in paths)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return path;
            }
            else if (Path.GetExtension(path) == extension)
            {
                return path;
            }
        }
        return null;
    }

    void Start()
    {
        Image img = GetComponent<Image>();
        img.sprite = LoadObject<Sprite>("image", "ClipImg");
    }
}
