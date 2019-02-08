// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module AnonRecords =

    [<Test>]
    let NotStructConstraintPass() =
        Compiler.AssertPositive("""
type RefClass<'a when 'a : not struct>() = class end
let rAnon = RefClass< {| R: int |}>()""")
        
