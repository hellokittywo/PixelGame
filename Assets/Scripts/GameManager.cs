using LuaFramework;
using PathologicalGames;
using Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class GameManager
    {

        private static GameManager _instance = new GameManager();
        public static GameManager Instance { get { return _instance; } }

        private bool m_initFlag = false;

        internal void Init()
        {
            if (m_initFlag == false)
            {
                m_initFlag = true;
                GameObject obj = new GameObject("PoolManager");
                SpawnPool pool = obj.AddComponent<SpawnPool>();
                pool.poolName = AppConst.PoolName;
                pool.dontDestroyOnLoad = true;
                PoolManager.Pools.Add(pool);

                GameObject lua = new GameObject("GameManager");
                Main main = lua.AddComponent<Main>();
                AppConst.GameManager = lua.transform;

                GameObject payObj = new GameObject("PaymentManager");
                PaymentManager paymentManager = payObj.AddComponent<PaymentManager>();
            }
            else
            {
                LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
                mgr.DoFile("Main.lua");
                mgr.CallFunction("EnterMainScene");
            }
        }

        public void AddOutLog()
        {
            GameObject obj = GameObject.Find("PoolManager");
            obj.AddComponent<OutLog>();
        }

    }
}