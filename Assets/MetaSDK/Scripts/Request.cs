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

using UnityEngine;
using UnityEngine.UI;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

using MetaSDK.Tools.Util;
using MetaSDK.IPFS;
using MetaSDK.Components.QRcode;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/*
Start -> Initialization -> Load -> Validation -> Events -> Render
*/
namespace MetaSDK.Components.Request
{
    class RequestJson
    {
        public string meta_id { get; set; }
        public string signature { get; set; }
        public Dictionary<string, string> data { get; set; }
    }

    public class Request
    {
        private static Timer timer;
        private static string session;

        private string pubKey = "";
        private string privKey = "";

        // Get
        Action<Dictionary<string, string>> callback;
        string[] request;
        string usage, callbackUrl;
        public Dictionary<string, string> Reqinfo { get; set; }

        public Request(string[] _request, string _usage, Action<Dictionary<string, string>> _callback, string _callbackUrl)
        {
            // Init class instance
            Init(_request, _usage, _callback, _callbackUrl);

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            session = Util.MakeSessionID();
            Reqinfo = new Dictionary<string, string>();
        }

        private void Init(string[] _request, string _usage, Action<Dictionary<string, string>> _callback, string _callbackUrl)
        {
            request = _request;
            usage = _usage;
            if(_callback != null)
            {
                callback = _callback;
            }
            if (!string.IsNullOrEmpty(_callbackUrl))
            {
                callbackUrl = _callbackUrl;
            }
        }

        // Array request, string service, Action<string> callback, string callbackUrl
        public async Task<Texture2D> GetRequestQR() 
        {
            // Uri for request transaction
            string baseRequestUri = "", trxRequestUri = "";

            // Get RSA key(public key, private key)
            Util.GetRSAKey(out pubKey, out privKey);

            // URI for usage
            baseRequestUri += "meta://information?u=" + usage;

            // URI for request
            baseRequestUri += "&r=" + string.Join(",", request);

            // URI for callbackUrl and callback
            if( !string.IsNullOrEmpty(callbackUrl) )
            {
                baseRequestUri += "&c=" + WWW.EscapeURL(callbackUrl);
            }
            else
            {
                baseRequestUri += "&c=https%3A%2F%2F0s5eebblre.execute-api.ap-northeast-2.amazonaws.com/dev?key=" + session;
            }

            // URI for AA or SP metaID - Should pass parameter with metaID string
            //this.trxRequestUri += "&m=" + metaID

            // URI for public key
            baseRequestUri += "&p=" + WWW.EscapeURL(pubKey);

            Debug.Log("Request baseRequestUri: " + baseRequestUri);

            // URI for IPFS
            IPFSClass ipfs = new IPFSClass();
            trxRequestUri = await ipfs.IpfsAdd(baseRequestUri);
            Debug.Log("Request trxRequestUri(IPFS hash): " + trxRequestUri);

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
            RequestJson json;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(httpRequestUrl);
            httpRequest.BeginGetResponse(new AsyncCallback((IAsyncResult ar) => {
                HttpWebResponse response = (ar.AsyncState as HttpWebRequest).EndGetResponse(ar) as HttpWebResponse;
                Stream respStream = response.GetResponseStream();
                Debug.Log("Request begin response " + response.ResponseUri + respStream.CanRead);

                using (StreamReader reader = new StreamReader(respStream))
                {
                    string result = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(result))
                    {
                        // Decryption using privKey
                        byte[] respBytes = Convert.FromBase64String(result);

                        // Get response data to byte array
                        byte[] encryptedAesKey = respBytes.Take(256).ToArray<byte>();
                        byte[] encryptedData = respBytes.Skip(256).Take(respBytes.Length).ToArray<byte>();

                        // Step1. aes키는 rsa로 암호화 되어있는데 private key로 풀어야 한다.
                        IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/NONE/PKCS1Padding");
                        RsaKeyParameters privateKey = (RsaKeyParameters)Util.pairKey.Private;
                        cipher.Init(false, privateKey);
                        byte[] secret = cipher.DoFinal(encryptedAesKey);

                        // Step2. data decryption using AES algorithm with secret key
                        AesManaged aes = new AesManaged {
                            Key = secret,
                            IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            Mode = CipherMode.ECB
                        };

                        // Step3. Decryption data using secret key
                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                        using(MemoryStream ms = new MemoryStream(encryptedData))
                        {
                            using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using(StreamReader sr = new StreamReader(cs))
                                {
                                    string plain = sr.ReadToEnd();
                                    json = JsonConvert.DeserializeObject<RequestJson>(plain);

                                    foreach (string id in this.request)
                                    {
                                        Reqinfo.Add(id, Encoding.UTF8.GetString(Convert.FromBase64String(json.data[id])));
                                    }

                                    // Excute callback function
                                    callback?.Invoke(Reqinfo);

                                    timer.Stop();
                                    timer.Dispose();
                                }
                            }
                        }
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
