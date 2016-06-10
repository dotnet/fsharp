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

        private readonly List<CultureInfo> supportedLanguages = new List<CultureInfo>()
        {
            new CultureInfo(1028), // CHT
            new CultureInfo(1029), // CSY
            new CultureInfo(1031), // DEU
            new CultureInfo(1033), // ENU
            new CultureInfo(1036), // FRA
            new CultureInfo(1040), // ITA
            new CultureInfo(1041), // JPN
            new CultureInfo(1042), // KOR
            new CultureInfo(1045), // PLK
            new CultureInfo(1046), // PTB
            new CultureInfo(1049), // RUS
            new CultureInfo(1055), // TRK
            new CultureInfo(2052), // CHS
            new CultureInfo(3082), // ESN
        };

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
                    case "localeToId":
                        return this.LocaleToId(args);
                    case "localeToCulture":
                        return this.LocaleToCulture(args);
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

        private string LocaleToId(string[] args)
        {
            return this.GetSupportedLanguage(args).LCID.ToString();
        }

        private string LocaleToCulture(string[] args)
        {
            return this.GetSupportedLanguage(args).Name;
        }
        
        private CultureInfo GetSupportedLanguage(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Exactly one argument (locale) should be provided.");
            }

            var language = this.supportedLanguages.FirstOrDefault(l => l.ThreeLetterWindowsLanguageName.ToString() == args[0]);

            if (language == null)
            {
                throw new ArgumentException($"Locale '{args[0]}' is not supported.");
            }
            else
            {
                return language;
            }
        }
    }
}
