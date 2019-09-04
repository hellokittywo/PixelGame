using LuaFramework;
using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Text;
using System.Security.Cryptography;
using Scripts.Utils;
using UnityEngine.SceneManagement;

namespace Scripts
{
    public class LuaToCSFunction
    {
        public static string IosPushToken;
        public static string IosUuid;
        public static string GetIosPushToken()
        {
            return IosPushToken;
        }
        public static string GetIosUuid()
        {
            return IosUuid;
        }
        public static GameObject LoadPrefabByName(string prefabName, Transform parent)
        {
            return ObjectPoolManager.Instance.LoadObjectByName(prefabName, parent, false);
        }

        private static Dictionary<string, Texture> m_textureDic = new Dictionary<string,Texture>();
        public static void SetUITexture(UITexture texture, string url)
        {
            if(!m_textureDic.ContainsKey(url))
            {
                m_textureDic[url] = Resources.Load<Texture>(url);
            }
            texture.mainTexture = m_textureDic[url];
            texture.MakePixelPerfect();
        }

        public static void Release(GameObject go)
        {
            ObjectPoolManager.Instance.Release(go);
        }

        public static void PoolDestroy(GameObject go)
        {
            ObjectPoolManager.Instance.PoolDestroy(go);
        }

        public static void SetGameObjectLocalScale(GameObject go, float x, float y, float z)
        {
            go.transform.localScale = new Vector3(x, y, z);
        }

        public static void SetGameObjectLocalEulerAngles(GameObject go, float x, float y, float z)
        {
            go.transform.localEulerAngles = new Vector3(x, y, z);

        }

        public static void SetGameObjectLocalPosition(GameObject go, float x, float y, float z)
        {
            go.transform.localPosition = new Vector3(x, y, z);

        }

        public static void SetGameObjectPosition(GameObject go, float x, float y, float z)
        {
            go.transform.position = new Vector3(x, y, z);
        }

        public static void SetGameObjectAndChildrenLayer(GameObject go, int layer)
        {
            foreach (Transform tran in go.GetComponentsInChildren<Transform>(true))
            {
                tran.gameObject.layer = layer;
            }
        }

        public static void CallToLuaFunction(string funcName, params object[] args)
        {
            LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            mgr.DoFile("Main.lua");
            mgr.CallFunction(funcName, args);
        }

        public static void CallToLuaFunction(string functionName)
        {
            LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            mgr.DoFile("Main.lua");
            mgr.CallFunction(functionName);
        }

        public static void LoadSceneAsync(string name)
        {
            SceneManager.LoadSceneAsync(name);
        }

        public static void PreLoadResources(string prefabName)
        {
            ObjectPoolManager.Instance.PreLoadObject(prefabName);
        }

        public static void DestroyHotUpgradeView()
        {
            LuaFramework.LuaHelper.GetGameManager().DestroyHotUpgradeView();
        }

        public static void UnloadAssetBundle(GameObject go)
        {
            ObjectPoolManager.Instance.Release(go);
        }

        public static int GetApplicationPlatform()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return 1;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return 2;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return 3;
            }
            return 0;
        }

        public static string GetAccountName()
        {
            return ObscuredPrefs.GetString("AccountName");
        }

        public static void SetAccountName(string name)
        {
            ObscuredPrefs.SetString("AccountName", name);
        }

        public static void PlayerPrefsSave(string name, string value)
        {
            PlayerPrefs.SetString(name, value);
        }

        public static string PlayerPrefsGet(string name)
        {
            return PlayerPrefs.GetString(name);
        }

        public static void PlayerPrefsDeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void PayMoney(string callbackFunc, string productId, int iapId, string extendJsonData)
        {
#if UNITY_IOS && !UNITY_EDITOR  
            PaymentManager.Instance.UnityToIos("PaymentManager", callbackFunc, productId, iapId, extendJsonData);
#else
            PaymentManager.Instance.UnityToAndroid(callbackFunc, productId, iapId);
#endif
        }

        public static void PayMoneyCallBack(string receipt_data, int result = 1)
        {
            LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            mgr.DoFile("CSToLuaFunction.lua");
            if (result == 1)
            {
                mgr.CallFunction("PayMoneySuccess", receipt_data);
            }
            else
            {
                mgr.CallFunction("PayMoneyFailed", receipt_data);
            }
        }

        //------------------------------------音乐音效
        public static void PlayBgMusic(string name, bool isAutoDestroy, bool loop)
        {
            AudioManager.Instance.PlayBgMusic(name, isAutoDestroy, loop, true);
        }
        public static void PlaySound(string name, bool isAutoDestroy)
        {
            AudioManager.Instance.PlaySound(name, isAutoDestroy);
        }

        public static void SavePlayerMusic(bool isOpen)
        {
            AudioManager.Instance.SetBgMusicVolumn(isOpen);
        }
        public static void SavePlayerSound(bool isOpen)
        {
            AudioManager.Instance.SetSoundVolumn(isOpen);
        }
        public static float GetScreenWidthW()
        {
            return Screen.width;
        }

        public static float GetScreenHeightH()
        {
            return Screen.height;
        }

        public static float GetScreenRatio()
        {
            return (float)Screen.width / (float)Screen.height;
        }
        public static void SetUICameraRect(Camera camera, float x, float y, float w, float h)
        {
            camera.rect = new Rect(x, y, w, h);
        }

        public static bool IsIphoneX()
        {
            if (SystemInfo.deviceModel.Contains("iPhone10,3") || SystemInfo.deviceModel.Contains("iPhone10,6"))
            {
                return true;
            }
            if (Application.platform == RuntimePlatform.WindowsEditor && GameStart.Instance.IsIphoneX)
            {
                return true;
            }
            return false;
        }
        public static bool GetMusic()
        {
            int i = ObscuredPrefs.GetInt("Music");
            return i == 0;
        }

        public static void SetMusic(bool isOpen)
        {
            AudioManager.Instance.SetSoundVolumn(isOpen);
            AudioManager.Instance.SetBgMusicVolumn(isOpen);
            ObscuredPrefs.SetInt("Music", isOpen ? 0 : 1);
        }

        public static void AddOutLog()
        {
            GameManager.Instance.AddOutLog();
        }

        //加密  
        public static string Encrypt(string content, string k)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(k);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateEncryptor();
            byte[] contentBytes = UTF8Encoding.UTF8.GetBytes(content);
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
        }

        //解密  
        public static string Decrypt(string content, string k)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(k);
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = keyBytes;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = rm.CreateDecryptor();
            byte[] contentBytes = Convert.FromBase64String(content);
            byte[] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
            return UTF8Encoding.UTF8.GetString(resultBytes);
        }  

    }

}
