// AssetsCleaner.cs
// 废弃资源的查找和清除

using AssetBundleBrowser.AssetBundleDataSource;
using AssetBundleBrowser.AssetBundleModel;
using Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AssetsCleaner
{
    [MenuItem("AssetsCleaner/查找所有废弃资源")]
    public static void FindAbandonedAssets()
    {
        // 列出所有prefab
        string path = Application.dataPath + "/UI/Windows";
        List<string> dirs = new List<string>();
        GetDirs(path, ref dirs);
    }

    private static void GetDirs(string dirPath, ref List<string> dirs)
    {
        foreach (string path in Directory.GetFiles(dirPath))
        {
            //获取所有文件夹中包含后缀为 .prefab 的路径
            if (System.IO.Path.GetExtension(path) == ".prefab")
            {
                string p = path.Substring(path.IndexOf("Assets"));
                dirs.Add(p);
                Debug.Log(p);

                string[] dependencies = AssetDatabase.GetDependencies(p);
                foreach(string d in dependencies)
                {
                    Debug.Log(d);
                }
            }
        }
 
        foreach (string path in Directory.GetDirectories(dirPath))
        {
            GetDirs(path, ref dirs);
        }
    }

}