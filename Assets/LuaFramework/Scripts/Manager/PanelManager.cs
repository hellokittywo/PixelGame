using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework {
    public class PanelManager : Manager {
        private Transform parent;

        Transform Parent {
            get {
                if (parent == null) {
                    GameObject go = GameObject.Find("UI_Root");
                    if (go != null) parent = go.transform;
                }
                return parent;
            }
        }

        /// <summary>
        /// 创建面板，请求资源管理器
        /// </summary>
        /// <param name="type"></param>
        public void CreatePanel(string name, LuaTable luaTable = null, LuaFunction func = null)
        {
            AssetBundle bundle = null;// ResManager.LoadBundle(name);
            StartCoroutine(StartCreatePanel(name, bundle, luaTable, func));
            //Debug.LogWarning("CreatePanel::>> " + name + " " + bundle);
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        IEnumerator StartCreatePanel(string name, AssetBundle bundle, LuaTable luaTable = null, LuaFunction func = null)
        {
            //Debug.Log("从AssetBundle中加载：" + name);
            //name += "Panel";
//             GameObject prefab = Util.LoadAsset(bundle, name);
//             yield return new WaitForEndOfFrame();
//             if (Parent.FindChild(name) != null || prefab == null) {
//                 yield break;
//             }
            GameObject go = ObjectPoolManager.Instance.LoadObjectByName(name, Parent);
//             go.name = name;

            yield return new WaitForEndOfFrame();
            Util.AddComponent<LuaBehaviour>(go);

            if (func != null) func.Call(luaTable, go);
            //Debug.Log("StartCreatePanel------>>>>" + name);
        }
    }
}