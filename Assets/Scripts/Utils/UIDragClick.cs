using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
namespace Scripts.Utils
{
    public class UIDragClick : MonoBehaviour
    {
        public void AddDragStart(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            UIEventListener.Get(go).onDragStart += delegate(GameObject o)
            {
                luafunc.Call(luaTable, go);
            };
        }

        public void AddDrag(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            UIEventListener.Get(go).onDrag += delegate(GameObject o, Vector2 delta)
            {
                luafunc.Call(luaTable, go);
            };
            
        }

        public void AddDragEnd(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            UIEventListener.Get(go).onDragEnd += delegate(GameObject o)
            {
                luafunc.Call(luaTable, go);
            };

        }
    }
}
