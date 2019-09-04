using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LuaFramework {
    public class LuaBehaviour : Base {
        private string data = null;
        private AssetBundle bundle = null;
        private Dictionary<GameObject, LuaBehaviourData> buttons = new Dictionary<GameObject, LuaBehaviourData>();
        private Dictionary<LuaTable, Dictionary<LuaFunction, EventDelegate.Callback>> callBackDic = new Dictionary<LuaTable, Dictionary<LuaFunction, EventDelegate.Callback>>();
        private EventDelegate.Callback callBack = null; 

        //protected void Awake() {
        //    Util.CallMethod(name, "Awake", gameObject);
        //}

        //protected void Start() {
        //    Util.CallMethod(name, "Start");
        //}

        protected void OnClick() {
            Util.CallMethod(name, "OnClick");
        }

        protected void OnClickEvent(GameObject go) {
            Util.CallMethod(name, "OnClick", go);
        }

        /// <summary>
        /// 初始化面板
        /// </summary>
        public void OnInit(AssetBundle bundle, string text = null) {
            this.data = text;   //初始化附加参数
            this.bundle = bundle; //初始化
            Debug.LogWarning("OnInit---->>>" + name + " text:>" + text);
        }

        /// <summary>
        /// 获取一个GameObject资源
        /// </summary>
        /// <param name="name"></param>
        public GameObject LoadAsset(string name) {
            if (bundle == null) return null;
#if UNITY_5
            return bundle.LoadAsset(name, typeof(GameObject)) as GameObject;
#else
            return bundle.Load(name, typeof(GameObject)) as GameObject;
#endif
        }

        public void AddEventDelegate(List<EventDelegate> list, LuaTable luaTable, LuaFunction luafunc, params object[] args)
        {
            if (list == null || luaTable == null || luafunc == null) return;
            EventDelegate.Callback func = delegate()
            {
                if (args != null && args.Length == 1)
                {
                    luafunc.Call(luaTable, args[0]);
                }
                else
                {
                    luafunc.Call(luaTable);
                }
            };
            EventDelegate.Add(list, func);
            if (!callBackDic.ContainsKey(luaTable))
            {
                callBackDic.Add(luaTable, new Dictionary<LuaFunction, EventDelegate.Callback>());
            }
            callBackDic[luaTable].Add(luafunc, func);
        }

        public void SetEventDelegate(List<EventDelegate> list, LuaTable luaTable, LuaFunction luafunc, params object[] args)
        {
            if (list == null || luaTable == null || luafunc == null) return;
            callBack = delegate()
            {
                if (args != null && args.Length == 1)
                {
                    luafunc.Call(luaTable, args[0]);
                }
                else
                {
                    luafunc.Call(luaTable);
                }
            };
            EventDelegate.Set(list, callBack);
        }

        public void RemoveEventDelegate(List<EventDelegate> list)
        {
            if (list == null || callBack == null)
            {
                return;
            }
            EventDelegate.Remove(list, callBack);
        }

        public void RemoveEventDelegate(List<EventDelegate> list, LuaTable luaTable, LuaFunction luafunc)
        {
            if (!callBackDic.ContainsKey(luaTable))
            {
                return;
            }
            if (!callBackDic[luaTable].ContainsKey(luafunc))
            {
                return;
            }
            EventDelegate.Remove(list, callBackDic[luaTable][luafunc]);
        }

        /// <summary>
        /// 添加单击事件
        /// </summary>
        public void AddClick(GameObject go, LuaTable luaTable = null, LuaFunction luafunc = null, params object[] args)
        {
            if (go == null || luaTable == null || luafunc == null) return;
            if (buttons.ContainsKey(go))
            {
                RemoveClick(go);
            }
            LuaBehaviourData data = new LuaBehaviourData();
            data.LuaTable = luaTable;
            data.LuaFunc = luafunc;
            data.Args = args;
            buttons.Add(go, data);
            UIEventListener.Get(go).onClick += OnButtonsClickHandle;
        }

        private void OnButtonsClickHandle(GameObject go)
        {
            if (buttons.ContainsKey(go))
            {
                LuaBehaviourData data = buttons[go];
                if (data.Args != null && data.Args.Length == 1)
                {
                    data.LuaFunc.Call(data.LuaTable, go, data.Args[0]);
                }
                else
                {
                    data.LuaFunc.Call(data.LuaTable, go);
                }
            }
        }

        /// <summary>
        /// 删除单击事件
        /// </summary>
        /// <param name="go"></param>
        public void RemoveClick(GameObject go)
        {
            if (go == null) return;
            LuaBehaviourData data = null;
            if (buttons.TryGetValue(go, out data))
            {
                UIEventListener.Get(go).onClick -= OnButtonsClickHandle;
                buttons.Remove(go);
                data.LuaFunc.Dispose();
                data.LuaFunc = null;
            }
        }

        /// <summary>
        /// 清除单击事件
        /// </summary>
        public void ClearClick()
        {
            foreach (var de in buttons)
            {
                if (de.Value != null)
                {
                    de.Value.LuaFunc.Dispose();
                }
            }
            buttons.Clear();
        }
        
        //-----------------------------------------------------------------
        protected void OnDestroy()
        {
            if (bundle)
            {
                bundle.Unload(true);
                bundle = null;  //销毁素材
            }
            ClearClick();
            //Util.ClearMemory();
            //Debug.Log("~" + name + " was destroy!");
        }
    }
}