// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeMap.FSharpStackFrameNameParserTests

open System.Reflection
open Xunit
open Microsoft.VisualStudio.FSharp.Editor

[<Theory>]
[<InlineData("")>]
[<InlineData("   ")>]
[<InlineData(null)>]
let ``Blank frame names do not parse`` (frameName: string) =
    Assert.True((FSharpStackFrameNameParser.parse frameName).IsNone)

/// The compiler nests case classes and debugger proxies inside a union, and names some of them
/// with an `@` the way it names closures.
[<Fact>]
let ``No type the compiler nests in a union reads as a closure`` () =
    let methods =
        typeof<CallStackSample.Shape>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
        |> Array.collect
            _.GetMethods(
                BindingFlags.Instance
                ||| BindingFlags.Public
                ||| BindingFlags.NonPublic
                ||| BindingFlags.DeclaredOnly
            )

    Assert.NotEmpty methods

    for method in methods do
        for spelling in [ CallStackSample.reflectedName method; CallStackSample.debuggerName method ] do
            match FSharpStackFrameNameParser.parse spelling with
            | ValueSome { Member = FrameClosureBody _ } -> failwith $"%s{spelling} reads as a closure"
            | _ -> ()
