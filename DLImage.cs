﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordWSS {
    public class DLImage {

        public static async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri) {
            Console.WriteLine($"Download size: {SizeSuffix(uri.ToString(), 2)}");
            using var httpClient = new HttpClient();

            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            var imageBytes = await httpClient.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(path, imageBytes);
        }

        public static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB" };
        public static string SizeSuffix(string url, int decimalPlaces = 1) {

            long result = -1;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.Method = "HEAD";
            using(System.Net.WebResponse resp = req.GetResponse()) {
                if(long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength)) {
                    result = ContentLength;
                }
            }


            if(decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if(result == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(result, 1024);

            decimal adjustedSize = (decimal)result / (1L << (mag * 10));

            if(Math.Round(adjustedSize, decimalPlaces) >= 1000) {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        private static readonly Random _random = new Random();

        public static int RandomNumber(int min, int max) {
            return _random.Next(min, max);
        }
    }
}
