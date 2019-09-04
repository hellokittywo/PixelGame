using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Scripts;
namespace Scripts.Utils
{
    public class OutLog : MonoBehaviour
    {
        static List<string> mLines = new List<string>();
        static List<string> mWriteTxt = new List<string>();
        private StreamWriter writer;

        public bool show_on_screen = true;

        void Awake()
        {
            // Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
            string persistentPath = "";
            if (LuaToCSFunction.GetApplicationPlatform() == 3)
            {
                persistentPath = Application.streamingAssetsPath;
            }
            else
            {
                persistentPath = Application.persistentDataPath;
            }
            string base_path = persistentPath + "/logs";
            if (!Directory.Exists(base_path))
            {
                Directory.CreateDirectory(base_path);
            }
            string log_path = base_path + "/outLog.txt";
            // 将上一个log转移logs目录
            if (File.Exists(log_path))
            {
                DateTime last_time = File.GetLastWriteTime(log_path);
                string time_str = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", last_time);
                string backup_path = base_path + "/BOWLog_" + time_str + ".txt";
                File.Copy(log_path, backup_path);
                File.Delete(log_path);
            }
            // 每次启动客户端删除10天前保存的Log
            DateTime time = DateTime.Now;
            DateTime pass_time = time.AddDays(-10);
            foreach (string full_name in Directory.GetFiles(base_path))
            {
                if (File.GetLastWriteTime(full_name) < pass_time)
                {
                    File.Delete(full_name);
                }
            }
            writer = new StreamWriter(log_path, false, Encoding.UTF8);
            // 在这里做一个Log的监听
            Application.RegisterLogCallback(HandleLog);

            //Application.logMessageReceived += HandleLog;
            // OutLog的第一个输出
            Debug.Log("OutLog Start");
        }

        void StartLogCallBack() {
            Application.RegisterLogCallback(HandleLog);
        }

        void OnDestroy()
        {
            Debug.Log("OutLog Finish");
            writer.Close();
        }

        void Update()
        {
        }
        void HandleLog(string str, string stackTrace, LogType type)
        {
            writer.WriteLine("[{0}]{1}", type.ToString(), str);
            writer.Flush();
            if (type == LogType.Error || type == LogType.Exception)
            {
                ShowLogOnScreen(str);
            }
        }

        // 这里我把错误的信息保存起来，用来输出在手机屏幕上
        static public void ShowLogOnScreen(string text)
        {
            if (Application.isPlaying)
            {
                while (mLines.Count > 20)
                {
                    mLines.RemoveAt(0);
                }
                mLines.Add(text.Substring(0, Mathf.Min(text.Length, 1000)));
            }
        }

        void OnGUI()
        {
            if (show_on_screen)
            {
                GUI.color = Color.red;
                for (int i = 0, imax = mLines.Count; i < imax; ++i)
                {
                    GUILayout.Label(mLines[i]);
                }
            }
        }
    }
}