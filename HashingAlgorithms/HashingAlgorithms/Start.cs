using System;
using System.Text;

namespace HashingAlgorithms {
    class Start {
        static void Main(string[] args) {
            string str = "Hello World";

            byte[] bytes = PadBits(Encoding.ASCII.GetBytes(str));


            foreach (byte b in bytes) {
                Console.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

        }

        static private byte[] PadBits(byte[] bytes) {
            int length = bytes.Length * 8;

            int padBytesNum = (512 - 64 - (length % 512)) / 8;

            byte[] paddedBytes = new byte[bytes.Length + padBytesNum + 8];

            for (int i = 0; i < paddedBytes.Length; i++) {
                if (i < bytes.Length) {
                    paddedBytes[i] = bytes[i];
                } else if (i == bytes.Length) {
                    paddedBytes[i] = 128;
                } else if (i < bytes.Length + padBytesNum) {
                    paddedBytes[i] = 0;
                } else {
                    long length64 = length;
                    byte[] length64Bytes = BitConverter.GetBytes(length64);
                    paddedBytes[i] = length64Bytes[i - bytes.Length - padBytesNum];
                }
            }
            return paddedBytes;
        }
    }
}
