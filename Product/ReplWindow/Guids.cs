/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

// Guids.cs
// MUST match guids.h
using System;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    static class Guids {
#if NTVS_FEATURE_INTERACTIVEWINDOW
        public const string guidReplWindowPkgString = "29102E6C-34F2-4FF1-BA2F-C02ADE3846E8";
        public const string guidReplWindowCmdSetString = "220C57E5-228F-46B5-AF80-D0AB55A44902";        
#else
        public const string guidReplWindowPkgString = "ce8d8e55-ad29-423e-aca2-810d0b16cdc4";
        public const string guidReplWindowCmdSetString = "68cb76e6-98c5-464a-aba9-9f2db66fa0fd";
#endif
        public static readonly Guid guidReplWindowCmdSet = new Guid(guidReplWindowCmdSetString);
    };
}