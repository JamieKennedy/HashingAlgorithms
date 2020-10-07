using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HashingAlgorithms {
    class MD5 {
        public string input { get; set; }
        public string output { get; set; }

        private byte[] bufferA, bufferB, bufferC, bufferD, currentBlock, bufferAA, bufferBB, bufferCC, bufferDD;


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
            byte[] message = PadBits(Encoding.ASCII.GetBytes(input));
            InitBuffers();
            int numberOfBlocks = (message.Length * 8) / 512;

            for (int i = 0; i < numberOfBlocks; i++) {
                currentBlock = new byte[64];
                Array.Copy(message, i * 64, currentBlock, 0, 64);
                hash();
            }





            return "";
        }

        private void hash() {
            bufferAA = bufferA;
            bufferBB = bufferB;
            bufferCC = bufferC;
            bufferDD = bufferD;

            // Round 1
            for (int i = 0; i < 16; i++) {
                switch (i % 4) {
                    case 0:
                        Round1Function(bufferA, bufferB, bufferC, bufferD, i * 4, 7, i + 1);
                        break;
                    case 1:
                        Round1Function(bufferD, bufferA, bufferB, bufferC, i * 4, 12, i + 1);
                        break;
                    case 2:
                        Round1Function(bufferC, bufferD, bufferA, bufferB, i * 4, 17, i + 1);
                        break;
                    case 3:
                        Round1Function(bufferB, bufferC, bufferD, bufferA, i * 4, 22, i + 1);
                        break;
                }
            }

            // Round 2
            for (int i = 0; i < 16; i++) {
                switch (i % 4) {
                    case 0:
                        Round2Function(bufferA, bufferB, bufferC, bufferD, ((5 * i + 1) % 16) * 4, 5, i + 17);
                        break;
                    case 1:
                        Round2Function(bufferD, bufferA, bufferB, bufferC, ((5 * i + 1) % 16) * 4, 9, i + 17);
                        break;
                    case 2:
                        Round2Function(bufferC, bufferD, bufferA, bufferB, ((5 * i + 1) % 16) * 4, 14, i + 17);
                        break;
                    case 3:
                        Round2Function(bufferB, bufferC, bufferD, bufferA, ((5 * i + 1) % 16) * 4, 20, i + 17);
                        break;
                }
            }

            // Round 3
            for (int i = 0; i < 16; i++) {
                switch (i % 4) {
                    case 0:
                        Round3Function(bufferA, bufferB, bufferC, bufferD, ((3 * i + 5) % 16) * 4, 4, i + 33);
                        break;
                    case 1:
                        Round3Function(bufferD, bufferA, bufferB, bufferC, ((3 * i + 5) % 16) * 4, 11, i + 33);
                        break;
                    case 2:
                        Round3Function(bufferC, bufferD, bufferA, bufferB, ((3 * i + 5) % 16) * 4, 16, i + 33);
                        break;
                    case 3:
                        Round3Function(bufferB, bufferC, bufferD, bufferA, ((3 * i + 5) % 16) * 4, 23, i + 33);
                        break;
                }
            }

            // Round 4
            for (int i = 0; i < 16; i++) {
                switch (i % 4) {
                    case 0:
                        Round4Function(bufferA, bufferB, bufferC, bufferD, ((3 * i) % 16) * 4, 6, i + 49);
                        break;
                    case 1:
                        Round4Function(bufferD, bufferA, bufferB, bufferC, ((3 * i) % 16) * 4, 10, i + 49);
                        break;
                    case 2:
                        Round4Function(bufferC, bufferD, bufferA, bufferB, ((3 * i) % 16) * 4, 15, i + 49);
                        break;
                    case 3:
                        Round4Function(bufferB, bufferC, bufferD, bufferA, ((3 * i) % 16) * 4, 21, i + 49);
                        break;
                }
            }

            for (int i = 0; i < 4; i++) {
                bufferA[i] = (byte)(bufferA[i] + bufferAA[i]);
                bufferB[i] = (byte)(bufferB[i] + bufferBB[i]);
                bufferC[i] = (byte)(bufferC[i] + bufferCC[i]);
                bufferD[i] = (byte)(bufferD[i] + bufferDD[i]);
            }

            for (int i = 0; i < 4; i++) {
                Console.WriteLine(Convert.ToString(bufferA[i], 2).PadLeft(8, '0'));
            }

            for (int i = 0; i < 4; i++) {
                Console.WriteLine(Convert.ToString(bufferB[i], 2).PadLeft(8, '0'));
            }

            for (int i = 0; i < 4; i++) {
                Console.WriteLine(Convert.ToString(bufferC[i], 2).PadLeft(8, '0'));
            }

            for (int i = 0; i < 4; i++) {
                Console.WriteLine(Convert.ToString(bufferD[i], 2).PadLeft(8, '0'));
            }



        }

        private void Round1Function(byte[] a, byte[] b, byte[] c, byte[] d, int k, int s, int i) {
            byte[] FAuxValue = FAuxFunc(b, c, d);
            for (int j = 0; j < 4; j++) {
                a[j] = (byte)(b[j] + ((a[j] + FAuxValue[j]) + currentBlock[k] + BitConverter.GetBytes(TFunction(i))[j]) << s);
            }
        }

        private void Round2Function(byte[] a, byte[] b, byte[] c, byte[] d, int k, int s, int i) {
            byte[] GAuxValue = GAuxFunc(b, c, d);
            for (int j = 0; j < 4; j++) {
                a[j] = (byte)(b[j] + ((a[j] + GAuxValue[j]) + currentBlock[k] + BitConverter.GetBytes(TFunction(i))[j]) << s);
            }
        }

        private void Round3Function(byte[] a, byte[] b, byte[] c, byte[] d, int k, int s, int i) {
            byte[] HAuxValue = HAuxFunc(b, c, d);
            for (int j = 0; j < 4; j++) {
                a[j] = (byte)(b[j] + ((a[j] + HAuxValue[j]) + currentBlock[k] + BitConverter.GetBytes(TFunction(i))[j]) << s);
            }
        }

        private void Round4Function(byte[] a, byte[] b, byte[] c, byte[] d, int k, int s, int i) {
            byte[] IAuxValue = IAuxFunc(b, c, d);
            for (int j = 0; j < 4; j++) {
                a[j] = (byte)(b[j] + ((a[j] + IAuxValue[j]) + currentBlock[k] + BitConverter.GetBytes(TFunction(i))[j]) << s);
            }
        }

        private long TFunction(int i) {
            return (long)Math.Floor(Math.Pow(2, 32) * Math.Abs(Math.Sin(i)));
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