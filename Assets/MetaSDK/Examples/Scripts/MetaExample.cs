using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using System.Numerics;

using MetaSDK.Components.MetaLogin;
using MetaSDK.Components.MetaRequest;
using MetaSDK.Components.MetaTransaction;
using MetaSDK.Components.MetaQRcode;
using Nethereum.StandardTokenEIP20.ContractDefinition;

public delegate void CallBack(string result);

public class MetaExample : MonoBehaviour {
    bool isLogin = false, isRequest = false, isTransaction = false;
    Texture2D requestQR, sendTransactionQR;

    async void Start () {

        // Example for MetaLogin
        MetaLogin metaLogin = new MetaLogin("data", "service", "callback", "callbackUrl");
        Debug.Log("Login Request Uri: " + metaLogin.GetRequestUri());

        // Example for MetaRequest
        string[] requestArr = { "10", "2" };
        MetaRequest request = new MetaRequest();
        requestQR = await request.Request(requestArr, "service", RequestCallbackExample, null);

        // Example for MetaTransaction
        string to = "0x8101487270f5411cf213b8d348a2ab46df66245d";
        var value = Web3.Convert.ToWei(0.01, UnitConversion.EthUnit.Ether);
        string data = "data";
        MetaTransaction metaTransaction = new MetaTransaction(to, value, data, "usageExample", SendTransactionCallbackExample, null);
        sendTransactionQR = await metaTransaction.SendTransaction();
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
            GUI.DrawTexture(new Rect(0, 0, 256, 256), sendTransactionQR);
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
   
    public void RequestCallbackExample(Dictionary<string, string> result)
    {
        Debug.Log("RequestCallbackExample");
        foreach(KeyValuePair<string, string> item in result)
        {
            Debug.Log("MetaExample callback result: " + item.Key + item.Value);
        }
        return;
    }

    public void SendTransactionCallbackExample(Dictionary<string, string> result)
    {
        Debug.Log("SendTransactionCallbackExample");
        return;
    }
}
