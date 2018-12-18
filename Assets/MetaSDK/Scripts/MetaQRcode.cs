using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using MetaSDK.Tools.Util;

namespace MetaSDK.Components.MetaQRcode
{
    public class MetaQR
    {
        // Defalut size: 128
        public Texture2D MakeQR(string value)
        {
            var encoded = new Texture2D(128, 128);
            var color32 = Encode(value, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();

            return encoded;
        }

        public Texture2D MakeQR(int size, string value)
        {
            var encoded = new Texture2D(size, size);
            var color32 = Encode(value, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();

            return encoded;
        }

        private static Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }
    }
}
