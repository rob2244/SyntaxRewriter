﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Securify.Console;
using System;
using System.IO;
using System.Linq;

namespace Securify
{
    class Program
    {
        static int Main(string[] args)
        {
            var dir = GetDirectory(args);

            if (!dir.Exists)
            {
                System.Console.WriteLine($"{dir.FullName} could not be found");
                return (int)ExitCodes.DirectoryNotFound;
            }

            var controllerDir = dir.EnumerateDirectories("controllers", SearchOption.AllDirectories).FirstOrDefault();

            if (controllerDir == null || !controllerDir.Exists)
            {
                System.Console.WriteLine($"Controller directory not found");
                return (int)ExitCodes.DirectoryNotFound;
            }

            var rewriter = new TokenAttributeSyntaxRewriter();

            foreach (var file in controllerDir.EnumerateFiles())
                Rewrite(file, rewriter);

            System.Console.WriteLine("Token rewrite completed successfully");
            return (int)ExitCodes.Success;
        }



        private static DirectoryInfo GetDirectory(string[] args)
        {
            if (args.Length == 0)
                return new DirectoryInfo(Directory.GetCurrentDirectory());

            return new DirectoryInfo(args[0].ToString());
        }

        private static void Rewrite(FileInfo file, TokenAttributeSyntaxRewriter rewriter)
        {
            var sourceTree = CSharpSyntaxTree.ParseText(File.ReadAllText(file.FullName));
            var newTree = rewriter.Visit(sourceTree.GetRoot());

            if (newTree != sourceTree.GetRoot())
                File.WriteAllText(file.FullName, newTree.ToString());
        }
    }
}
