using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;
using Scripts.Utils;
using Scripts;

namespace LuaFramework {
    public class GameManager : Manager {
        protected static bool initialize = false;
        private List<string> downloadFiles = new List<string>();
        private String m_dataPath;
        private string m_filesName = "files.txt";
        private LoadingView m_view = null;

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Awake() {
            m_dataPath = Util.DataPath;	//数据目录
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init() {
            if(!GameStart.Instance.IsMute)
            {
                m_view = new LoadingView();
                m_view.Init();
            }
            //if (AppConst.ExampleMode) {
            //    InitGui();
            //}
            DontDestroyOnLoad(gameObject);  //防止销毁自己

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
            StartCoroutine(LoadLocalVersions());
        }

        IEnumerator LoadLocalVersions()
        {
            string resPath = Util.AppContentPath(); //游戏包资源目录
            string versionsInfile = resPath + "versions.txt";
            string versionsOutfile = m_dataPath + "versionsTemp.txt";
            if (!Directory.Exists(m_dataPath))
            {
                Directory.CreateDirectory(m_dataPath);
            }
            if (File.Exists(versionsOutfile))
            {
                File.Delete(versionsOutfile);
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(versionsInfile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(versionsOutfile, www.bytes);
                }
                yield return 0;
            }
            else File.Copy(versionsInfile, versionsOutfile, true);
            yield return new WaitForEndOfFrame();

            if (GameStart.Instance.IsDebug == false)
            {
                CheckExtractResource(); //释放资源
            }
            else
            {
                ResManager.initialize(OnResourceInited);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource() {
            bool isExists = Directory.Exists(m_dataPath) && File.Exists(m_dataPath + m_filesName) && File.Exists(m_dataPath + "versions.txt");
            if (isExists)
            {
                Dictionary<string, string> packVersionDic = UnityUtils.AnalyzeTxt(UnityUtils.ReadLocalTxt(m_dataPath + "versionsTemp.txt"));
                Dictionary<string, string> localVersionDic = UnityUtils.AnalyzeTxt(UnityUtils.ReadLocalTxt(m_dataPath + "versions.txt"));
                if (packVersionDic.ContainsKey("versionCode") && localVersionDic.ContainsKey("versionCode"))
                {
                    int localVersionNum = Int32.Parse(localVersionDic["versionCode"]);
                    int packVersionNum = Int32.Parse(packVersionDic["versionCode"]);
                    if (localVersionNum < packVersionNum)
                    {
                        //大版本更新删除数据目录下的files.txt/versions.txt
                        if (Directory.Exists(m_dataPath)) Directory.Delete(m_dataPath, true);
                        Caching.CleanCache();
                        StartCoroutine(OnExtractResource());    //启动释放协成 
                        return;
                    }
                }
                ResManager.initialize(OnResourceInited);
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(OnExtractResource());    //启动释放协成 
        }

        IEnumerator OnExtractResource() {
            
            if (!Directory.Exists(m_dataPath))
            {
                Directory.CreateDirectory(m_dataPath);
            }
            string resPath = Util.AppContentPath(); //游戏包资源目录

            string infileTemp = m_dataPath + m_filesName;
            string infile = resPath + m_filesName;

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(infileTemp, www.bytes);                //保存files文件
                }
                yield return 0;
            }
            else
                File.Copy(infile, infileTemp, true);
            yield return new WaitForEndOfFrame();
            //释放所有文件到数据目录
            string[] files = File.ReadAllLines(infileTemp);
            string[] fs = null;
            string outfileTemp = "";
            int unzipCount = 0;
            foreach (var file in files)
            {
                fs = file.Split('|');
                infileTemp = resPath + fs[0];  //
                outfileTemp = m_dataPath + fs[0];
                string dir = Path.GetDirectoryName(outfileTemp);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (Application.platform == RuntimePlatform.Android)
                {
                    WWW www = new WWW(infileTemp);
                    yield return www;

                    if (www.isDone)
                    {
                        File.WriteAllBytes(outfileTemp, www.bytes);
                        unzipCount++;
                        yield return 0;
                    }
                }
                else
                {
                    File.Copy(infileTemp, outfileTemp, true);
                    unzipCount++;
                }

                if (outfileTemp.EndsWith(AppConst.ZipName))
                {
                    if (outfileTemp.IndexOf("lua/") > 0)
                    {
                        if (!Directory.Exists(m_dataPath + "lua"))
                        {
                            Directory.CreateDirectory(m_dataPath + "lua");
                        }
                        Util.DecompressFileFastZip(outfileTemp, m_dataPath + "lua");
                    }
                    else
                    {
                        Util.DecompressFileFastZip(outfileTemp, m_dataPath);
                    }
                    m_view.UpdateProgress(unzipCount, files.Length);
                    File.Delete(outfileTemp);
                }

                yield return new WaitForEndOfFrame();
            }
            m_view.UpdateProgress(files.Length, files.Length);
            string versionsInfile = resPath + "versions.txt";
            string versionsOutfile = m_dataPath + "versions.txt";
            if (File.Exists(versionsOutfile)) File.Delete(versionsOutfile);
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(versionsInfile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(versionsOutfile, www.bytes);
                }
                yield return 0;
            }
            else File.Copy(versionsInfile, versionsOutfile, true);
            yield return new WaitForEndOfFrame();

            StartCoroutine(OnUpdateResource());
        }


        public void DestroyHotUpgradeView()
        {
            if(m_view != null)
            {
                Invoke("EnterGame", 1.2f);
                m_view.TweenComplete();
            }
        }
        private void EnterGame()
        {
            if(m_view != null)
            {
                m_view.Destroy();
                m_view = null;
            }
        }
        /// <summary>
        /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
        /// </summary>
        IEnumerator OnUpdateResource() {
            downloadFiles.Clear();

            if (!AppConst.UpdateMode) {
                ResManager.initialize(OnResourceInited);
                yield break;
            }
            string dataPath = Util.DataPath;  //数据目录
            string url = AppConst.WebUrl;
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "files.txt?v=" + random;
            Debug.LogWarning("LoadUpdate---->>>" + listUrl);

            WWW www = new WWW(listUrl); yield return www;
            if (www.error != null) {
                OnUpdateFailed(string.Empty);
                yield break;
            }
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            File.WriteAllBytes(dataPath + "files.txt", www.bytes);

            string filesText = www.text;
            string[] files = filesText.Split('\n');

            string message = string.Empty;
            for (int i = 0; i < files.Length; i++) {
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0];
                string localfile = (dataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fileUrl = url + keyValue[0] + "?v=" + random;
                bool canUpdate = !File.Exists(localfile);
                if (!canUpdate) {
                    string remoteMd5 = keyValue[1].Trim();
                    string localMd5 = Util.md5file(localfile);
                    canUpdate = !remoteMd5.Equals(localMd5);
                    if (canUpdate) File.Delete(localfile);
                }
                if (canUpdate) {   //本地缺少文件
                    Debug.Log(fileUrl);
                    message = "downloading>>" + fileUrl;
                    facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
                    /*
                    www = new WWW(fileUrl); yield return www;
                    if (www.error != null) {
                        OnUpdateFailed(path);   //
                        yield break;
                    }
                    File.WriteAllBytes(localfile, www.bytes);
                     * */
                    //这里都是资源文件，用线程下载
                    BeginDownload(fileUrl, localfile);
                    while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }
                }
            }
            yield return new WaitForEndOfFrame();
            message = "更新完成!!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

            ResManager.initialize(OnResourceInited);
        }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        bool IsDownOK(string file) {
            return downloadFiles.Contains(file);
        }

        /// <summary>
        /// 线程下载
        /// </summary>
        void BeginDownload(string url, string file) {     //线程下载
            object[] param = new object[2] {url, file};

            ThreadEvent ev = new ThreadEvent();
            ev.Key = NotiConst.UPDATE_DOWNLOAD;
            ev.evParams.AddRange(param);
            ThreadManager.AddEvent(ev, OnThreadCompleted);   //线程下载
        }

        /// <summary>
        /// 线程完成
        /// </summary>
        /// <param name="data"></param>
        void OnThreadCompleted(NotiData data) {
            switch (data.evName) {
                case NotiConst.UPDATE_EXTRACT:  //解压一个完成
                    //
                break;
                case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
                    downloadFiles.Add(data.evParam.ToString());
                break;
            }
        }

        void OnUpdateFailed(string file) {
            string message = "更新失败!>" + file;
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
        }

        /// <summary>
        /// 资源初始化结束
        /// </summary>
        public void OnResourceInited() {
            LuaManager.InitStart();
            //LuaManager.InitStart();
            //LuaManager.DoFile("Logic/Game");            //加载游戏
            //LuaManager.DoFile("Logic/Network");         //加载网络
            //NetManager.OnInit();                        //初始化网络

            //Util.CallMethod("Game", "OnInitOK");          //初始化完成
            //initialize = true;                          //初始化完 

            ////类对象池测试
            //var classObjPool = ObjPoolManager.CreatePool<TestObjectClass>(OnPoolGetElement, OnPoolPushElement);
            ////方法1
            ////objPool.Release(new TestObjectClass("abcd", 100, 200f));
            ////var testObj1 = objPool.Get();

            ////方法2
            //ObjPoolManager.Release<TestObjectClass>(new TestObjectClass("abcd", 100, 200f));
            //var testObj1 = ObjPoolManager.Get<TestObjectClass>();

            //Debugger.Log("TestObjectClass--->>>" + testObj1.ToString());

            ////游戏对象池测试
            //var prefab = Resources.Load("TestGameObjectPrefab", typeof(GameObject)) as GameObject;
            //var gameObjPool = ObjPoolManager.CreatePool("TestGameObject", 5, 10, prefab);

            //var gameObj = Instantiate(prefab) as GameObject;
            //gameObj.name = "TestGameObject_01";
            //gameObj.transform.localScale = Vector3.one;
            //gameObj.transform.localPosition = Vector3.zero;

            //ObjPoolManager.Release("TestGameObject", gameObj);
            //var backObj = ObjPoolManager.Get("TestGameObject");
            //backObj.transform.SetParent(null);

            //Debug.Log("TestGameObject--->>>" + backObj);
        }

        /// <summary>
        /// 当从池子里面获取时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolGetElement(TestObjectClass obj) {
            Debug.Log("OnPoolGetElement--->>>" + obj);
        }

        /// <summary>
        /// 当放回池子里面时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolPushElement(TestObjectClass obj) {
            Debug.Log("OnPoolPushElement--->>>" + obj);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy() {
            if (NetManager != null) {
                NetManager.Unload();
            }
            if (LuaManager != null) {
                LuaManager.Close();
            }
            Debug.Log("~GameManager was destroyed");
        }
    }
}