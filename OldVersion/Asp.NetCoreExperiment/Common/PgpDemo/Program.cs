﻿using Cinchoo.PGP;
using PgpCore;
using System;

namespace PgpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var txt = @"C:\MyFile\pgp\data.txt";
            var pgpf = @"C:\MyFile\pgp\20190927.pgp";
            var key = @"C:\MyFile\pgp\FilesKey\secret-key.asc";
            var pubkey = @"C:\MyFile\pgp\FilesKey\GLNgpgPub";
            var password = "123456";
            //using (ChoPGPEncryptDecrypt pgp = new ChoPGPEncryptDecrypt())
            //{
            //    pgp.DecryptFileAndVerify(pgpf,txt, pubkey, key,password);
            //}

            using (var pgp2 = new PGP())
            {
                pgp2.DecryptFile(pgpf, txt, key, password);
                Console.WriteLine("Hello World!");
            }
        }
    }
}
