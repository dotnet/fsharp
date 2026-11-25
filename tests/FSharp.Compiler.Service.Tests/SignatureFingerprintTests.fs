// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.SignatureFingerprintTests

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Test.Assert
open Xunit

let private fingerprintOfModule (source: string) =
    let options = createProjectOptionsFromNamedSources [ "Test.fs", source ] []
    let filePath = options.SourceFiles[0]
    let _, checkResults = parseAndCheckFile filePath source options
    let entity = checkResults.FileSignature.FindEntityByPath [ "Test" ] |> Option.get

    let tcref =
        match entity.Item with
        | Item.ModuleOrNamespaces [ tcref ] -> tcref
        | item -> failwith $"Unexpected item %A{item}"

    Construct.NewModuleOrNamespaceType (Namespace true) [ tcref.Deref ] []

let private moduleFingerprint (source: string) =
    let mty = fingerprintOfModule source
    SignatureFingerprint.compute mty EmptyTopAttrs |> Option.get

[<Fact>]
let ``accessPathText matches sprintf formatting for short, long, and non-plain paths`` () =
    let paths =
        [ []
          [ "Ns", Namespace true; "Mod", ModuleOrType; "Sub", FSharpModuleWithSuffix ]
          [ "VeryLongNamespaceSegmentOne", Namespace false
            "VeryLongModuleSegmentTwo", ModuleOrType
            "VeryLongNestedModuleSegmentThree", ModuleOrType ]
          [ "with space", ModuleOrType; "with-dash", FSharpModuleWithSuffix ] ]

    for path in paths do
        let compPath = CompPath(ILScopeRef.Local, SyntaxAccess.Unknown, path)
        SignatureFingerprint.accessPathText compPath |> shouldEqual (sprintf "%A" compPath.AccessPath)

[<Fact>]
let ``compute is invariant under body, comment, whitespace, and line changes`` () =
    let before =
        """
module Test

let compute (x: int) = x + 1

type Box(value: int) =
    member _.Value = value + 10
"""

    let after =
        """
module Test


/// Added XML doc comment
let compute (x: int) =
    // Changed local implementation details and line positions
    let doubled = x * 2
    doubled - x + 1

// Another comment
type Box(value: int) =
    member _.Value =
        let bias = 5 + 5
        value + bias
"""

    moduleFingerprint after |> shouldEqual (moduleFingerprint before)

[<Fact>]
let ``compute distinguishes signature changes in types, parameter names, literals, accessibility, and attributes`` () =
    let baseline =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 1

let add (left: int) (right: int) = left + right

[<Obsolete("v1")>]
let tagged () = ()
"""

    let returnTypeChanged =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 1

let add (left: int) (right: int) = float (left + right)

[<Obsolete("v1")>]
let tagged () = ()
"""

    let paramNameChanged =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 1

let add (lhs: int) (right: int) = lhs + right

[<Obsolete("v1")>]
let tagged () = ()
"""

    let literalChanged =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 2

let add (left: int) (right: int) = left + right

[<Obsolete("v1")>]
let tagged () = ()
"""

    let accessChanged =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 1

let internal add (left: int) (right: int) = left + right

[<Obsolete("v1")>]
let tagged () = ()
"""

    let attribChanged =
        moduleFingerprint """
module Test

open System

[<Literal>]
let Version = 1

let add (left: int) (right: int) = left + right

[<Obsolete("v2")>]
let tagged () = ()
"""

    Assert.NotEqual<string>(baseline, returnTypeChanged)
    Assert.NotEqual<string>(baseline, paramNameChanged)
    Assert.NotEqual<string>(baseline, literalChanged)
    Assert.NotEqual<string>(baseline, accessChanged)
    Assert.NotEqual<string>(baseline, attribChanged)

[<Fact>]
let ``computeDeclaration returns None when a member's slots are not yet available`` () =
    let mty =
        fingerprintOfModule """
module Test

type C() =
    member _.Prop = 42
"""

    let withoutSlots = SignatureFingerprint.computeDeclaration mty [] (fun _ _ -> None)
    Assert.True(withoutSlots.IsNone)

    let withSlots = SignatureFingerprint.computeDeclaration mty [] (fun _ info -> Some(info.ImplementedSlotSigs, info.IsImplemented))
    Assert.True(withSlots.IsSome)
