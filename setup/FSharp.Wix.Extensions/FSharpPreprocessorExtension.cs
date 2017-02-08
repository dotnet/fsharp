// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.Tools.WindowsInstallerXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FSharp.WIX.Extensions
{
    public class FSharpPreprocessorExtension : PreprocessorExtension
    {
        private readonly string[] prefixes = new string[] { "fsharp" };

        public override string[] Prefixes
        {
            get { return this.prefixes; }
        }

        public override string EvaluateFunction(string prefix, string function, string[] args)
        {
            if (prefixes.Contains(prefix))
            {
                switch (function)
                {
                    case "guid":
                        return this.Guid(args);
                }
            }

            return null;
        }

        private string Guid(string[] args)
        {
            var input = string.Join("|", args);
            var bytes = Encoding.Default.GetBytes(input);
            var output = MD5.Create().ComputeHash(bytes);

            return new Guid(output).ToString();
        }
    }
}
