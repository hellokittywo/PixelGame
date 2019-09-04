using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LoadingView : EditorWindow
{
    private Rect mPathRect = new Rect(40, 18f, 1000, 40f);

    private Rect mLastFilesRect = new Rect(40, 73, 100, 40f);
    private Rect mLastFilesPathInputRect = new Rect(190f, 68, 330f, 24f);
    private Rect mLastFilesPathSetPathBtnRect = new Rect(580f, 68, 100f, 24f);
    private Rect mLastFilesPathOpenPathBtnRect = new Rect(700f, 68, 100f, 24f);
    private string mLastFilesPathText;

    private Rect mHotupdatePathRect = new Rect(40, 123, 100, 40f);
    private Rect mHotupdatePathInputRect = new Rect(190f, 118, 330f, 24f);
    private Rect mHotupdatePathSetPathBtnRect = new Rect(580f, 118, 100f, 24f);
    private Rect mHotupdatePathOpenPathBtnRect = new Rect(700f, 118, 100f, 24f);
    private string mHotupdatePathText;

    private Rect mPrefabPathRect = new Rect(40, 173, 300, 40f);
    private Rect mPrefabPathInputRect1 = new Rect(330f, 168, 100, 24f);
    private Rect mPrefabPathInputRect2 = new Rect(480f, 168, 100, 24f);
    private string mPrefabPathText1;
    private string mPrefabPathText2;

    private Rect mAtlasPathRect = new Rect(40, 223, 300, 40f);
    private Rect mAtlasPathInputRect1 = new Rect(330f, 218, 100, 24f);
    private Rect mAtlasPathInputRect2 = new Rect(480f, 218, 100, 24f);
    private string mAtlasPathText1;
    private string mAtlasPathText2;

    private Rect mHotupteLuaBtnRect = new Rect(250, 350, 100f, 24f);
    private Rect mSvnDifferBtnRect = new Rect(100, 350, 100f, 24f);
    private Rect mHotupdateResBtnRect = new Rect(400, 350, 100f, 24f);
    private Rect mHotupdateAllBtnRect = new Rect(550, 350, 100f, 24f);

    private Rect mHotupdateResNameRect = new Rect(240, 270, 100f, 24f);
    private string mHotupdateResNameText;

    public void OnEnable()
    {
        mLastFilesPathText = PlayerPrefs.GetString("lasthotupdatePath");
        mHotupdatePathText = PlayerPrefs.GetString("hotupdatePath");
        mPrefabPathText1 = PlayerPrefs.GetString("prefabPath1");
        mPrefabPathText2 = PlayerPrefs.GetString("prefabPath2");
        mAtlasPathText1 = PlayerPrefs.GetString("atlasPath1");
        mAtlasPathText2 = PlayerPrefs.GetString("atlasPath2");
        mHotupdateResNameText = "resupdate1";
    }

    public void OnGUI()
    {
        //GUIStyle style = new GUIStyle();
        //style.fontSize = 12;
        //style.normal.textColor = new Color(1, 1, 1, 1);
        GUI.Label(mPathRect, "热更新路径：" + UnityEngine.Application.dataPath.Replace("Assets", "") + "hotupdate");
        if (GUI.Button(new Rect(700f, 13f, 100f, 24f), "打开目录"))
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = (UnityEngine.Application.dataPath.Replace("Assets", "") + "hotupdate").Replace("/", "\\");
            System.Diagnostics.Process.Start(psi);
        }
        
        GUI.Label(mLastFilesRect, "上个版本files目录：");
        mLastFilesPathText = EditorGUI.TextField(mLastFilesPathInputRect, mLastFilesPathText);

        GUI.Label(mHotupdatePathRect, "热更新目录：");
        mHotupdatePathText = EditorGUI.TextField(mHotupdatePathInputRect, mHotupdatePathText);

        GUI.Label(mPrefabPathRect, "prefab版本(要包括当前版本必须输入前一个版本)：");
        mPrefabPathText1 = EditorGUI.TextField(mPrefabPathInputRect1, mPrefabPathText1);
        mPrefabPathText2 = EditorGUI.TextField(mPrefabPathInputRect2, mPrefabPathText2);

        GUI.Label(mAtlasPathRect, "atlas版本(要包括当前版本必须输入前一个版本)：");
        mAtlasPathText1 = EditorGUI.TextField(mAtlasPathInputRect1, mAtlasPathText1);
        mAtlasPathText2 = EditorGUI.TextField(mAtlasPathInputRect2, mAtlasPathText2);

        if (GUI.Button(mLastFilesPathSetPathBtnRect, "设置目录"))
        {
            string tee = EditorUtility.OpenFolderPanel("选择上个版本files目录", "", "");
            if(!tee.Equals(""))
            {
                mLastFilesPathText = tee;
            }
        }
            
        if (GUI.Button(mLastFilesPathOpenPathBtnRect, "打开目录"))
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = mLastFilesPathText.Replace("/", "\\");
            System.Diagnostics.Process.Start(psi);
        }

        if (GUI.Button(mHotupdatePathSetPathBtnRect, "设置目录"))
        {
            string tee = EditorUtility.OpenFolderPanel("选择热更新目录", "", "");
            if (!tee.Equals(""))
            {
                mHotupdatePathText = tee;
            }
        }
        if (GUI.Button(mHotupdatePathOpenPathBtnRect, "打开目录"))
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = mHotupdatePathText.Replace("/", "\\");
            System.Diagnostics.Process.Start(psi);
        }

        if (Application.platform == RuntimePlatform.WindowsEditor && GUI.Button(mSvnDifferBtnRect, "导出SVN差异文件"))
        {
            if(mPrefabPathText1 == "" || mPrefabPathText2 == "" || mAtlasPathText1 == "" || mAtlasPathText2 == "")
            {
                UnityEngine.Debug.LogError("SVN版本不能为空！");
                return;
            }
            string targetDir = string.Format((UnityEngine.Application.dataPath + "/Editor").Replace("/", "\\"));//this is where mybatch.bat lies
            Process proc = new Process();
            proc.StartInfo.WorkingDirectory = targetDir;
            proc.StartInfo.FileName = "ResHotupdate.bat";
            string str = mPrefabPathText1 + " " + mPrefabPathText2 + " " + mAtlasPathText1 +
                " " + mAtlasPathText2 + " " + UnityEngine.Application.dataPath.Replace("/", "\\");
            proc.StartInfo.Arguments = string.Format(str);//this is argument
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();
        }
        if (GUI.Button(mHotupteLuaBtnRect, "热更新代码"))
        {
            AssetLuaCode.HotUpdate();
            AssetDatabase.Refresh();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                CreateFiles();
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                UnityEngine.Debug.Log("去windows生成files！");
            }
        }
        if (GUI.Button(mHotupdateResBtnRect, "热更新资源"))
        {
            AssetBundlesAll.HotUpdateResources(mHotupdateResNameText);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                CreateFiles();
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                UnityEngine.Debug.Log("去windows生成files！");
            }
        }
        if (GUI.Button(mHotupdateAllBtnRect, "热更新代码和资源"))
        {
            AssetBundlesAll.HotUpdateAll(mHotupdateResNameText);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                CreateFiles();
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                UnityEngine.Debug.Log("去windows生成files！");
            }

        }
        GUI.Label(new Rect(40, 275, 700f, 24f), "资源zip名字（resupdate1）：                                 " +
                  "       (资源太多可以填resupdate2/3/4/5/6/7.zip等等)");
        mHotupdateResNameText = EditorGUI.TextField(mHotupdateResNameRect, mHotupdateResNameText);

        if (GUI.Button(new Rect(700f, 350f, 100f, 24f), "生成files"))
        {
            CreateFiles();
        }
    }

    private void CreateFiles()
    {
        string hotupdatePath = mHotupdatePathText;
        if (Directory.Exists(hotupdatePath))
        {
            Directory.Delete(hotupdatePath, true);
        }
        Directory.CreateDirectory(hotupdatePath);
        if (!Directory.Exists(hotupdatePath))
        {
            UnityEngine.Debug.LogError("找不到热更新目录!");
            return;
        }
        if (mLastFilesPathText == "" || !Directory.Exists(mLastFilesPathText))
        {
            UnityEngine.Debug.LogError("请设置上个版本files目录!");
            return;
        }
        if (!File.Exists(mLastFilesPathText + "/" + "files.txt"))
        {
            UnityEngine.Debug.LogError("找不到上个版本files目录!");
            return;
        }
        string filesName = hotupdatePath + "/" + "files.txt";
        File.Copy(mLastFilesPathText + "/" + "files.txt", filesName, true);
        if (!File.Exists(filesName))
        {
            UnityEngine.Debug.LogError("没有找到上个版本的files.txt!");
            return;
        }
        string path = UnityEngine.Application.dataPath.Replace("/", "\\").Replace("Assets", "hotupdate");
        if (Directory.Exists(path + "/lua"))
        {
            if (Directory.Exists(hotupdatePath + "/lua"))
            {
                Directory.Delete(hotupdatePath + "/lua", true);
            }
            Directory.CreateDirectory(hotupdatePath + "/lua");
            string[] files = Directory.GetFiles(path + "/lua");
            foreach (string formFileName in files)
            {
                string fileName = Path.GetFileName(formFileName);
                string toFileName = Path.Combine(hotupdatePath + "/lua", fileName);
                File.Copy(formFileName, toFileName);
            }
        }
        string[] res = Directory.GetFiles(path, "*.zip");
        foreach (string formFileName in res)
        {
            string fileName = Path.GetFileName(formFileName);
            string toFileName = Path.Combine(hotupdatePath, fileName);
            File.Copy(formFileName, toFileName);
        }

        Dictionary<string, string> oldMd5Dic = new Dictionary<string, string>();
        string result = "";
        string[] contents = File.ReadAllText(filesName).Split('\n');
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i].Trim() == "")
            {
                continue;
            }
            string[] temp = contents[i].Split('|');
            if (temp.Length < 2)
            {
                continue;
            }
            string name = temp[0];
            if (name == "Zlast.unity3d")
            {
                continue;
            }
            string[] sizeL = temp[1].Split(',');
            string md5 = sizeL[0].Trim();
            oldMd5Dic.Add(name, md5);
            if (File.Exists(hotupdatePath + "/" + name))
            {
                FileInfo info = new FileInfo(hotupdatePath + "/" + name);
                result += name + "|" + System.DateTime.Now.ToString("yyyymmddhhmmss") + "," + Math.Floor((double)info.Length / 1024) + "\n";
            }
            else
            {
                result += name + "|" + md5 + "," + sizeL[1].Trim() + "\n";
            }
        }
        List<string> list = new List<string>();
        FindAllFiles(new DirectoryInfo(hotupdatePath), list);
        for (int i = 0; i < list.Count; i++)
        {
            if (!oldMd5Dic.ContainsKey(list[i]))
            {
                FileInfo info = new FileInfo(hotupdatePath + "/" + list[i]);
                result += list[i] + "|" + System.DateTime.Now.ToString("yyyymmddhhmmss") + "," + Math.Floor((double)info.Length / 1024) + "\n";
            }
        }
        result += "Zlast.unity3d" + "|" + System.DateTime.Now.ToString("yyyymmddhhmmss") + "," + 1 + "\n";
        File.Delete(filesName);
        File.WriteAllText(filesName, result);
        UnityEngine.Debug.Log("生成完成!");
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
        psi.Arguments = mHotupdatePathText.Replace("/", "\\");
        System.Diagnostics.Process.Start(psi);
    }

    private void FindAllFiles(DirectoryInfo di, List<string> list)
    {
        FileInfo[] fis = di.GetFiles();
        for (int i = 0; i < fis.Length; i++)
        {
            FileInfo info = fis[i];
            if (info.Name.EndsWith(".txt"))
            {
                continue;
            }
            list.Add(info.Name.StartsWith("lua") ? "lua/" + info.Name : info.Name);
        }
        DirectoryInfo[] dis = di.GetDirectories();
        for (int j = 0; j < dis.Length; j++)
        {
            FindAllFiles(dis[j], list);
        }
    }

    [MenuItem("Custom Editor/Tools/Hotupdate")]
    public static void ShowWindow()
    {
        LoadingView window = EditorWindow.GetWindow<LoadingView>(true, "Hotupdate");
        window.minSize = window.maxSize = new Vector2(820f, 520f);
        var position = window.position;
        position.center = new Rect(0f, 0f, UnityEngine.Screen.currentResolution.width, UnityEngine.Screen.currentResolution.height).center;
        window.position = position;
        UnityEngine.Object.DontDestroyOnLoad(window);
    }

    public void OnDestroy()
    {
        if (mLastFilesPathText.Trim() != "" && mLastFilesPathText.Trim().Equals(PlayerPrefs.GetString("lasthotupdatePath")) == false)
        {
            PlayerPrefs.SetString("lasthotupdatePath", mLastFilesPathText);
        }
        if (mHotupdatePathText.Trim() != "" && mHotupdatePathText.Trim().Equals(PlayerPrefs.GetString("hotupdatePath")) == false)
        {
            PlayerPrefs.SetString("hotupdatePath", mHotupdatePathText);
        }
        if (mPrefabPathText1.Trim() != "" && mPrefabPathText1.Trim().Equals(PlayerPrefs.GetString("prefabPath1")) == false)
        {
            PlayerPrefs.SetString("prefabPath1", mPrefabPathText1);
        }
        if (mPrefabPathText2.Trim() != "" && mPrefabPathText2.Trim().Equals(PlayerPrefs.GetString("prefabPath2")) == false)
        {
            PlayerPrefs.SetString("prefabPath2", mPrefabPathText2);
        }
        if (mAtlasPathText1.Trim() != "" && mAtlasPathText1.Trim().Equals(PlayerPrefs.GetString("atlasPath1")) == false)
        {
            PlayerPrefs.SetString("atlasPath1", mAtlasPathText1);
        }
        if (mAtlasPathText2.Trim() != "" && mAtlasPathText2.Trim().Equals(PlayerPrefs.GetString("atlasPath2")) == false)
        {
            PlayerPrefs.SetString("atlasPath2", mAtlasPathText2);
        }
    }
}


