using System;
using System.Text;

namespace HashingAlgorithms {
    class Start {
        static void Main(string[] args) {
            string str = "Hello World";


            MD5 hash = new MD5(str);
            string hashOut = hash.getHash();
            Console.WriteLine(hashOut);
        }


    }
}
