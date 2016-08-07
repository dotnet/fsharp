// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Unittests

open System
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler

[<TestFixture>]
module EditDistance =
    open Internal.Utilities.EditDistance

    [<Test>]
    [<TestCase("CA", "ABC", ExpectedResult = 3)>]
    let RestrictedEditDistance (str1 : string, str2 : string) : int =
        CalcEditDistance (str1, str2)
