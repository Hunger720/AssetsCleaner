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
using AssetsCleanerWindows;

// // 添加白名单窗口
// public class AddWhiteListWindow: EditorWindow
// {
//     bool[] _checkList;
//     Vector2 _scrollPos;
//     List<string> _whiteList = new List<string>();
//     List<string> _saveWhiteList;

//     void OnGUI()
//     {
//         using (var v = new EditorGUILayout.VerticalScope())
//         {
//             using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
//             {
//                 _scrollPos = scrollView.scrollPosition;
//                 for(int i = 0; i < _whiteList.Count; ++i)
//                     _checkList[i] = EditorGUILayout.ToggleLeft(_whiteList[i], _checkList[i]);
//             }
//         }

//         // float w = maxSize[0];
//         // float h = maxSize[1];
//         int bh = 30;

//         // GUILayout.BeginArea(new Rect(0, h-bh, w, bh));
//         // if(GUILayout.Button("添加", GUILayout.Width(w), GUILayout.Height(bh)))
//         //     OnAddButtonClick();
//         // GUILayout.EndArea();
//         if(GUILayout.Button("添加", GUILayout.Height(bh)))
//             OnAddButtonClick();
//     }

//     private void OnAddButtonClick()
//     {
//         for(int i = 0; i < _checkList.Length; ++i)
//         {
//             if(_checkList[i])
//             {
//                 Debug.Log(_whiteList[i]);
//                 // 将_whiteList[i]添加到_saveWhiteList
//             }
//         }
//         this.Close();
//     }

//     private void GetAddWhiteListFiles(string path, ref List<string> files)
//     {
//         foreach(string f in Directory.GetFiles(path, "*.png"))
//             files.Add(f);
        
//         foreach(string d in Directory.GetDirectories(path))
//             GetAddWhiteListFiles(d, ref files);
        
//     }

//     public void OpenAddWhiteListFolder()
//     {
//         string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
//         if(path == "")
//         {
//             Debug.LogWarning("请选择文件夹");
//             return;
//         }
//         GetAddWhiteListFiles(path, ref _whiteList);
//         _checkList = new bool[_whiteList.Count];
//         for(int i = 0; i < _whiteList.Count; ++i)
//             _checkList[i] = false;
//     }

//     public void SetSaveWhiteList(ref List<string> saveWhiteList)
//     {
//         _saveWhiteList = saveWhiteList;
//     }
// }

// // 白名单窗口
// public class WhiteListWindow: EditorWindow
// {
//     bool[] _checkList;
//     List<string> _whiteList;
    
//     Vector2 _scrollPos;

//     void OnGUI()
//     {
//         using (var v = new EditorGUILayout.VerticalScope())
//         {
//             using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
//             {
//                 _scrollPos = scrollView.scrollPosition;
//                 for(int i = 0; i < _whiteList.Count; ++i)
//                     _checkList[i] = EditorGUILayout.ToggleLeft(_whiteList[i], _checkList[i]);
//             }
//         }

//         float w = maxSize[0];
//         float h = maxSize[1];
//         float bw = w / 3;
//         int bh = 30;

//         GUILayout.BeginArea(new Rect(0, h-bh, bw, bh));
//         if(GUILayout.Button("添加", GUILayout.Width(bw), GUILayout.Height(bh)))
//             OnAddButtonClick(ref _whiteList);
//         GUILayout.EndArea();

//         GUILayout.BeginArea(new Rect(bw, h-bh, bw, bh));
//         if(GUILayout.Button("删除", GUILayout.Width(bw), GUILayout.Height(bh)))
//             OnDeleteButtonClick(ref _whiteList);
//         GUILayout.EndArea();

//         GUILayout.BeginArea(new Rect(2*bw, h-bh, bw, bh));
//         if(GUILayout.Button("保存", GUILayout.Width(bw), GUILayout.Height(bh)))
//             OnSaveButtonClick(ref _whiteList);
//         GUILayout.EndArea();
//     }

//     private void OnAddButtonClick(ref List<string> whiteList)
//     {
//         Debug.Log("点击添加按钮");
//         this.Close();
//     }

//     private void OnDeleteButtonClick(ref List<string> whiteList)
//     {
//         Debug.Log("点击删除按钮");
//     }

//     private void OnSaveButtonClick(ref List<string> whiteList)
//     {
//         Debug.Log("点击保存按钮");
//     }

//     public void SetWhiteList(List<string> whiteList)
//     {
//         _whiteList = whiteList;
//         _checkList = new bool[whiteList.Count];
//         for(int i = 0; i < whiteList.Count; ++i)
//             _checkList[i] = false;
//     }
// }

public class AssetsCleaner
{
    [MenuItem("AssetsCleaner/打开白名单")]
    public static void OpenWhiteList()
    {
        string path = EditorUtility.OpenFilePanel("打开白名单", Application.dataPath, "txt");
        if(path == "")
        {
            Debug.LogWarning("请选择白名单文件");
            return;
        }

        List<string> whiteList = new List<string>();
        using(StreamReader sr = new StreamReader(path)) 
        {
            string line;
            while ((line = sr.ReadLine()) != null) 
            {
                whiteList.Add(line);
            }
        }

        int width = 500;
        int height = 650;
        Rect rect = new Rect(0, 0, width, height);
        WhiteListWindow w = (WhiteListWindow)EditorWindow.GetWindowWithRect(typeof(WhiteListWindow), rect, false, "白名单");
        w.maxSize = w.minSize = new Vector2(width, height);
        w.SetWhiteList(whiteList);
        w.Show();
    }

    [MenuItem("AssetsCleaner/添加白名单")]
    public static void SetWhiteList()
    {
        string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
        if(path == "")
        {
            Debug.LogWarning("请选择文件夹");
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
    }

    [MenuItem("AssetsCleaner/删除废弃资源")]
    public static void FindAbandonedAssets()
    {
        string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
        if(path == "")
        {
            Debug.LogWarning("请选择文件夹");
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


    [MenuItem("AssetsCleaner/转换纹理图片格式")]
    public static void TextureSetting()
    {
        string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath + "\\UI", "");
        if(path == "")
        {
            Debug.LogWarning("请选择文件夹");
            return;
        }
        // UI交互
        // 一个勾选isReadable的toggle
        // 一个下拉菜单
        SetTexture(path);
    }

    private static void SetTexture(string path)
    {
        string fp = "";
        foreach(string f in Directory.GetFiles(path, "*.png"))
        {  
            fp = f.Substring(f.IndexOf("Assets")).Replace("\\", "/");
            Debug.Log(fp);
            TextureImporter ti = TextureImporter.GetAtPath(fp) as TextureImporter;
            ti.isReadable = false;
            ti.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA);
        }

        foreach(string d in Directory.GetDirectories(path))
            SetTexture(d);
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