using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LuaFramework;

namespace Scripts.Utils
{
    public class UnityUtils
    {
        public static void UnityToAndroid(string func, params object[] args)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass m_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject m_curActivity = m_unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (m_curActivity == null)
            {
                Debug.LogError("获得不到JAVA对象");
                return;
            }
            if (args.Length > 0)
            {
                m_curActivity.Call(func, args);
            }
            else
            {
                m_curActivity.Call(func);
            }
#endif
        }

        public static string GetAndroidData(string funcName)
        {
            string reStr = "";
            if (!GameStart.Instance.IsMute)
            {
#if UNITY_ANDROID && !UNITY_EDITOR  
            AndroidJavaClass m_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject m_curActivity = m_unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (m_curActivity == null)
            {
                Debug.LogError("获得不到JAVA对象");
                return "获得不到JAVA对象";
            }

            reStr = m_curActivity.Call<string>(funcName);
#endif
            }
            return reStr;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        public static string GetMD5(string msg)
        {
            StringBuilder sb = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg + "127.###!@EEAsdsad???plk");
                byte[] newB = md5.ComputeHash(buffer);

                foreach (byte item in newB)
                {
                    sb.Append(item.ToString("x2"));
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// 读取本地txt文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadLocalTxt(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        
        /// <summary>
        /// 解析文件
        /// </summary>
        /// <param name="filesText">文件中的内容，以string格式输入</param>
        /// <returns>以字典格式返回，键值：[0] | 值：[1]</returns>
        internal static Dictionary<string, string> AnalyzeTxt(string filesText, bool isMd5File = false)
        {
            Dictionary<string, string> resDic = new Dictionary<string, string>();

            string[] files = filesText.Split('\n');

            for (int i = 0; i < files.Length; i++)
            {
                string[] keyValue = files[i].Split('|');
                if (keyValue.Length >= 2)   //"Bow.assetbundle|382e750278dca9cb15ab24b572226251,1024"最后一个是文件大小
                {
                    if (resDic.ContainsKey(keyValue[0]))
                    {
                        resDic.Remove(keyValue[0]);
                    }
                    if (isMd5File == true)
                    {
                        resDic.Add(keyValue[0], keyValue[1].Trim() + "," + i);
                    }
                    else
                    {
                        resDic.Add(keyValue[0], keyValue[1].Trim());
                    }
                }
            }

            return resDic;
        }

        internal static string SubstringByLength(string str, int length)
        {
            if (str.Length > 8)
            {
                str = str.Substring(0, length) + "...";
            }

            return str;
        }

        internal static string GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            string _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                _methodName = sfs[i].GetMethod().Name;//方法名称
                //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                if (_filterdName.Contains(_methodName)) continue;
                _fullName = _methodName + "()->" + _fullName;
            }
            st = null;
            sfs = null;
            _filterdName = _methodName = null;
            return _fullName.TrimEnd('-', '>');
        }


        internal static bool CheckMail(string email)
        {
            string filter = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            return Regex.IsMatch(email, filter);
        }

        private static int CodeLeft = 1;
        private static int CodeRight = 2;
        private static int CodeButtom = 4;
        private static int CodeTop = 8;

        internal static int GetLineLengthInRect(int P1x, int P1y, int P2x, int P2y, int rectX1, int rectX2, int rectY1, int rectY2)
        {
            int C1 = Code(P1x, P1y, rectX1, rectX2, rectY1, rectY2), C2 = Code(P2x, P2y, rectX1, rectX2, rectY1, rectY2);
            int C;
            int Px = 0, Py = 0;//记录交点  
            while (C1 != 0 || C2 != 0)//两个点（P1x,P1y）,（P2x,P2y）不都在矩形框内；都在内部就画出线段  
            {
                if ((C1 & C2) != 0)   //两个点在矩形框的同一外侧 → 不可见  
                {
                    P1x = 0;
                    P1y = 0;
                    P2x = 0;
                    P2y = 0;
                    break;
                }
                C = C1;
                if (C1 == 0)// 判断P1 P2谁在矩形框内（可能是P1，也可能是P2）  
                {
                    C = C2;
                }

                if ((C & CodeLeft) != 0)//用与判断的点在左侧   
                {
                    Px = rectX1;
                    Py = P1y + (int)(Convert.ToDouble(P2y - P1y) / (P2x - P1x) * (rectX1 - P1x));
                }
                else if ((C & CodeRight) != 0)//用与判断的点在右侧   
                {
                    Px = rectX2;
                    Py = P1y + (int)(Convert.ToDouble(P2y - P1y) / (P2x - P1x) * (rectX2 - P1x));
                }
                else if ((C & CodeTop) != 0)//用与判断的点在上方  
                {
                    Py = rectY1;
                    Px = P1x + (int)(Convert.ToDouble(P2x - P1x) / (P2y - P1y) * (rectY1 - P1y));
                }
                else if ((C & CodeButtom) != 0)//用与判断的点在下方  
                {
                    Py = rectY2;
                    Px = P1x + (int)(Convert.ToDouble(P2x - P1x) / (P2y - P1y) * (rectY2 - P1y));
                }

                if (C == C1) //上面判断使用的是哪个端点就替换该端点为新值  
                {
                    P1x = Px;
                    P1y = Py;
                    C1 = Code(P1x, P1y, rectX1, rectX2, rectY1, rectY2);
                }
                else
                {
                    P2x = Px;
                    P2y = Py;
                    C2 = Code(P2x, P2y, rectX1, rectX2, rectY1, rectY2);
                }
            }
            return (int)Math.Sqrt((P1x - P2x) * (P1x - P2x) + (P1y - P2y) * (P1y - P2y));
        }

        private static int Code(int x, int y, int rectX1, int rectX2, int rectY1, int rectY2) //端点编码函数 左右上下分别对应一位  
        {
            int c = 0;
            if (x < rectX1)
            {
                c = c | CodeLeft;
            }
            if (x > rectX2)
            {
                c = c | CodeRight;
            }
            if (y < rectY1)
            {
                c = c | CodeTop;
            }
            if (y > rectY2)
            {
                c = c | CodeButtom;
            }
            return c;
        }

        public static UnityEngine.Object GetChildByName(GameObject parent, string name)
        {
            UnityEngine.Object ret = null;

            ForEachChildFunction temp = (UnityEngine.Object obj) =>
            {
                bool compareResult = obj.name.Equals(name);
                ret = compareResult ? obj : null;
                return compareResult;
            };

            ForEachChild(parent, temp);
            return ret;
        }

        public delegate bool ForEachChildFunction(UnityEngine.Object obj);

        public static bool ForEachChild(GameObject obj, ForEachChildFunction fun)
        {
            //广度优先
            Transform root = obj.transform;
            for (int i = 0; i < root.childCount; ++i)
            {
                GameObject kid = root.GetChild(i).gameObject;
                if (true == fun(kid))
                {
                    return true;
                }
            }

            for (int i = 0; i < root.childCount; ++i)
            {
                GameObject kid = root.GetChild(i).gameObject;
                if (true == ForEachChild(kid, fun))
                {
                    return true;
                }
            }
            return false;
        }
    }
}