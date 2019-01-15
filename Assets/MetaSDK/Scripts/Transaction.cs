using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Timers;
using System.Threading.Tasks;
using System.Numerics;

using UnityEngine;
using UnityEngine.UI;

using MetaSDK.Tools.Util;
using MetaSDK.IPFS;
using MetaSDK.Components.QRcode;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Nethereum.Hex.HexConvertors.Extensions;

namespace MetaSDK.Components.Transaction
{
    class SendTransactionJson
    {
        public string txid { get; set; }
        public string address { get; set; }
    }

    public class Transaction
    {
        private static Timer timer;
        private static string session;

        // Get data from User
        Action<Dictionary<string, string>> callback;
        private string to, data;
        private BigInteger value;
        private string usage, callbackUrl;

        // Return data form
        public Dictionary<string, string> Reqinfo { get; set; }

        public Transaction(string to, BigInteger value, string data, string usage, Action<Dictionary<string, string>> callback, string callbackUrl) 
        {
            Init(to, value, data, usage, callback, callbackUrl);
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            session = Util.MakeSessionID();
            Reqinfo = new Dictionary<string, string>();
        }

        private void Init(string _to, BigInteger _value, string _data, string _usage, Action<Dictionary<string, string>> _callback, string _callbackUrl)
        {
            to = _to;
            value = _value;
            data = _data;
            usage = _usage;
            if (_callback != null)
            {
                callback = _callback;
            }
            if (!string.IsNullOrEmpty(_callbackUrl))
            {
                callbackUrl = _callbackUrl;
            }
        }

        // Array request, string service, Action<string> callback, string callbackUrl
        public async Task<Texture2D> SendTransaction()
        {
            // Uri for transaction
            string baseRequestUri = "meta://transaction?", trxRequestUri = "";

            // URI for to, value, data
            baseRequestUri += "t=" + to + "&v=" + HexBigIntegerConvertorExtensions.ToHex(value,true) + "&d=" + HexStringUTF8ConvertorExtensions.ToHexUTF8(data);

            // URI for usage
            baseRequestUri += "&u=" + usage;

            // URI for callbackUrl and callback
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                baseRequestUri += "&c=" + WWW.EscapeURL(callbackUrl);
            }
            else
            {
                baseRequestUri += "&c=https%3A%2F%2F0s5eebblre.execute-api.ap-northeast-2.amazonaws.com/dev?key=" + session;
            }

            Debug.Log("Transaction baseRequestUri: " + baseRequestUri);

            // URI for IPFS
            IPFSClass ipfs = new IPFSClass();
            trxRequestUri = await ipfs.IpfsAdd(baseRequestUri);
            Debug.Log("Transaction trxRequestUri(IPFS hash): " + trxRequestUri);

            // Polling request using timer
            timer = new Timer { Interval = 2000 };
            timer.Elapsed += HttpRequest;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();

            // Make QRCode for request
            QRcode.QRcode metaQR = new QRcode.QRcode();
            return metaQR.MakeQR(256, trxRequestUri);
        }

        private void HttpRequest(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            string httpRequestUrl = "https://0s5eebblre.execute-api.ap-northeast-2.amazonaws.com/dev?key=" + session;
            SendTransactionJson json;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(httpRequestUrl);
            httpRequest.BeginGetResponse(new AsyncCallback((IAsyncResult ar) => {
                HttpWebResponse response = (ar.AsyncState as HttpWebRequest).EndGetResponse(ar) as HttpWebResponse;
                Stream respStream = response.GetResponseStream();
                Debug.Log("Transaction begin response " + response.ResponseUri + respStream.CanRead);

                using (StreamReader reader = new StreamReader(respStream))
                {
                    string result = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(result))
                    {
                        json = JsonConvert.DeserializeObject<SendTransactionJson>(result);
                        Debug.Log("Transaction SendTransaction result: " + result);

                        // Excute callback function
                        callback?.Invoke(Reqinfo);

                        timer.Stop();
                        timer.Dispose();
                    }
                }
            }), httpRequest);
        }

        public void TimerTrigger()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}
