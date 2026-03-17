// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Signatures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open TestFramework

/// Tests for Signature conformance - migrated from tests/fsharpqa/Source/Conformance/Signatures/SignatureConformance/
module SignatureConformance =

    let private resourcePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ "resources" ++ "tests" ++ "Conformance" ++ "Signatures" ++ "SignatureConformance"

    // Regression test for DevDiv:266717 - "Unable to compile .fs/.fsi with literal values"
    // SOURCE="Literal01.fsi Literal01.fs" SCFLAGS=-a
    [<Fact>]
    let ``Literal01 - literal values in signature`` () =
        FsFromPath (resourcePath ++ "Literal01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "Literal01.fs"))
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Verify ability to use FSI files in conjunction with internal types
    // SOURCE="InternalAccessibility01.fsi InternalAccessibility01.fs"
    [<Fact>]
    let ``InternalAccessibility01 - internal types with signature`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "InternalAccessibility01.fs"))
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression test for FSHARP1.0:4155 - combined accessibilities internal --> internal give "private"
    // SOURCE="InternalAccessibility02.fsi InternalAccessibility02.fs" SCFLAGS="--warnaserror+"
    // Note: Original test uses PRECMD to generate .fsi from --sig. This test compiles standalone.
    // When compiled as part of a multi-file compilation, this code works - so we use asExe to test it
    [<Fact>]
    let ``InternalAccessibility02 - combined internal accessibilities`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility02.fs")
        |> withOptions ["--warnaserror+"]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // Verify ability to use FSI files in conjunction with internal types (interface case)
    // SOURCE="ImplementsComparable.fsi ImplementsComparable.fs"
    [<Fact>]
    let ``ImplementsComparable - interface hidden by signature`` () =
        FsFromPath (resourcePath ++ "ImplementsComparable.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "ImplementsComparable.fs"))
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression for FSHARP1.0:5852 - Overly strict checks when a type is implicitly hidden by signature
    // SOURCE="InternalAccessibility03.fsi InternalAccessibility03.fs"
    [<Fact>]
    let ``InternalAccessibility03 - implicit internal types`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility03.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "InternalAccessibility03.fs"))
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression for 5618 - bad error message when LSS doesn't implement member Bar
    // SOURCE="MissingMethodInImplementation01.fsi MissingMethodInImplementation01.fs" SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" span="(7,8-7,13)" id="FS0193">Module 'MyNS\.File2' requires a value 'member File2\.LSS\.Bar: string -> int'$</Expects>
    [<Fact>]
    let ``MissingMethodInImplementation01 - missing member in implementation`` () =
        FsFromPath (resourcePath ++ "MissingMethodInImplementation01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "MissingMethodInImplementation01.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "requires a value 'member File2.LSS.Bar"
        |> ignore

    // Regression for 6446 - verifying spec matches implementation when fs/fsi files attributes differ
    // SOURCE="AttributeMatching01.fsi AttributeMatching01.fs" SCFLAGS="--test:ErrorRanges --warnaserror"
    // <Expects status="error" id="FS1200" span="(17,7-17,23)">The attribute 'ObsoleteAttribute' appears in both the implementation and the signature, but the attribute arguments differ</Expects>
    [<Fact>]
    let ``AttributeMatching01 - attribute mismatch between signature and implementation`` () =
        FsFromPath (resourcePath ++ "AttributeMatching01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "AttributeMatching01.fs"))
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> compile
        |> shouldFail
        |> withErrorCode 1200
        |> withDiagnosticMessageMatches "ObsoleteAttribute"
        |> ignore

    // Regression for Dev11:137930 - structs used to not give errors on unimplemented constructors
    // SOURCE="E_StructConstructor01.fsi E_StructConstructor01.fs" SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0193" span="(4,8-4,9)">Module 'M' requires a value 'new: unit -> Foo<'T>'</Expects>
    [<Fact>]
    let ``E_StructConstructor01 - struct missing constructor`` () =
        FsFromPath (resourcePath ++ "E_StructConstructor01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructConstructor01.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "requires a value"
        |> ignore

    // Regression for Dev11:137942 - structs used to not give errors when member names conflicted with interface members
    // SOURCE="E_StructWithNameConflict01.fsi E_StructWithNameConflict01.fs" SCFLAGS="--test:ErrorRanges --flaterrors"
    // <Expects status="error" span="(14,21-14,34)" id="FS0034">...</Expects>
    // <Expects status="notin" span="(18,13-18,26)" id="FS0039">The field, constructor or member 'GetEnumerator' is not defined$</Expects>
    [<Fact>]
    let ``E_StructWithNameConflict01 - struct interface member name conflict`` () =
        FsFromPath (resourcePath ++ "E_StructWithNameConflict01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructWithNameConflict01.fs"))
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0034
        |> withDiagnosticMessageDoesntMatch "The field, constructor or member 'GetEnumerator' is not defined"
        |> ignore

    // Regression for Dev11:137942 - structs used to not give errors when member names conflicted with interface members
    // SOURCE="E_StructWithNameConflict02.fsi E_StructWithNameConflict02.fs" SCFLAGS="--test:ErrorRanges --flaterrors"
    // <Expects status="error" span="(18,13-18,26)" id="FS0039">The type 'Foo<_>' does not define the field, constructor or member 'GetEnumerator'</Expects>
    // <Expects status="notin" span="(14,21-14,34)" id="FS0034">...</Expects>
    [<Fact>]
    let ``E_StructWithNameConflict02 - struct undefined member from signature`` () =
        FsFromPath (resourcePath ++ "E_StructWithNameConflict02.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructWithNameConflict02.fs"))
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "GetEnumerator"
        |> withDiagnosticMessageDoesntMatch "The compiled names differ"
        |> ignore
