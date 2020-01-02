// AssetsCleaner.cs
// 废弃资源的查找和清除
// 功能
// 1.找出指定文件夹中所有的prefab
// 2.找到所有prefab依赖的资源
// 3.列出所有没用到的资源
// 4.支持白名单

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AssetsCleaner
{
    [MenuItem("AssetsCleaner/查找文件夹资源")]
    public static void OpenFolder()
    {
        // 列出所有prefab
        string path = EditorUtility.OpenFolderPanel("Open File Dailog", Application.dataPath, ".prefab");
        Debug.Log(path);
    }

    [MenuItem("AssetsCleaner/查找所有废弃资源")]
    public static void FindAbandonedAssets()
    {
        // 列出所有prefab
        string path = Application.dataPath;
        List<string> dirs = new List<string>();
        int count = GetDirs(path, ref dirs);
        Debug.Log("查找文件数为:" + count);
    }

    private static int GetDirs(string dirPath, ref List<string> dirs)
    {
        Debug.Log("正在查找文件夹:" + dirPath);

        int count = 0;
        // 遍历当前目录下的文件
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
                    ++count;
                }
            }
        }
 
        // 遍历当前目录下的文件夹
        foreach (string path in Directory.GetDirectories(dirPath))
        {
            count += GetDirs(path, ref dirs);
        }

        return count;
    }

}