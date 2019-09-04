using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LuaFramework;
using Scripts.Utils;

public class AssetLuaCode
{
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    [MenuItem("Custom Editor/Tools/Build Google Project")]
    static public void BuildAssetBundles()
    {
        BuildPipeline.BuildPlayer(new string[] { "Assets/Untitled.unity" }, Application.dataPath + "/../", BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    private static bool buildForWindows = false;
    [MenuItem("Custom Editor/Lua/Build Lua Code For Windows")]
    public static void BuildAssetResourceForWindows()
    {
        buildForWindows = true;
        string streamPath = Application.streamingAssetsPath + "/lua";
        if (Directory.Exists(streamPath))
        {
            Directory.Delete(streamPath, true);
        }

        BuildAssetLuaCode();

        BuildFileIndex();
        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Editor/Lua/Build Lua Code")]
    public static void BuildAssetResource()
    {
        string streamPath = Application.streamingAssetsPath + "/lua";
        if (Directory.Exists(streamPath))
        {
            Directory.Delete(streamPath, true);
        }

        BuildAssetLuaCode();

        BuildFileIndex();
        AssetDatabase.Refresh();
    }

    //[MenuItem("Custom Editor/HotUpdate/Lua")]
    public static void HotUpdate()
    {
        string streamPath = Application.streamingAssetsPath + "/luaTemp";
        if (Directory.Exists(streamPath))
        {
            Directory.Delete(streamPath, true);
        }
        BuildHotUpdateLuaAsset();
        string path = Application.dataPath.Replace("Assets", "") + "hotupdate";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (Directory.Exists(path + "/lua"))
        {
            Directory.Delete(path + "/lua", true);
        }
        Directory.CreateDirectory(path + "/lua");
        string[] deletefiles = Directory.GetFiles(path, "*.zip", SearchOption.TopDirectoryOnly);
        for (int j = 0; j < deletefiles.Length; j++)
        {
            File.Delete(deletefiles[j]);
        }
        foreach (var item in m_md5DiffDic)
        {
            File.Move(AppDataPath + "/StreamingAssets/" + item.Key, path + "/lua/" + item.Key);
        }
        Directory.Delete(Application.streamingAssetsPath + "/luaTemp", true);
    }
    private static Dictionary<string, string> GetFilesMd5(string url)
    {
        string filesText = UnityUtils.ReadLocalTxt(url);
        Dictionary<string, string> md5Dic = new Dictionary<string, string>();

        string[] files = filesText.Split('\n');

        for (int i = 0; i < files.Length; i++)
        {
            string[] keyValue = files[i].Split('|');
            if (keyValue.Length == 2)
            {
                md5Dic.Add(keyValue[0], keyValue[1].Trim());
            }
        }
        return md5Dic;
    }
    public static void BuildAssetLuaCode()
    {
        //先复制一份AccelerateConstrctionView.unity3d，最后写MD5码到files32.txt的最后，这是防止热更新最后一个文件下载不了(WebClient的bug？)
        //if (!File.Exists(Application.streamingAssetsPath + "/Zlast.unity3d"))
        //{
        //    File.Copy(Application.streamingAssetsPath + "/AccelerateConstrctionView.unity3d", Application.streamingAssetsPath + "/Zlast.unity3d", true);
        //}
//         if (AppConst.LuaBundleMode)
        {
            BuildLuaBundles();
        }
//         else
//         {
//             HandleLuaFile();
//         }
    }

    static void ClearAllLuaFiles()
    {
        string osPath = Application.streamingAssetsPath + "/" + LuaConst.osDir;

        if (Directory.Exists(osPath))
        {
            string[] files = Directory.GetFiles(osPath, "Lua*.zip");

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        string path = osPath + "/Lua";

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        path = Application.dataPath + "/Resources/Lua";

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        path = Application.persistentDataPath + "/" + LuaConst.osDir + "/Lua";

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    static void CreateStreamDir(string dir)
    {
        dir = Application.streamingAssetsPath + "/" + dir;

        if (!File.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true)
    {
        if (!Directory.Exists(sourceDir))
        {
            return;
        }

        string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
        int len = sourceDir.Length;

        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
        {
            --len;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string str = files[i].Remove(0, len);
            string dest = destDir + str;
            if (appendext) dest += ".bytes";
            string dir = Path.GetDirectoryName(dest);
            Directory.CreateDirectory(dir);

            if (AppConst.LuaByteMode)
            {
                Packager.EncodeLuaFile(files[i], dest, buildForWindows);
            }
            else
            {
                File.Copy(files[i], dest, true);
            }
        }
    }

    [MenuItem("Custom Editor/Tools/Build Base md5 File")]
    public static void BuildBaseMd5File()
    {
        //生成之前先打包所有资源assetbundle
        //读取基准版本assetbundle的md5信息
        if (File.Exists(m_baseMd5Str))
        {
            File.Delete(m_baseMd5Str);
        }
        FileStream fs = new FileStream(m_baseMd5Str, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        //string filesText = UnityUtils.ReadLocalTxt(Application.streamingAssetsPath + "/files.txt");
        //string[] luafiles = filesText.Split('\n');
        //for (int k = 0; k < luafiles.Length; k++)
        //{
        //    string[] keyValue = luafiles[k].Split('|');
        //    if (keyValue.Length == 2)
        //    {
        //        sw.WriteLine(keyValue[0] + "," + keyValue[0] + "|" + keyValue[1].Trim());
        //    }
        //}

        //lua代码每次重启unity打包的assetbundle的MD5都不一致，这里根据单个lua文件判断，
        //只要打成assetbundle的所有lua文件的MD5没变，assetbundle的MD5也不变
        string streamDir = Application.dataPath + "/lua_md5";
        if (Directory.Exists(streamDir))
        {
            Directory.Delete(streamDir, true);
        }
        CopyLuaBytesFiles(CustomSettings.luaDir, streamDir);
        CopyLuaBytesFiles(CustomSettings.FrameworkPath + "/ToLua/Lua", streamDir);

        AssetDatabase.Refresh();
        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.TopDirectoryOnly);
        List<string> list = new List<string>(dirs);
        list.Add(streamDir);
        dirs = list.ToArray();
        for (int i = 0; i < dirs.Length; i++)
        {
            string dir = dirs[i].Remove(0, streamDir.Length);
            string[] files = Directory.GetFiles("Assets/lua_md5" + dir, "*.lua.bytes", SearchOption.AllDirectories);
            string bundleName = "lua" + AppConst.ExtName;
            if (dir != null)
            {
                dir = dir.Replace('\\', '_').Replace('/', '_');
                bundleName = "lua" + dir.ToLower() + AppConst.ExtName;
            }
            for (int j = 0; j < files.Length; j++)
            {
                string md5 = Util.md5file(files[j]);
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    sw.WriteLine(files[j].Replace("Assets/lua_md5/", "") + "|" + md5);
                }
                else
                {
                    sw.WriteLine(files[j].Replace("Assets/lua_md5\\", "") + "|" + md5);
                }
            }
        }
        sw.Close(); fs.Close();
        if (Directory.Exists(streamDir))
        {
            Directory.Delete(streamDir, true);
        }
    }

    private static Dictionary<string, string> m_baseMd5Dic = new Dictionary<string, string>();
    private static Dictionary<string, int> m_md5DiffDic = new Dictionary<string, int>();
    private static string m_baseMd5Str = Application.dataPath.Replace("Assets", "") + "BasesMd5File.txt";
    private static void InitBaseMd5File()
    {
        m_baseMd5Dic = new Dictionary<string, string>();
        m_md5DiffDic = new Dictionary<string, int>();
        if (!File.Exists(m_baseMd5Str))
        {
            return;
        }
        string filesText = UnityUtils.ReadLocalTxt(m_baseMd5Str);
        string[] files = filesText.Split('\n');

        for (int i = 0; i < files.Length; i++)
        {
            if(files[i].Equals(""))
            {
                continue;
            }
            string[] names = files[i].Split('|');
            if (!m_baseMd5Dic.ContainsKey(names[0]))
            {
                m_baseMd5Dic.Add(names[0], names[1].TrimEnd("\r".ToCharArray()));
            }
        }
    }

    static void BuildHotUpdateLuaAsset()
    {
        InitBaseMd5File();
        ClearAllLuaFiles();
        CreateStreamDir("luaTemp/");

        string dirPath = Application.persistentDataPath;
        if (!File.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;
        CopyLuaBytesFiles(CustomSettings.luaDir, streamDir);
        CopyLuaBytesFiles(CustomSettings.FrameworkPath + "/ToLua/Lua", streamDir);

        AssetDatabase.Refresh();
        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.TopDirectoryOnly);

        for (int i = 0; i < dirs.Length; i++)
        {
            string dir = dirs[i].Remove(0, streamDir.Length);
            string[] files = Directory.GetFiles("Assets/lua/" + dir, "*.lua.bytes", SearchOption.AllDirectories);
            string bundleName = "lua" + AppConst.ExtName;
            if (dir != null)
            {
                dir = dir.Replace('\\', '_').Replace('/', '_');
                bundleName = "lua" + dir.ToLower() + AppConst.ExtName;
            }
            for (int j = 0; j < files.Length; j++)
            {
                string md5 = Util.md5file(files[j]);
                string name;
                name = files[j].Replace("Assets/lua/", "");
                if (!m_baseMd5Dic.ContainsKey(name) || m_baseMd5Dic[name] != md5)
                {
                    BuildHotUpdateLuaBundle(dir);
                    break;
                }
            }
        }
        string str = "\\";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            str = "\\";
        }
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            str = "/";
        }
        string[] luafiles = Directory.GetFiles("Assets/lua", "*.lua.bytes");
        for (int k = 0; k < luafiles.Length; k++)
        {
            string lua_md5 = Util.md5file(luafiles[k]);
            string lua_name;
            lua_name = luafiles[k].Replace("Assets/lua" + str, "");
            if (!m_baseMd5Dic.ContainsKey(lua_name) || m_baseMd5Dic[lua_name] != lua_md5)
            {
                BuildHotUpdateLuaBundle(null);
                break;
            }
        }
        string dPath = Application.dataPath.ToLower();
        string[] deletefiles = Directory.GetFiles(dPath + "/StreamingAssets/luaTemp", "*", SearchOption.AllDirectories);
        for (int j = 0; j < deletefiles.Length; j++)
        {
            if(deletefiles[j].EndsWith(".meta"))
            {
                File.Delete(deletefiles[j]);
            }
        }
        Util.CompressFileFastZip(dPath + "/StreamingAssets/luaupdate1" + AppConst.ZipName, dPath + "/StreamingAssets/luaTemp");
        m_md5DiffDic.Add("luaupdate1" + AppConst.ZipName, 1);
        Directory.Delete(streamDir, true);
    }

    static void BuildHotUpdateLuaBundle(string dir)
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets |
                                          BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle;
        string path = "Assets/" + AppConst.LuaTempDir + dir;
        string[] files = Directory.GetFiles(path, "*.lua.bytes", dir != null ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        List<Object> list = new List<Object>();
        string bundleName = "lua.unity3d";
        if (dir != null)
        {
            dir = dir.Replace('\\', '_').Replace('/', '_');
            bundleName = "lua_" + dir.ToLower() + AppConst.ExtName;
        }
        for (int i = 0; i < files.Length; i++)
        {
            string md5 = Util.md5file(files[i]);
            string luaname = files[i].Replace("Assets/lua/", "");
            string luastr = ("lua/" + bundleName).Replace(AppConst.ExtName, AppConst.ZipName);
            string zipLuastr = "luaTemp/" + bundleName.Replace(AppConst.ExtName, AppConst.ZipName);
            Object obj = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(obj);
        }

        if (files.Length > 0)
        {
            string output = Application.streamingAssetsPath + "/luaTemp/" + bundleName;
            if (File.Exists(output))
            {
                File.Delete(output);
            }
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), output, options, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }
    }

    static void BuildLuaBundles()
    {
        //if (!File.Exists(m_baseMd5Str))
        //{
        //    Debugger.LogError("请先生成lua代码的MD5文件，工具：Custom Editor/Build Base md5 File");
        //    return;
        //}
        InitBaseMd5File();
        ClearAllLuaFiles();
        CreateStreamDir(AppConst.LuaTempDir);

        string dir = Application.persistentDataPath;
        if (!File.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;
        CopyLuaBytesFiles(CustomSettings.luaDir, streamDir);
        CopyLuaBytesFiles(CustomSettings.toluaLuaDir, streamDir);

        AssetDatabase.Refresh();
        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.TopDirectoryOnly);

        for (int i = 0; i < dirs.Length; i++)
        {
            string str = dirs[i].Remove(0, streamDir.Length);
            BuildLuaBundle(str);
        }

        BuildLuaBundle(null);

        CompressFileToZip();

        Directory.Delete(streamDir, true);

//         AssetDatabase.Refresh();
    }

    private static void CompressFileToZip()
    {
        string dPath = Application.dataPath.ToLower();
        string[] files = Directory.GetFiles(dPath + "/StreamingAssets/lua", "*.meta", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        Util.CompressFileFastZip(dPath + "/StreamingAssets/lua" + AppConst.ZipName, dPath + "/StreamingAssets/lua");
        if (Directory.Exists(dPath + "/StreamingAssets/lua"))
        {
            Directory.Delete(dPath + "/StreamingAssets/lua", true);
        }
        Directory.CreateDirectory(dPath + "/StreamingAssets/lua");
        File.Move(dPath + "/StreamingAssets/lua" + AppConst.ZipName, dPath + "/StreamingAssets/lua/lua" + AppConst.ZipName);
        //Directory.Delete(Application.dataPath.ToLower() + "/StreamingAssets/lua", true);
        //foreach (FileInfo file in folder.GetFiles("*.unity3d"))
        //{
        //    if (file.Name.EndsWith(".meta") && !file.Name.StartsWith("lua/"))
        //    {
        //        continue;
        //    }
        //    if (Application.platform == RuntimePlatform.OSXEditor)
        //    {
        //        Util.CompressFileGZip(file.FullName, file.FullName.Replace(AppConst.ExtName, AppConst.ZipName));
        //    }
        //    else
        //    {
        //        Util.CompressFileLZMA(file.FullName, file.FullName.Replace(AppConst.ExtName, AppConst.ZipName));
        //    }
        //    File.Delete(file.FullName);
        //}
    }

    static string BuildLuaBundle(string dir)
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets |
                                          BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle;
        string path = "Assets/" + AppConst.LuaTempDir + dir;
        string[] files;
        if(dir == null)
        {
            files = Directory.GetFiles(path, "*.lua.bytes");
        }
        else
        {
            files = Directory.GetFiles(path, "*.lua.bytes", SearchOption.AllDirectories);
        }
        List<Object> list = new List<Object>();
        string bundleName = "lua.unity3d";
        if (dir != null)
        {
            dir = dir.Replace('\\', '_').Replace('/', '_');
            bundleName = "lua_" + dir.ToLower() + AppConst.ExtName;
        }
        for (int i = 0; i < files.Length; i++)
        {
            string md5 = Util.md5file(files[i]);
            string luaname = files[i].Replace("Assets/lua/", "");
            string luastr = "lua/" + bundleName;
            string zipLuastr = luastr.Replace(AppConst.ExtName, AppConst.ZipName);
            if (!m_md5DiffDic.ContainsKey(zipLuastr) && m_baseMd5Dic.ContainsKey(luaname))
            {
                if (m_baseMd5Dic[luaname] != md5)
                {
                    m_md5DiffDic.Add(zipLuastr, 1);
                }
            }
            Object obj = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(obj);
        }

        string output = "";
        if (files.Length > 0)
        {
            output = Application.streamingAssetsPath + "/lua/" + bundleName;
            if (File.Exists(output))
            {
                File.Delete(output);
            }
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), output, options, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }
        return output;
    }

    static void BuildFileIndex()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.EndsWith(".txt")
                || file.Contains("lua32/") || file.EndsWith("Zlast.unity3d")) continue;

            string md5 = Util.md5file(file);
            string value = file.Replace(resPath, string.Empty);
            FileInfo info = new FileInfo(file);
            if (!m_md5DiffDic.ContainsKey(value) && m_baseMd5Dic.ContainsKey(value) &&
                m_baseMd5Dic.ContainsKey(value))
            {
                sw.WriteLine(value + "|" + m_baseMd5Dic[value] + "," + Mathf.Floor(info.Length / 1024));
            }
            else
            {
                sw.WriteLine(value + "|" + md5 + "," + Mathf.Floor(info.Length / 1024));
            }
        }
        //写入Zlast.unity3d到最后(这个文件没有用到)，确保所有需要热更文件下载完毕
        if (!File.Exists(Application.streamingAssetsPath + "/Zlast.unity3d"))
        {
            File.Create(Application.streamingAssetsPath + "/Zlast.unity3d");
        }
        string random = System.DateTime.Now.ToString("yyyymmddhhmmss");
        sw.WriteLine("Zlast.unity3d" + "|" + random + "," + 1);
        sw.Close(); fs.Close();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    public static void EncodeLuaFile(string srcFile, string outFile)
    {
        if (!srcFile.ToLower().EndsWith(".lua"))
        {
            File.Copy(srcFile, outFile, true);
            return;
        }
        bool isWin = true;
        string luaexe = string.Empty;
        string args = string.Empty;
        string exedir = string.Empty;
        string currDir = Directory.GetCurrentDirectory();
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            isWin = true;
            luaexe = "luajit.exe";
            args = "-b " + srcFile + " " + outFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            isWin = false;
            luaexe = "./luac";
            args = "-o " + outFile + " " + srcFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luavm/";
        }
        Directory.SetCurrentDirectory(exedir);
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = luaexe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = isWin;
        info.ErrorDialog = true;
        Util.Log(info.FileName + " " + info.Arguments);

        Process pro = Process.Start(info);
        pro.WaitForExit();
        Directory.SetCurrentDirectory(currDir);
    }

    [MenuItem("LuaFramework/Build Protobuf-lua-gen File")]
    public static void BuildProtobufFile()
    {
        if (!AppConst.ExampleMode)
        {
            UnityEngine.Debug.LogError("若使用编码Protobuf-lua-gen功能，需要自己配置外部环境！！");
            return;
        }
        string dir = AppDataPath + "/Lua/3rd/pblua";
        paths.Clear(); files.Clear(); Recursive(dir);

        string protoc = "d:/protobuf-2.4.1/src/protoc.exe";
        string protoc_gen_dir = "\"d:/protoc-gen-lua/plugin/protoc-gen-lua.bat\"";

        foreach (string f in files)
        {
            string name = Path.GetFileName(f);
            string ext = Path.GetExtension(f);
            if (!ext.Equals(".proto")) continue;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = protoc;
            info.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;
            info.ErrorDialog = true;
            Util.Log(info.FileName + " " + info.Arguments);

            Process pro = Process.Start(info);
            pro.WaitForExit();
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Editor/Tools/UGUIAtlasMaker")]
    static private void MakeAtlas()
    {
        List<string> spriteDirList = new List<string>();
        spriteDirList.Add("/BuildingTexture");
        spriteDirList.Add("/WorldMapTexture");
        spriteDirList.Add("/MainBgTexture");
        List<string> spriteList = new List<string>();
        spriteList.Add("/Resources/Prefabs/Ugui/Building");
        spriteList.Add("/Resources/Prefabs/Ugui/WorldMap");
        spriteList.Add("/Resources/Prefabs/Ugui/MainBg");
        for (int i = 0; i < spriteList.Count; i++)
        {
            string spriteDir = Application.dataPath + spriteList[i];
            if (!Directory.Exists(spriteDir))
            {
                Directory.CreateDirectory(spriteDir);
            }

            DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + spriteDirList[i]);
            foreach (FileInfo pngFile in rootDirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                int value = pngFile.FullName.IndexOf("CastleEffect");
                if (pngFile.FullName.IndexOf("CastleEffect") == -1)
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    GameObject go = new GameObject(sprite.name);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;
                    allPath = spriteDir + "/" + sprite.name + ".prefab";
                    string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
                    PrefabUtility.CreatePrefab(prefabPath, go);
                    GameObject.DestroyImmediate(go);
                }
            }
        }
    }
}