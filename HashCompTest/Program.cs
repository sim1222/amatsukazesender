using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace HashCompTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string sourcedir = "D:\\Temp\\gotiusa_op.mp4";
            string destdir = "D:\\Temp\\gotiusa_op.mp4";
            Class1.HashComparison(sourcedir, destdir);
        }
    }
}
