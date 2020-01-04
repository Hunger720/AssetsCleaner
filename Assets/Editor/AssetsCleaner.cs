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

public class WarningWindow: EditorWindow
{
    string _content = "";

    void OnGUI()
    {
        GUILayout.Label(_content);
    }

    public void SetContent(string content)
    {
        _content = content;
    }
}

public class WhiteListWindow: EditorWindow
{
    bool[] _checkList;
    List<string> _whiteList;
    
    Vector2 scrollPos;

    void OnGUI()
    {
        using (var v = new EditorGUILayout.VerticalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollView.scrollPosition;
                for(int i = 0; i < _whiteList.Count; ++i)
                    _checkList[i] = EditorGUILayout.ToggleLeft(_whiteList[i], _checkList[i]);
            }
        }

        GUILayout.Button("确定", GUILayout.Height(50));
    }

    public void SetWhiteList(List<string> whiteList)
    {
        _whiteList = whiteList;
        _checkList = new bool[whiteList.Count];
        for(int i = 0; i < whiteList.Count; ++i)
            _checkList[i] = false;
    }
}

public class AssetsCleaner
{
    [MenuItem("AssetsCleaner/打开白名单")]
    public static void OpenWhiteList()
    {
        string path = EditorUtility.OpenFolderPanel("打开白名单", Application.dataPath, "");
        if(path == "")
        {
            CreateWarningWindow("警告", "请选择白名单文件");
            return;
        }
    }

    [MenuItem("AssetsCleaner/添加白名单")]
    public static void SetWhiteList()
    {
        // 读取本地白名单
        string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
        if(path == "")
        {
            CreateWarningWindow("警告", "请选择文件夹");
            return;
        }
        string[] ignoreExtensions = {".meta", ".spriteatlas"};
        List<string> files = new List<string>();
        List<string> prefabs = new List<string>();
        List<string> dependencies = new List<string>();
        GetAllFiles(path, ref files, ignoreExtensions);
        Debug.Log("文件夹检查完毕，文件数量为：" + files.Count);
        CheckDirectory(Application.dataPath, ref prefabs, ref dependencies);
        Debug.Log("prefab依赖项：" + dependencies.Count);
        Debug.Log("正在筛选废弃资源...");
        PickUpAbandonedAssets(ref files, ref dependencies);
        Debug.Log("筛选完毕，废弃资源数量为：" + files.Count);
        WhiteListWindow w = (WhiteListWindow)EditorWindow.GetWindow(typeof(WhiteListWindow), false, "白名单设置窗口");
        w.SetWhiteList(files);
        w.autoRepaintOnSceneChange = true;
        w.Show();
        // 勾选白名单
        // 更新本地白名单
    }

    [MenuItem("AssetsCleaner/删除废弃资源")]
    public static void FindAbandonedAssets()
    {
        string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
        if(path == "")
        {
            CreateWarningWindow("警告", "请选择文件夹");
            return;
        }
        string[] ignoreExtensions = {".meta", ".spriteatlas"};
        List<string> files = new List<string>();
        List<string> prefabs = new List<string>();
        List<string> dependencies = new List<string>();
        GetAllFiles(path, ref files, ignoreExtensions);
        Debug.Log("文件夹检查完毕，文件数量为：" + files.Count);
        CheckDirectory(Application.dataPath, ref prefabs, ref dependencies);
        Debug.Log("prefab依赖项：" + dependencies.Count);
        Debug.Log("正在筛选废弃资源...");
        PickUpAbandonedAssets(ref files, ref dependencies);
        Debug.Log("筛选完毕，废弃资源数量为：" + files.Count);
    }

    private static void CreateWarningWindow(string title, string content)
    {
        Rect rect = new Rect(0, 0, 200, 50);
        WarningWindow w = (WarningWindow)EditorWindow.GetWindowWithRect(typeof(WarningWindow), rect, false, title);
        w.SetContent(content);
        w.Show();
    }

    private static void GetAllFiles(string path, ref List<string> files, string[] ignoreExtensions)
    {
        // 遍历当前目录下的文件
        foreach (string f in Directory.GetFiles(path))
        {
            bool ignore = false;
            foreach(string ext in ignoreExtensions)
                if(System.IO.Path.GetExtension(f) == ext)
                {
                    ignore = true;
                    break;
                }

            if(!ignore)
            {
                string tf = f.Replace("\\", "/");
                files.Add(tf.Substring(tf.IndexOf("Assets")));
            }
                
        }
 
        // 遍历当前目录下的文件夹
        foreach (string d in Directory.GetDirectories(path))
            GetAllFiles(d, ref files, ignoreExtensions);
    }

    private static void CheckDirectory(string path, ref List<string> prefabs, ref List<string> dependencies)
    {
        // 遍历当前目录下的文件
        foreach (string file in Directory.GetFiles(path))
        {
            // 获取文件夹中包含后缀为 .prefab 的路径
            if (System.IO.Path.GetExtension(file) == ".prefab")
            {
                string p = file.Substring(file.IndexOf("Assets"));
                prefabs.Add(p);

                string[] ds = AssetDatabase.GetDependencies(p);
                foreach(string d in ds)
                    dependencies.Add(d);
            }
        }
 
        // 遍历当前目录下的文件夹
        foreach (string p in Directory.GetDirectories(path))
            CheckDirectory(p, ref prefabs, ref dependencies);
    }

    private static void PickUpAbandonedAssets(ref List<string> files, ref List<string> dependencies)
    {
        for(int i = 0; i < dependencies.Count; ++i)
            files.Remove(dependencies[i]);
    }
}