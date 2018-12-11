using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using MetaSDK.Tools.Util;

namespace MetaSDK.Components.MetaLogin
{
    public class MetaLogin
    {
        protected string requestUri;
        protected string sessionID;
        protected string callback;
        Util util = new Util();

        // Constructor
        public MetaLogin(string data, string service, string callback, string callbackUrl)
        {
            this.callback = callback;

            // Uri for service
            requestUri = "meta://authentication?usage=login&service=" + service;

            //Uri for callback
            if (callbackUrl != null)
            {
                requestUri += "&callback=" + WWW.EscapeURL(callbackUrl);
            }
            else
            {
                requestUri += "&callback=https%3A%2F%2F2g5198x91e.execute-api.ap-northeast-2.amazonaws.com/test?key=" + util.MakeSessionID();
            }
        }

        public string GetRequestUri() {
            return requestUri;
        }
    }
}
