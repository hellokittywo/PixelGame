using UnityEngine;
using System.Collections;
using LuaFramework;
using Scripts;
using PathologicalGames;
using System.Net;

namespace Scripts
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField]
        public bool IsDebug = true;

        [SerializeField]
        public bool ShowTestBtn = false;

        [SerializeField]
        public bool SaveData = true;

        [SerializeField]
        public bool IsMute = false;

        [SerializeField]
        public bool UpdateMode = false;

        [SerializeField]
        public bool IsIphoneX = false;

        private bool m_initLuaFramework = false;
        public bool InitLuaFramework
        {
            get { return m_initLuaFramework; }
            set { m_initLuaFramework = value; }
        }

        private static GameStart _instance;
        public static GameStart Instance { get { return _instance; } }

        void Awake()
        {
            _instance = this;
        }
        // Use this for initialization
        void Start()
        {
            AppConst.UIRoot = GameObject.Find("UI_Root").transform;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            GameManager.Instance.Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnApplicationPause(bool isPause)
        {
            if (InitLuaFramework)
            {
                if (isPause)
                {
                    //游戏暂停 一切停止
                    LuaToCSFunction.CallToLuaFunction("GameEnterBackground");
                }
                else
                {
                    LuaToCSFunction.CallToLuaFunction("GameEnterForeground");
                }
            }

            if (isPause)
            {
                Debug.Log("游戏暂停！");
            }
            else
            {
            }
        }
    }
}