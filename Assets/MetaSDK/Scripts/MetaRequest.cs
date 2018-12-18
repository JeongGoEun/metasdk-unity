using System;
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



using MetaSDK.Tools.Util;
using MetaSDK.IPFS;
using MetaSDK.Components.MetaQRcode;

/*
Start -> Initialization -> Load -> Validation -> Events -> Render
*/
namespace MetaSDK.Components.MetaRequest
{
    public class MetaRequest
    {
        private static Timer timer;
        private static string session;
        private string pubKey, privKey;
        protected string trxRequestUri, baseRequestUri;
        Action<string> callback;

        public MetaRequest()
        {
            session = Util.MakeSessionID();
            trxRequestUri = "";
            baseRequestUri = "";
        }

        // Array request, string service, Action<string> callback, string callbackUrl
        public async Task<Texture2D> Request(string[] request, string usage, Action<string> callback, string callbackUrl) 
        {
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
                this.callback = callback;
                baseRequestUri += "&c=https%3A%2F%2F0s5eebblre.execute-api.ap-northeast-2.amazonaws.com/dev?key=" + session;
            }

            // URI for AA or SP metaID - Should pass parameter with metaID string
            //this.trxRequestUri += "&m=" + metaID

            // URI for public key
            baseRequestUri += "&p=" + WWW.EscapeURL(pubKey);

            Debug.Log("Request trxRequestUri: " + baseRequestUri);

            // URI for IPFS
            IPFSClass ipfs = new IPFSClass();
            trxRequestUri = await ipfs.IpfsAdd(baseRequestUri);
            Debug.Log("trxRequest IPFS hash: " + trxRequestUri);

            // Polling request using timer
            timer = new Timer { Interval = 2000 };
            timer.Elapsed += HttpRequest;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Make QRCode for request
            MetaQR metaQR = new MetaQR();
            return metaQR.MakeQR(256, trxRequestUri);
        }

        private void HttpRequest(System.Object source, System.Timers.ElapsedEventArgs e)
        {
            string httpRequestUrl = "https://0s5eebblre.execute-api.ap-northeast-2.amazonaws.com/dev?key=" + session;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpRequestUrl);
            request.BeginGetResponse(new AsyncCallback((IAsyncResult ar) => {
                HttpWebResponse response = (ar.AsyncState as HttpWebRequest).EndGetResponse(ar) as HttpWebResponse;
                Stream respStream = response.GetResponseStream();
                using(StreamReader reader = new StreamReader(respStream))
                {
                    if(!string.IsNullOrEmpty(reader.ReadToEnd()))
                    {
                        // Decryption using privKey

                        /*
                        byte[] csrEncode = Convert.FromBase64CharArray(characters, 0, characters.Length);
                        Pkcs10CertificationRequest decodedCsr = new Pkcs10CertificationRequest(csrEncode);
                        Console.WriteLine(decodedCsr.GetCertificationRequestInfo().Subject);
                        */

                        timer.Dispose();
                    }
                }
            }), request);
        }
    }
}
