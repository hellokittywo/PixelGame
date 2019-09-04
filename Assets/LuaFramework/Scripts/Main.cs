using UnityEngine;
using System.Collections;
using Scripts;

namespace LuaFramework
{
    /// <summary>
    /// 框架主入口
    /// </summary>
    public class Main : MonoBehaviour
    {
        void Start()
        {
            AppConst.UpdateMode = GameStart.Instance.UpdateMode;
            AppConst.UpdateMode = false;
            AppConst.LuaByteMode = GameStart.Instance.UpdateMode;
            AppConst.LuaBundleMode = GameStart.Instance.UpdateMode;

            AppFacade.Instance.StartUp();   //启动游戏
        }
    }
}