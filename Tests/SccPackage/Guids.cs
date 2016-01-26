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

namespace Microsoft.TestSccPackage
{
    static class Guids
    {
        public const string guidSccPackagePkgString = "394d1b85-f4a7-4af2-9078-e4aab7673b22";
        public const string guidSccPackageCmdSetString = "045cf08e-e640-42c4-af80-0251d6f553a1";

        public static readonly Guid guidSccPackageCmdSet = new Guid(guidSccPackageCmdSetString);
    };
}