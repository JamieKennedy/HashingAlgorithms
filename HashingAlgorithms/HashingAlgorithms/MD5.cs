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

            for (int i = 0; i < numberOfBlocks; i++) { // Each 512 bit block of the 
                currentBlock = new uint[16];
                // Copy 4 bytes into 1 32-but unsigned int
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

            // Round 1
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 0, 7, 1, FAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 1, 12, 2, FAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 2, 17, 3, FAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 3, 22, 4, FAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 4, 7, 5, FAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 5, 12, 6, FAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 6, 17, 7, FAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 7, 22, 8, FAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 8, 7, 9, FAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 9, 12, 10, FAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 10, 17, 11, FAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 11, 22, 12, FAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 12, 7, 13, FAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 13, 12, 14, FAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 14, 17, 15, FAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 15, 22, 16, FAuxFunc(bufferC, bufferD, bufferA));

            // Round 2
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 1, 5, 17, GAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 6, 9, 18, GAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 11, 14, 19, GAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 0, 20, 20, GAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 5, 5, 21, GAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 10, 9, 22, GAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 15, 14, 23, GAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 4, 20, 24, GAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 9, 5, 25, GAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 14, 9, 26, GAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 3, 14, 27, GAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 8, 20, 28, GAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 13, 5, 29, GAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 2, 9, 30, GAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 7, 14, 31, GAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 12, 20, 32, GAuxFunc(bufferC, bufferD, bufferA));

            // Round 3
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 5, 4, 33, HAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 8, 11, 34, HAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 11, 16, 35, HAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 14, 23, 36, HAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 1, 4, 37, HAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 4, 11, 38, HAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 7, 16, 39, HAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 10, 23, 40, HAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 13, 4, 41, HAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 0, 11, 42, HAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 3, 16, 43, HAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 6, 23, 44, HAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 9, 4, 45, HAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 12, 11, 46, HAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 15, 16, 47, HAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 2, 23, 48, HAuxFunc(bufferC, bufferD, bufferA));

            // Round 4
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 0, 6, 1, IAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 7, 10, 2, IAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 14, 15, 3, IAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 5, 21, 4, IAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 12, 6, 5, IAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 3, 11, 6, IAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 10, 15, 7, IAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 1, 21, 8, IAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 8, 6, 9, IAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 15, 10, 10, IAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 6, 15, 11, IAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 13, 21, 12, IAuxFunc(bufferC, bufferD, bufferA));
            RoundFunction(ref bufferA, bufferB, bufferC, bufferD, 4, 6, 13, IAuxFunc(bufferB, bufferC, bufferD));
            RoundFunction(ref bufferD, bufferA, bufferB, bufferC, 11, 10, 14, IAuxFunc(bufferA, bufferB, bufferC));
            RoundFunction(ref bufferC, bufferD, bufferA, bufferB, 2, 15, 15, IAuxFunc(bufferD, bufferA, bufferB));
            RoundFunction(ref bufferB, bufferC, bufferD, bufferA, 9, 21, 16, IAuxFunc(bufferC, bufferD, bufferA));

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