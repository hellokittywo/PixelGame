
using LuaFramework;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using Scripts.Utils;

public class AssetBundlesAll : MonoBehaviour
{
    private static string ResourceName = Application.dataPath + "/Resources";
    private static string ApkName = Application.dataPath.Replace("Assets", "") + "Bow.apk";

    private static string FirstPackage = "FirstPackage";    //必须最先被打包进去的
    private static string SecondPackage = "SecondPackage";    //必须最先被打包进去的
    private static string DesDirectory = "C:/luaframework";

    [MenuItem("Custom Editor/Create AssetBunldes/Android")]
    public static void CreateAssetBundlesAndroid()
    {
        CreateAssetBunldesALL(BuildTarget.Android);
        Packager.BuildFileIndex();
    }

    [MenuItem("Custom Editor/Create AssetBunldes/iPhone")]
    public static void CreateAssetBundlesIphone()
    {
        CreateAssetBunldesALL(BuildTarget.iOS);
        Packager.BuildFileIndex();
    }

    [MenuItem("Custom Editor/Tools/AssetBundleCollectDependencies")]
    public static void AssetBundleCollectDependencies()
    {
        Caching.CleanCache();

        List<Object> prefabList = new List<Object>();

        string prfabsPath = Application.dataPath.ToLower() + "/Resources/Prefabs";
        List<DirectoryInfo> list = new List<DirectoryInfo>();
        DirectoryInfo dir = new DirectoryInfo(prfabsPath);
        GetEndDirectories(dir, list);

        string prefabName = "";
        for (int i = 0; i < list.Count; i++)
        {
            DirectoryInfo folder = new DirectoryInfo(list[i].FullName);
            foreach (FileInfo file in folder.GetFiles("*.prefab"))
            {
                prefabName = file.FullName.Substring(file.FullName.IndexOf("assets"));
                prefabName = prefabName.Replace("\\", "/");
                prefabList.Add(AssetDatabase.LoadMainAssetAtPath(prefabName));
            }
        }

        Dictionary<string, List<string>> dependsDic = new Dictionary<string, List<string>>();
        for (int i = 0; i < prefabList.Count; i++)
        {
            Object[] dependenciesObj = EditorUtility.CollectDependencies(new UnityEngine.Object[] { prefabList[i] });
            List<string> temp = new List<string>();
            dependsDic.Add(prefabList[i].name, temp);
            //LoadingView这个是打的完整包，不需要依赖
            if (prefabList[i].name == "LoadingView")
            {
                continue;
            }
            for (int j = 0; j < dependenciesObj.Length; j++)
            {
                if (!dependenciesObj[j])
                {
                    continue;
                }
                if (dependenciesObj[j].ToString().Contains("(UIAtlas)"))
                {
                    temp.Add(dependenciesObj[j].ToString().Replace(" (UIAtlas)", ""));
                }
            }
        }
        string result = "AssetBundlesDependenciesTable = {\n";
        foreach (var item in dependsDic)
        {
            List<string> dependsList = item.Value;
            if(dependsList.Count == 0)
            {
                continue;
            }
            result += "    " + item.Key + " = ";
            string str = "";
            str += '"';
            for (int i = 0; i < dependsList.Count; i++)
			{
                if(i == dependsList.Count - 1)
                {
                    str += dependsList[i];
                }
                else
                {
                    str += dependsList[i] + ",";
                }
			}
            result += str + '"' + ",\n";
        }
        result += "}";
        File.WriteAllText(Application.dataPath + "/LuaFramework/Lua/Table/AssetBundlesDependenciesTable.lua", result);
        Debug.Log("生成assetbundles依赖完成！");
    }


    public static void CreateAssetBunldesALL(BuildTarget target)
    {
        Caching.CleanCache();

        Dictionary<string, Object> prefabDic = new Dictionary<string, Object>();
        Dictionary<string, Object> hotPrefabDic = new Dictionary<string, Object>();    //里面只有LoadingView一个

        string prfabsPath = Application.dataPath.ToLower() + "/Resources/Prefabs";
        List<DirectoryInfo> list = new List<DirectoryInfo>();
        DirectoryInfo dir = new DirectoryInfo(prfabsPath);

        string firstPackage = Application.dataPath.ToLower() + "/Resources/Prefabs/" + FirstPackage;
        string secondPackage = Application.dataPath.ToLower() + "/Resources/Prefabs/" + SecondPackage;

        list.Add(new DirectoryInfo(firstPackage));
        list.Add(new DirectoryInfo(secondPackage));
        GetEndDirectories(dir, list);
        string tPath = Application.dataPath.ToLower() + "/StreamingAssets/";
        string prefabName = "";
        for (int i = 0; i < list.Count; i++)
        {
            DirectoryInfo folder = new DirectoryInfo(list[i].FullName);
            foreach (FileInfo file in folder.GetFiles("*.prefab"))
            {
                prefabName = file.FullName.Substring(file.FullName.IndexOf("assets"));
                prefabName = prefabName.Replace("\\", "/");
                string name = prefabName.Substring(prefabName.IndexOf("Prefabs/") + 8);
                if (name.IndexOf("/") > 0)
                {
                    name = name.Substring(0, name.IndexOf("/"));
                }
                else
                {
                    name = "res";
                }
                if (Directory.Exists(tPath + name) == true)
                {
                    Directory.Delete(tPath + name, true);
                }
                Directory.CreateDirectory(tPath + name);
                if (prefabName.Contains("LoadingView"))
                {
                    hotPrefabDic.Add(prefabName, AssetDatabase.LoadMainAssetAtPath(prefabName));
                }
                else
                {
                    prefabDic.Add(prefabName, AssetDatabase.LoadMainAssetAtPath(prefabName));
                }
            }
        }
        //打AssetBundle包
        BuildAssetBundle(prefabDic, target);
        if (Directory.Exists(tPath + "Load") == true)
        {
            Directory.Delete(tPath + "Load", true);
        }
        Directory.CreateDirectory(tPath + "Load");
        BuildAssetBundle(hotPrefabDic, target);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 打AssetBundle包
    /// </summary>
    static void BuildAssetBundle(Dictionary<string, Object> prefabDic, BuildTarget target)
    {
        //遍历所有的游戏对象
        BuildPipeline.PushAssetDependencies();
        List<string> zipList = new List<string>();
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.UncompressedAssetBundle 
            | BuildAssetBundleOptions.DeterministicAssetBundle;
        Dictionary<string, string> zipDic = new Dictionary<string, string>();
        foreach (var item in prefabDic)
        {
            string name = item.Key.Substring(item.Key.IndexOf("Prefabs/") + 8);
            if (name.IndexOf("/") > 0)
            {
                name = name.Substring(0, name.IndexOf("/"));
            }
            else
            {
                name = "res";
            }
            if (zipDic.ContainsKey(name) == false)
            {
                zipDic.Add(name, name);
            }
            Object obj = item.Value;
            string targetPath = Application.dataPath.ToLower() + "/StreamingAssets/" + name + "/" + obj.name + AppConst.ExtName;
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, options, target))
            {
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        string tPath = Application.dataPath.ToLower() + "/StreamingAssets/";
        foreach (var item in zipDic)
        {
            string[] files = Directory.GetFiles(tPath + item.Key, "*.meta", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            Util.CompressFileFastZip(tPath + item.Key + AppConst.ZipName, tPath + item.Key);
            Directory.Delete(tPath + item.Key, true);
        }
//        for (int i = 0; i < prefabList.Count; i++)
//        {
//            Object obj = prefabList[i];
//            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
//            //StreamingAssets是只读路径，不能写入
//            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
//            string targetPath = Application.dataPath.ToLower() + "/StreamingAssets/" + obj.name + AppConst.ExtName;

//            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, options, target))
//            {
////                 Debug.Log(obj.name + "资源打包成功");
//            }
//            else
//            {
//                Debug.Log(obj.name + "资源打包失败");
//            }
//            zipList.Add(targetPath);
////             File.Copy(targetPath, DesDirectory + "/" + obj.name + AppConst.ExtName, true);
//        }
//        for (int j = 0; j < zipList.Count; j++)
//        {
//            if (Application.platform == RuntimePlatform.WindowsEditor)
//            {
//                Util.CompressFileLZMA(zipList[j], zipList[j].Replace(AppConst.ExtName, AppConst.ZipName));
//            }
//            else if (Application.platform == RuntimePlatform.OSXEditor)
//            {
//                Util.CompressFileGZip(zipList[j], zipList[j].Replace(AppConst.ExtName, AppConst.ZipName));
//            }
//            File.Delete(zipList[j]);
//        }
        BuildPipeline.PopAssetDependencies();
    }

    /// <summary>
    /// 查找指定目录下的所有末级子目录
    /// </summary>
    /// <param name="dir">要查找的目录</param>
    /// <param name="list">查找结果列表</param>
    /// <param name="system">是否包含系统目录</param>
    /// <param name="hidden">是否包含隐藏目录</param>
    static void GetEndDirectories(DirectoryInfo dir, List<DirectoryInfo> list, bool system = false, bool hidden = false)
    {
        DirectoryInfo[] sub = dir.GetDirectories();

        if (dir.FullName.IndexOf(FirstPackage) == -1 && dir.FullName.IndexOf(SecondPackage) == -1)
        {
            list.Add(dir);
        }
        if (sub.Length == 0)
        {// 没有子目录了
            return;
        }

        foreach (DirectoryInfo subDir in sub)
        {
            // 跳过系统目录
            if (!system && (subDir.Attributes & FileAttributes.System) == FileAttributes.System)
                continue;
            // 跳过隐藏目录
            if (!hidden && (subDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                continue;

            GetEndDirectories(subDir, list);
        }
    }

    //[MenuItem("Custom Editor/HotUpdate/All")]
    public static void HotUpdateAll(string resName = "resupdate1")
    {
        AssetLuaCode.HotUpdate();
        HotUpdateResources(resName, false);
    }

    //[MenuItem("Custom Editor/HotUpdate/Resources")]
    public static void HotUpdateResources(string resName = "resupdate1", bool clear = true)
    {
        string path = Application.dataPath.Replace("Assets", "") + "hotupdate";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists(path + "/diifer_prefabs.txt"))
        {
            Debug.LogError("没有找到diifer_prefabs.txt！！");
            return;
        }
        if (!File.Exists(path + "/diifer_atlas.txt"))
        {
            Debug.LogError("没有找到diifer_atlas.txt！！");
            return;
        }
        if (clear == true)
        {
            if (Directory.Exists(path + "/lua"))
            {
                Directory.Delete(path + "/lua", true);
            }
            string[] deletefiles = Directory.GetFiles(path, "*.zip", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < deletefiles.Length; j++)
            {
                File.Delete(deletefiles[j]);
            }
        }
        Dictionary<string, int> updateResDic = new Dictionary<string, int>();
        string filesText = UnityUtils.ReadLocalTxt(path + "/diifer_prefabs.txt") + "\n" + UnityUtils.ReadLocalTxt(path + "/diifer_atlas.txt");
        string[] files = filesText.Split('\n');
        for (int i = 0; i < files.Length; i++)
        {
            if(files[i].Trim() != "")
            {
                string name = files[i].Replace("M       ", "");
                //name = name.Replace("\\", "/");
                name = name.Replace("\r", "");
                name = GetResPath(name);
                updateResDic.Add(name, 1);
            }
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            HotUpdateAssetBunldesALL(BuildTarget.Android, updateResDic, resName);
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            HotUpdateAssetBunldesALL(BuildTarget.iOS, updateResDic, resName);
        }
    }

    private static string GetResPath(string path)
    {
        string match = "trunk\\Assets\\";
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            path = path.Replace("\\", "/");
            match = "trunk/Assets/";
        }
        string name = path.Substring(path.IndexOf(match));
        name = name.Replace(match, "");
        return name;
    }

    public static void HotUpdateAssetBunldesALL(BuildTarget target, Dictionary<string, int> updateResDic, string resName)
    {
        Caching.CleanCache();
        List<Object> prefabList = new List<Object>();
        List<Object> hotPrefabList = new List<Object>();

        string prfabsPath = Application.dataPath + "/Resources/Prefabs";
        List<DirectoryInfo> list = new List<DirectoryInfo>();
        DirectoryInfo dir = new DirectoryInfo(prfabsPath);

        string firstPackage = Application.dataPath + "/Resources/Prefabs/" + FirstPackage;
        string secondPackage = Application.dataPath + "/Resources/Prefabs/" + SecondPackage;

        list.Add(new DirectoryInfo(firstPackage));
        list.Add(new DirectoryInfo(secondPackage));
        GetEndDirectories(dir, list);
        string prefabName = "";
        for (int i = 0; i < list.Count; i++)
        {
            DirectoryInfo folder = new DirectoryInfo(list[i].FullName);
            foreach (FileInfo file in folder.GetFiles("*.prefab"))
            {
                if (file.FullName.IndexOf(FirstPackage) > 0 || file.FullName.IndexOf(SecondPackage) > 0 ||
                    updateResDic.ContainsKey(GetResPath(file.FullName)))
                {
                    prefabName = file.FullName.Substring(file.FullName.IndexOf("Assets"));
                    prefabName = prefabName.Replace("\\", "/");
                    if (prefabName.Contains("LoadingView"))
                    {
                        hotPrefabList.Add(AssetDatabase.LoadMainAssetAtPath(prefabName));
                    }
                    else
                    {
                        prefabList.Add(AssetDatabase.LoadMainAssetAtPath(prefabName));
                    }
                }
            }
        }
        string tPath = Application.dataPath + "/StreamingAssets/HotupdateResources";
        if (Directory.Exists(tPath))
        {
            Directory.Delete(tPath, true);
        }
        Directory.CreateDirectory(tPath);
        ////打AssetBundle包
        HotUpdateAssetBundle(prefabList, target);
        HotUpdateAssetBundle(hotPrefabList, target);

        string str = "\\";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            str = "\\";
        }
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            str = "/";
        }
        DirectoryInfo folders = new DirectoryInfo(tPath);
        foreach (FileInfo file in folders.GetFiles("*"))
        {
            bool dele = true;
            if(file.FullName.EndsWith(".meta") == false)
            {
                foreach (var item in updateResDic)
                {
                    string tar = file.FullName.Substring(file.FullName.LastIndexOf(str) + 1);
                    tar = tar.Replace(AppConst.ExtName, "");
                    if (item.Key.IndexOf(tar) >= 0)
                    {
                        dele = false;
                        break;
                    }
                }
            }
            if(dele == true)
            {
                File.Delete(file.FullName);
            }
        }
        Util.CompressFileFastZip(Application.dataPath.Replace("Assets", "") + "hotupdate/" + resName + AppConst.ZipName, tPath);
        Directory.Delete(tPath, true);
        AssetDatabase.Refresh();
    }

    static void HotUpdateAssetBundle(List<Object> prefabList, BuildTarget target)
    {
        if(prefabList.Count == 0)
        {
            return;
        }
        //遍历所有的游戏对象
        BuildPipeline.PushAssetDependencies();
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.UncompressedAssetBundle
            | BuildAssetBundleOptions.DeterministicAssetBundle;
        string tPath = Application.dataPath + "/StreamingAssets/HotupdateResources/";
        for (int i = 0; i < prefabList.Count; i++)
        {
            Object obj = prefabList[i];
            string targetPath = tPath + obj.name + AppConst.ExtName;
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, options, target))
            {
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        BuildPipeline.PopAssetDependencies();
    }
}
