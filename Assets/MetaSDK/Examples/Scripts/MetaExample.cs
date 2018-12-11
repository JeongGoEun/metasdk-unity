using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MetaSDK.Components.MetaLogin;
using MetaSDK.Components.MetaQRcode;
using MetaSDK.IPFS;

public class MetaExample : MonoBehaviour {
    string requestUri = "";

    async void Start () {
        MetaLogin metaLogin = new MetaLogin("data", "service", "callback", "callbackUrl");
        requestUri = metaLogin.GetRequestUri();
        Debug.Log("Login Request Uri: " + requestUri);

        IPFS ipfs = new IPFS();
        string ipfsHash = await ipfs.IpfsAdd(requestUri);

        Debug.Log("Response hash : " + ipfsHash);
	}

    // Update is called once per frame
    void Update () {
		
	}

    void OnGUI()
    {
        MetaQRcode metaQR = new MetaQRcode();

        if (GUI.Button(new Rect(300, 300, 256, 256), metaQR.MakeQR(256, requestUri), GUIStyle.none)) { }
    }

    public void OnClick() {
        Button curButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string curButtonName = curButton.name;
        Debug.Log("OnClick button: " + curButtonName);

        switch (curButtonName)
        {
            case "LoginBtn":
                break;
            case "RequestBtn":
                break;
            case "TransactionBtn":
                break;
        }
    }
}
