using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using com.tencent.httpdns;
using UnityEngine.UI;

public class ClickEvent : MonoBehaviour
{
    private const string AndroidAppId = "0AND066Q1U479G8H";
    private const string iOSAppId = "0IOS076Q1U4QXOT8";
    private const int dnsId = 73650;
    private const string dnsKey = @"F2925ifQ";

    
    public InputField _inputField;
    public Button _buttonSync;
    public Button _buttonAsync;
    public Text _text;
    
#if UNITY_ANDROID
    private static Thread mMainThread = Thread.CurrentThread;
    private static bool mIsMainThread
    {
        get
        {
            return Thread.CurrentThread == mMainThread;
        }
    }

    private static AndroidJavaObject mHttpDnsObj = null;
#endif
    private void Awake()
    {
#if UNITY_ANDROID
        HttpDns.Init(AndroidAppId, dnsId, dnsKey, true, 0);
#elif UNITY_IOS
        HttpDns.Init(iOSAppId, dnsId, dnsKey, true, 0);
#endif
        _buttonSync.onClick.AddListener(OnClickSync);
        _buttonAsync.onClick.AddListener(OnClickAsync);
    }
    
    private void OnClickSync()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            Debug.Log("请输入一个域名...");
            return;
        }
        string ip = HttpDns.GetAddrByName(_inputField.text);
        _text.text = ip;
    }

    private void OnClickAsync()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            Debug.Log("请输入一个域名...");
            return;
        }
        HttpDns.GetAddrByNameAsync(_inputField.text);
    }

    public void onDnsNotify(string ipString) {
        print (ipString);
        string[] sArray=ipString.Split(new char[] {';'});
        if (sArray != null && sArray.Length > 1) {
            if (!sArray [1].Equals ("0")) {
                //使用建议：当ipv6地址存在时，优先使用ipv6地址
                //TODO 使用ipv6地址进行连接，注意格式，ipv6需加方框号[ ]进行处理，例如：http://[64:ff9b::b6fe:7475]/
                _text.text = "ipv6 address exist:" + sArray [1] + ", suggest to use ipv6 address.";
            } else if(!sArray [0].Equals ("0")){
                //使用ipv4地址进行连接
                _text.text = "ipv6 address not exist, use the ipv4 address:" + sArray [0] + " to connect.";
            } else {
                //异常情况返回为0,0，建议重试一次
                print("ReStartSyncClick");
                string domainStr = _inputField.text;
                print(domainStr);
                if (domainStr == null || domainStr.Equals("")) {
                    domainStr = "www.qq.com";
                    print("input is null, use the default domain:www.qq.com.");
                    _text.text = "input is null, use the default domain:www.qq.com.";
                }
                HttpDns.GetAddrByNameAsync(domainStr);
            }
        }
    }
}
