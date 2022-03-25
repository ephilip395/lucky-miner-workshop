﻿using System.IO;
using System.IO.Compression;

namespace Lucky {
    public static class GZipUtil {
        public static byte[] Compress(byte[] data) {
            if (data == null || data.Length == 0) {
                return data;
            }
            try {
                using (MemoryStream stream = new MemoryStream())
                using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress)) {
                    gZipStream.Write(data, 0, data.Length);
                    gZipStream.Close();
                    return stream.ToArray();
                }
            }
            catch {
                return data;
            }
        }

        public static byte[] Decompress(byte[] zippedData) {
            if (zippedData == null || zippedData.Length == 0) {
                return zippedData;
            }
            try {
                using (MemoryStream inputStream = new MemoryStream(zippedData))
                using (GZipStream gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (MemoryStream outputStream = new MemoryStream()) {
                    byte[] block = new byte[LuckyKeyword.IntK];
                    while (true) {
                        int bytesRead = gZipStream.Read(block, 0, block.Length);
                        if (bytesRead <= 0) {
                            break;
                        }
                        else {
                            outputStream.Write(block, 0, bytesRead);
                        }
                    }
                    gZipStream.Close();
                    return outputStream.ToArray();
                }
            }
            catch {
                return new byte[0];
            }
        }
    }
}
