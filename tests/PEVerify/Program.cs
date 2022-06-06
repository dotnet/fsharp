// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.IO;
using Roslyn.Test.Utilities.Desktop;
using System.Globalization;

namespace PEVerify
{
    class Program
    {
        static int Main(string[] args)
        {
            // this version of PEVerify is only intended to run F# test suit
            // force the en-US culture so that tests warning/error message comparision
            // is not culture dependent
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
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
