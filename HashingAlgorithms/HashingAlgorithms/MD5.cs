using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace HashingAlgorithms {
    class MD5 {
        public string input { get; set; }
        public string output { get; set; }

        private uint bufferA, bufferB, bufferC, bufferD, bufferAA, bufferBB, bufferCC, bufferDD;
        private uint[] currentBlock;

        // Array containing values for the integer part of 2^32 * abs(sin(i))
        protected readonly static uint[] T = new uint[64]
            {   0xd76aa478,0xe8c7b756,0x242070db,0xc1bdceee,
                0xf57c0faf,0x4787c62a,0xa8304613,0xfd469501,
                0x698098d8,0x8b44f7af,0xffff5bb1,0x895cd7be,
                0x6b901122,0xfd987193,0xa679438e,0x49b40821,
                0xf61e2562,0xc040b340,0x265e5a51,0xe9b6c7aa,
                0xd62f105d,0x2441453,0xd8a1e681,0xe7d3fbc8,
                0x21e1cde6,0xc33707d6,0xf4d50d87,0x455a14ed,
                0xa9e3e905,0xfcefa3f8,0x676f02d9,0x8d2a4c8a,
                0xfffa3942,0x8771f681,0x6d9d6122,0xfde5380c,
                0xa4beea44,0x4bdecfa9,0xf6bb4b60,0xbebfbc70,
                0x289b7ec6,0xeaa127fa,0xd4ef3085,0x4881d05,
                0xd9d4d039,0xe6db99e5,0x1fa27cf8,0xc4ac5665,
                0xf4292244,0x432aff97,0xab9423a7,0xfc93a039,
                0x655b59c3,0x8f0ccc92,0xffeff47d,0x85845dd1,
                0x6fa87e4f,0xfe2ce6e0,0xa3014314,0x4e0811a1,
                0xf7537e82,0xbd3af235,0x2ad7d2bb,0xeb86d391};


        public MD5(string input, string output = "hex") {
            this.input = input;
            this.output = output;


        }

        // F auxiliary function - RCF-1321
        private uint FAuxFunc(uint x, uint y, uint z) {
            return (x & y) | ((~x) & z);
        }

        // G auxiliary function - RCF-1321
        private uint GAuxFunc(uint x, uint y, uint z) {
            return (x & x) | (y & (~z));
        }

        // H auxiliary function - RCF-1321
        private uint HAuxFunc(uint x, uint y, uint z) {
            return x ^ y ^ z;
        }

        // I auxiliary function - RCF-1321
        private uint IAuxFunc(uint x, uint y, uint z) {
            return y ^ (x | (~z));
        }

        public string getHash() {
            byte[] message = PadBits(Encoding.ASCII.GetBytes(input));
            InitBuffers();
            int numberOfBlocks = (message.Length * 8) / 512;

            for (int i = 0; i < numberOfBlocks; i++) { // Each 512 bit block of the message
                currentBlock = new uint[16];
                // Copy 4 bytes into 1 32-bit unsigned int
                for (int j = 0; j < 16; j++) {
                    byte[] bytes = {
                        message[(i * 64) + (j * 4)],
                        message[(i * 64) + (j * 4) + 1],
                        message[(i * 64) + (j * 4) + 2],
                        message[(i * 64) + (j * 4) + 3]
                    };
                    currentBlock[j] = BitConverter.ToUInt32(bytes);
                }
                hash();
            }


            string output = GetOutput();

            return output;
        }

        private void hash() {
            // Copy current buffer values into temp buffers
            bufferAA = bufferA;
            bufferBB = bufferB;
            bufferCC = bufferC;
            bufferDD = bufferD;

            for (int i = 0; i < 64; i++) {
                if (i >= 0 && i < 16) { // Round 1
                    switch (i % 4) {
                        case 0:
                            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, i, 7, i + 1, FAuxFunc(bufferB, bufferC, bufferD));
                            break;
                        case 1:
                            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, i, 12, i + 1, FAuxFunc(bufferA, bufferB, bufferC));
                            break;
                        case 2:
                            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, i, 17, i + 1, FAuxFunc(bufferD, bufferA, bufferB));
                            break;
                        case 3:
                            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, i, 22, i + 1, FAuxFunc(bufferC, bufferD, bufferA));
                            break;
                    }
                } else if (i >= 16 && i < 32) { // Round 2
                    switch (i % 4) {
                        case 0:
                            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, (5 * i + 1) % 16, 5, i + 1, GAuxFunc(bufferB, bufferC, bufferD));
                            break;
                        case 1:
                            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, (5 * i + 1) % 16, 9, i + 1, GAuxFunc(bufferA, bufferB, bufferC));
                            break;
                        case 2:
                            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, (5 * i + 1) % 16, 14, i + 1, GAuxFunc(bufferD, bufferA, bufferB));
                            break;
                        case 3:
                            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, (5 * i + 1) % 16, 20, i + 1, GAuxFunc(bufferC, bufferD, bufferA));
                            break;
                    }
                } else if (i >= 32 && i < 48) { // Round 3
                    switch (i % 4) {
                        case 0:
                            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, (3 * i + 5) % 16, 4, i + 1, HAuxFunc(bufferB, bufferC, bufferD));
                            break;
                        case 1:
                            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, (3 * i + 5) % 16, 11, i + 1, HAuxFunc(bufferA, bufferB, bufferC));
                            break;
                        case 2:
                            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, (3 * i + 5) % 16, 16, i + 1, HAuxFunc(bufferD, bufferA, bufferB));
                            break;
                        case 3:
                            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, (3 * i + 5) % 16, 23, i + 1, HAuxFunc(bufferC, bufferD, bufferA));
                            break;
                    }
                } else {
                    switch (i % 4) { // Round 4
                        case 0:
                            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, (7 * i) % 16, 6, i + 1, IAuxFunc(bufferB, bufferC, bufferD));
                            break;
                        case 1:
                            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, (7 * i) % 16, 10, i + 1, IAuxFunc(bufferA, bufferB, bufferC));
                            break;
                        case 2:
                            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, (7 * i) % 16, 15, i + 1, IAuxFunc(bufferD, bufferA, bufferB));
                            break;
                        case 3:
                            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, (7 * i) % 16, 21, i + 1, IAuxFunc(bufferC, bufferD, bufferA));
                            break;
                    }
                }

            }


            // Adds temp buffer values onto updated buffer values
            bufferA = bufferA + bufferAA;
            bufferB = bufferB + bufferBB;
            bufferC = bufferC + bufferCC;
            bufferD = bufferD + bufferDD;
        }

        // Passes reference of 32-bit unsigned int which gets updated using the following funciton
        private void RoundFunction(ref uint a, uint b, uint c, uint d, int k, int s, int i, uint auxValue) {
            a = b + (a + auxValue + currentBlock[k] + T[i - 1]) << s;
        }

        static private byte[] PadBits(byte[] bytes) {
            int length = bytes.Length * 8;

            // Number of bytes that need to added
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
                    // Adds 64-bit representation of the message size
                    long length64 = length;
                    byte[] length64Bytes = BitConverter.GetBytes(length64);
                    paddedBytes[i] = length64Bytes[i - bytes.Length - padBytesNum];
                }
            }
            return paddedBytes;
        }

        // Append the 4 buffers hex values after they have been reversed
        private string GetOutput() {

            string output = ReverseInt(bufferA).ToString("x8") +
                ReverseInt(bufferB).ToString("x8") +
                ReverseInt(bufferC).ToString("x8") +
                ReverseInt(bufferD).ToString("x8");

            return output;
        }

        // Takes unsigned int and returns its reverse
        private uint ReverseInt(uint n) {
            return (((n & 0x000000ff) << 24) |
                        (n >> 24) |
                    ((n & 0x00ff0000) >> 8) |
                    ((n & 0x0000ff00) << 8));
        }

        // Initialise the buffers values
        private void InitBuffers() {
            bufferA = 0x67452301;
            bufferB = 0xefcdab89;
            bufferC = 0x98badcfe;
            bufferD = 0x10325476;
        }

    }
}