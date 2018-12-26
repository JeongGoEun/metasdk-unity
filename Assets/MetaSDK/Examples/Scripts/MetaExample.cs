using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MetaSDK.Components.MetaLogin;
using MetaSDK.Components.MetaRequest;
using MetaSDK.Components.MetaQRcode;

public delegate void CallBack(string result);

public class MetaExample : MonoBehaviour {
    bool isLogin = false, isRequest = false, isTransaction = false;
    Texture2D requestQR;

    async void Start () {
        /*MetaLogin metaLogin = new MetaLogin("data", "service", "callback", "callbackUrl");
        requestUri = metaLogin.GetRequestUri();
        Debug.Log("Login Request Uri: " + requestUri);*/

        // Example for MetaRequest and result
        string[] requestArr = { "10", "2" };
        Action<Dictionary<string, string>> callback = (result) => {
            Debug.Log("RequestCallbackExample");
            foreach (KeyValuePair<string, string> item in result)
            {
                Debug.Log("MetaExample callback result: " + item.Key + item.Value);
            }
            return;
        };

        MetaRequest request = new MetaRequest();
        requestQR = await request.Request(requestArr, "service", callback, null);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnGUI()
    {
        if (isLogin)
        {
            //Debug.Log("OnGUI isLogin");
        }
        else if (isRequest)
        {
            //Debug.Log("OnGUI isRequest");
            GUI.DrawTexture(new Rect(0, 0, 256, 256), requestQR);
        }
        else if (isTransaction)
        {
            //Debug.Log("OnGUI isTransaction");
        }
    }

    public void OnClick() {
        Button curButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string curButtonName = curButton.name;

        switch (curButtonName)
        {
            case "LoginBtn":
                isLogin = true;
                break;
            case "RequestBtn":
                isRequest = true;
                break;
            case "TransactionBtn":
                isTransaction = true;
                break;
        }
    }
   
    public static void RequestCallbackExample(Dictionary<string, string> result)
    {
        Debug.Log("RequestCallbackExample");
        foreach(KeyValuePair<string, string> item in result)
        {
            Debug.Log("MetaExample callback result: " + item.Key + item.Value);
        }
    }
}
