using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.tencent.httpdns
{
    class HttpDns
    {
#if UNITY_ANDROID
        private static Thread mMainThread = Thread.CurrentThread;
        private static bool mIsMainThread
        {
            get
            {
                return Thread.CurrentThread == mMainThread;
            }
        }

        private static AndroidJavaObject sHttpDnsObj;
#endif

#if !UNITY_IOS
        [DllImport("__Internal")]
        private static extern string WGGetHostByName(string domain);
        [DllImport("__Internal")]
        private static extern void WGGetHostByNameAsync(string domain);
        [DllImport("__Internal")]
        private static extern bool WGSetDnsOpenId(string dnsOpenId);
        [DllImport("__Internal")]
        private static extern bool WGSetInitInnerParams(string appkey, bool debug, int timeout);
        [DllImport("__Internal")]
        private static extern bool WGSetInitParams(string appkey, int dnsid, string dnskey, bool debug, int timeout);
#endif

        public static void Init(string appId,int dnsId,string dnskey, bool debug = true, int timeout = 0)
        {
            Debug.Log("HttpDns Init");
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityObj = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (activityObj == null) {
                return;
            }
            AndroidJavaObject contextObj = activityObj.Call<AndroidJavaObject>("getApplicationContext");
            // 初始化 HTTPDNS
            AndroidJavaObject httpDnsClass = new AndroidJavaObject("com.tencent.msdk.dns.MSDKDnsResolver");
            sHttpDnsObj = httpDnsClass.CallStatic<AndroidJavaObject>("getInstance");
            if (sHttpDnsObj == null) {
                return;
            }
         
            // V4.0.0开始（推荐使用）
            AndroidJavaObject dnsConfigBuilder = new AndroidJavaObject("com.tencent.msdk.dns.DnsConfig$Builder");
            dnsConfigBuilder.Call<AndroidJavaObject>("dnsId", $"{dnsId}");
            dnsConfigBuilder.Call<AndroidJavaObject>("dnsIp", "119.29.29.98");
            dnsConfigBuilder.Call<AndroidJavaObject>("dnsKey", dnskey);
            //dnsConfigBuilder.Call<AndroidJavaObject>("setUseExpiredIpEnable", false);

            // 其他配置信息...
            AndroidJavaObject dnsConfig = dnsConfigBuilder.Call<AndroidJavaObject>("build");
            
            sHttpDnsObj.Call("init", contextObj, dnsConfig);
#endif
#if UNITY_IOS
            WGSetInitInnerParams(appId, debug, timeout);
#endif
            Debug.Log("HttpDns finished!");

        }

        public static string GetAddrByName(string domain)
        {
            Debug.Log($"HttpDns GetAddrByName:{domain}");
            string ip = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!mIsMainThread && AndroidJNI.AttachCurrentThread() != 0)
            {
                return null;
            }
            string ips = sHttpDnsObj.Call<string>("getAddrByName", domain);
            if (!mIsMainThread)
            {
                AndroidJNI.DetachCurrentThread();
            }
            if (null != ips)
            {
                Debug.Log("GetHttpDnsIP: " + domain + " -> " + ips);
                string[] ipArr = ips.Split(';');
                if (2 == ipArr.Length && !"0".Equals(ipArr[0]))
                    ip = ipArr[0];
            }
            return ip;
#elif UNITY_IOS
            return WGGetHostByName (domain);
#endif
            return ip;
        }

        public static void GetAddrByNameAsync(string domain)
        {
            Debug.Log($"HttpDns GetAddrByNameAsync:{domain}");
#if UNITY_ANDROID && !UNITY_EDITOR
            GetAddrByName(domain);
#endif
#if UNITY_IOS
            WGGetHostByNameAsync (domain);
#endif
        }
    }
}
