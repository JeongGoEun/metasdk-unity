using System;
using System.Diagnostics;
namespace MetaSDK.Tools.Util
{
    public class Util
    {
        public string MakeSessionID() {
            string text = "";
            string possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random rand = new Random();
            for (int i = 0; i < 8; i++) {
                text += possible[rand.Next() / possible.Length];
            }

            Debug.WriteLine("Util.MakeSessionID: "+text);
            return text;
        }
    }
}
