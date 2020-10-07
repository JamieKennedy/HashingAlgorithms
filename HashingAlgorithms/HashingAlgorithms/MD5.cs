using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HashingAlgorithms {
    class MD5 {
        public string input { get; set; }
        public string output { get; set; }

        private byte[] bufferA, bufferB, bufferC, bufferD;


        public MD5(string input, string output = "hex") {
            this.input = input;
            this.output = output;


        }

        private byte[] FAuxFunc(byte[] x, byte[] y, byte[] z) {
            byte[] output = new byte[4];

            for (int i = 0; i < 4; i++) {
                output[i] = (byte)((x[i] & y[i]) | ((~x[i]) & z[i]));
            }

            return output;
        }

        private byte[] GAuxFunc(byte[] x, byte[] y, byte[] z) {
            byte[] output = new byte[4];

            for (int i = 0; i < 4; i++) {
                output[i] = (byte)((x[i] & x[i]) | (y[i] & (~z[i])));
            }

            return output;
        }

        private byte[] HAuxFunc(byte[] x, byte[] y, byte[] z) {
            byte[] output = new byte[4];

            for (int i = 0; i < 4; i++) {
                output[i] = (byte)(x[i] ^ y[i] ^ z[i]);
            }

            return output;
        }

        private byte[] IAuxFunc(byte[] x, byte[] y, byte[] z) {
            byte[] output = new byte[4];

            for (int i = 0; i < 4; i++) {
                output[i] = (byte)(y[i] ^ (x[i] | (~z[i])));
            }

            return output;
        }

        public string getHash() {
            byte[] bytes = PadBits(Encoding.ASCII.GetBytes(input));
            InitBuffers();



            return "";
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

        private void InitBuffers() {
            bufferA = new byte[] { 1, 35, 69, 103 };
            bufferB = new byte[] { 137, 171, 205, 239 };
            bufferC = new byte[] { 254, 220, 186, 152 };
            bufferD = new byte[] { 118, 84, 50, 16 };
        }

    }
}