// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.IO;
using Roslyn.Test.Utilities.Desktop;

namespace PEVerify
{
    class Program
    {
        static int Main(string[] args)
        {
            string assemblyPath = null;
            bool metadataOnly = false;
            foreach (var arg in args)
            {
                switch (arg.ToUpperInvariant())
                {
                    case "/IL":
                    case "/NOLOGO":
                    case "/UNIQUE":
                        // ignore these options
                        break;
                    case "/MD":
                        metadataOnly = true;
                        break;
                    default:
                        if (assemblyPath != null)
                        {
                            Console.WriteLine("Assembly already specified or unknown option.");
                            return -1;
                        }

                        assemblyPath = arg;
                        break;
                }
            }

            if (!Path.IsPathRooted(assemblyPath))
            {
                var workingDir = Directory.GetCurrentDirectory();
                assemblyPath = Path.Combine(workingDir, assemblyPath);
            }

            var errors = CLRHelpers.PeVerify(assemblyPath, metadataOnly);
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return errors.Length;
        }
    }
}
