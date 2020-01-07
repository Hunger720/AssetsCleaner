// AssetsCleanerWindows.cs
// AssetsCleaner的UI层

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetsCleanerWindows
{
    // 添加白名单窗口
    public class AddWhiteListWindow: EditorWindow
    {
        bool[] _checkList;
        Vector2 _scrollPos;
        List<string> _whiteList = new List<string>();
        List<string> _saveWhiteList;

        void OnGUI()
        {
            using (var v = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
                {
                    _scrollPos = scrollView.scrollPosition;
                    for(int i = 0; i < _whiteList.Count; ++i)
                        _checkList[i] = EditorGUILayout.ToggleLeft(_whiteList[i], _checkList[i]);
                }
            }

            // float w = maxSize[0];
            // float h = maxSize[1];
            int bh = 30;

            // GUILayout.BeginArea(new Rect(0, h-bh, w, bh));
            // if(GUILayout.Button("添加", GUILayout.Width(w), GUILayout.Height(bh)))
            //     OnAddButtonClick();
            // GUILayout.EndArea();
            if(GUILayout.Button("添加", GUILayout.Height(bh)))
                OnAddButtonClick();
        }

        private void OnAddButtonClick()
        {
            for(int i = 0; i < _checkList.Length; ++i)
            {
                if(_checkList[i])
                {
                    Debug.Log(_whiteList[i]);
                    // 将_whiteList[i]添加到_saveWhiteList
                }
            }
            this.Close();
        }

        private void GetAddWhiteListFiles(string path, ref List<string> files)
        {
            foreach(string f in Directory.GetFiles(path, "*.png"))
                files.Add(f);
            
            foreach(string d in Directory.GetDirectories(path))
                GetAddWhiteListFiles(d, ref files);
            
        }

        public void OpenAddWhiteListFolder()
        {
            string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
            if(path == "")
            {
                Debug.LogWarning("请选择文件夹");
                return;
            }
            GetAddWhiteListFiles(path, ref _whiteList);
            _checkList = new bool[_whiteList.Count];
            for(int i = 0; i < _whiteList.Count; ++i)
                _checkList[i] = false;
        }

        public void SetSaveWhiteList(ref List<string> saveWhiteList)
        {
            _saveWhiteList = saveWhiteList;
        }
    }

    // 白名单窗口
    public class WhiteListWindow: EditorWindow
    {
        bool[] _checkList;
        List<string> _whiteList;
        
        Vector2 _scrollPos;

        void OnGUI()
        {
            using (var v = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
                {
                    _scrollPos = scrollView.scrollPosition;
                    for(int i = 0; i < _whiteList.Count; ++i)
                        _checkList[i] = EditorGUILayout.ToggleLeft(_whiteList[i], _checkList[i]);
                }
            }

            float w = maxSize[0];
            float h = maxSize[1];
            float bw = w / 3;
            int bh = 30;

            GUILayout.BeginArea(new Rect(0, h-bh, bw, bh));
            if(GUILayout.Button("添加", GUILayout.Width(bw), GUILayout.Height(bh)))
                OnAddButtonClick(ref _whiteList);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(bw, h-bh, bw, bh));
            if(GUILayout.Button("删除", GUILayout.Width(bw), GUILayout.Height(bh)))
                OnDeleteButtonClick(ref _whiteList);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(2*bw, h-bh, bw, bh));
            if(GUILayout.Button("保存", GUILayout.Width(bw), GUILayout.Height(bh)))
                OnSaveButtonClick(ref _whiteList);
            GUILayout.EndArea();
        }

        private void OnAddButtonClick(ref List<string> whiteList)
        {
            Debug.Log("点击添加按钮");
            this.Close();
        }

        private void OnDeleteButtonClick(ref List<string> whiteList)
        {
            Debug.Log("点击删除按钮");
        }

        private void OnSaveButtonClick(ref List<string> whiteList)
        {
            Debug.Log("点击保存按钮");
        }

        public void SetWhiteList(List<string> whiteList)
        {
            _whiteList = whiteList;
            _checkList = new bool[whiteList.Count];
            for(int i = 0; i < whiteList.Count; ++i)
                _checkList[i] = false;
        }
    }
}