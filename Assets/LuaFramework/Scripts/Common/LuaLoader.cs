﻿using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;

namespace LuaFramework {
    /// <summary>
    /// 集成自LuaFileUtils，重写里面的ReadFile，
    /// </summary>
    public class LuaLoader : LuaFileUtils {

        // Use this for initialization
        public LuaLoader() {
            instance = this;
            beZip = AppConst.LuaBundleMode;
        }

        /// <summary>
        /// 添加打入Lua代码的AssetBundle
        /// </summary>
        /// <param name="bundle"></param>
        public void AddBundle(string url)
        {
            if (File.Exists(url))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(url);
                if (bundle != null)
                {
                    string bundleName = url.Replace(Util.DataPath + "lua/", "").Replace(".unity3d", "");
                    base.AddSearchBundle(bundleName.ToLower(), bundle);
                }
            }
        }

        /// <summary>
        /// 当LuaVM加载Lua文件的时候，这里就会被调用，
        /// 用户可以自定义加载行为，只要返回byte[]即可。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName) {
            return base.ReadFile(fileName);     
        }
    }
}