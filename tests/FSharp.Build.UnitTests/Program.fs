// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Program

open System
open System.Reflection
open NUnitLite
open NUnit.Common

type HelperType() = inherit System.Object()

[<EntryPoint>]
let main argv =
    AutoRun(typeof<HelperType>.GetTypeInfo().Assembly).Execute(argv, new ExtendedTextWrapper(Console.Out), Console.In)
