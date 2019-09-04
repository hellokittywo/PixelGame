using System.Net;
using UnityEngine;
using System.Runtime.InteropServices;
using Scripts;
using Scripts.Event;
namespace Scripts.Utils
{
    public class PaymentManager : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            m_instance = this;
        }

        //IOS内购相关
#if UNITY_IOS && !UNITY_EDITOR  
        [DllImport ("__Internal")]  
        private static extern void _iosPay(string callbackGameObject, string callbackFunc, string productId, int iapId, string extendJsonData);   
        [DllImport ("__Internal")]
        private static extern void _getIosToken();
        [DllImport ("__Internal")]
        private static extern void _toAppStore();
        [DllImport ("__Internal")]
        private static extern void _saveAccount(string account);
#endif
        public void UnityToIos(string callbackGameObject, string callbackFunc, string productId, int iapId, string extendJsonData)
        {
#if UNITY_IOS && !UNITY_EDITOR  
            _iosPay(callbackGameObject, callbackFunc, productId, iapId, extendJsonData);
#endif
        }

        //安卓支付通信
        public void UnityToAndroid(string callbackFunc, string productId, int iapId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR  
            AndroidJavaClass m_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject m_curActivity = m_unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (m_curActivity == null)
            {
                Debug.LogError("获得不到JAVA对象");
                return;
            }

            m_curActivity.Call("Pay", callbackFunc, productId, iapId.ToString());
#endif
        }

        void SendIosPushToken(string token)
        {
            LuaToCSFunction.IosPushToken = token;
        }

        void SendIosUUID(string uuid)
        {
            LuaToCSFunction.IosUuid = uuid;
        }


        void IosSysBit(string sysBit)
        {
            vp_GlobalEvent.Send(EventName.Ios_GetSysBitComplete);
        }

        internal void SaveAccountToIosSSKeychain(string value)
        {
#if UNITY_IOS && !UNITY_EDITOR 
            _saveAccount(value);
#endif
        }

        internal void ToAppStore()
        {
#if UNITY_IOS && !UNITY_EDITOR 
            _toAppStore();
#endif
        }

        //java或者ios回调函数
        void PayMoneySuccess(string receipt_data)
        {
            LuaToCSFunction.PayMoneyCallBack(receipt_data);
        }

        void PayMoneyFailed(string result)
        {
            LuaToCSFunction.PayMoneyCallBack(result, 0);
        }

        private void GetTestAccount()
        {
#if UNITY_ANDROID && !UNITY_EDITOR  

#endif
#if UNITY_IOS && !UNITY_EDITOR 
            _getIosToken();
#endif
        }

        private static PaymentManager m_instance;
        public static PaymentManager Instance
        {
            get { return m_instance; }
        }
    }

}