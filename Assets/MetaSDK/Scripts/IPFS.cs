using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;


namespace MetaSDK.IPFS
{
    [Serializable]
    public class IpfsResponse
    {
        public string Name;
        public string Hash;
        public int Size;
    }

    public class IPFS
    {
        private readonly string ipfsAddUrl = "https://ipfs.infura.io:5001/api/v0/add";
        protected IpfsResponse resp;

        public IPFS()
        {
            resp = new IpfsResponse();
        }

        public async Task<string> IpfsAdd(string ipfsData)
        {
            return await HttpPost(ipfsData);
        }

        private async Task<string> HttpPost(string ipfsData)
        {
            byte[] file_bytes = Encoding.ASCII.GetBytes(ipfsData);
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "file", "ipfs.txt");
            HttpResponseMessage response = await httpClient.PostAsync(ipfsAddUrl, form);

            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            resp = JsonUtility.FromJson<IpfsResponse>(response.Content.ReadAsStringAsync().Result);

            return resp.Hash;
        }
    }
}
