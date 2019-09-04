using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;

namespace LuaFramework {
    public class LuaBehaviourData
    {
        public LuaFunction LuaFunc;
        public LuaTable LuaTable;
        public object[] Args;
    }
}