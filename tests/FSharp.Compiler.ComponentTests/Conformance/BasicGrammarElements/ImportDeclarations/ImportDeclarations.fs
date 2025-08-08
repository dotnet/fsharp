// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ImportDeclarations =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=E_OpenTwice.fs        SCFLAGS="--warnaserror+ --test:ErrorRanges"		# E_OpenTwice.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OpenTwice.fs"|])>]
    let ``E_OpenTwice_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 892, Line 11, Col 6, Line 11, Col 39, "This declaration opens the module 'Microsoft.FSharp.Collections.List', which is marked as 'RequireQualifiedAccess'. Adjust your code to use qualified references to the elements of the module instead, e.g. 'List.map' instead of 'map'. This change will ensure that your code is robust as new constructs are added to libraries.")
            (Error 892, Line 12, Col 6, Line 12, Col 39, "This declaration opens the module 'Microsoft.FSharp.Collections.List', which is marked as 'RequireQualifiedAccess'. Adjust your code to use qualified references to the elements of the module instead, e.g. 'List.map' instead of 'map'. This change will ensure that your code is robust as new constructs are added to libraries.")
        ]

    // SOURCE=E_OpenUnknownNS.fs					# E_OpenUnknownNS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OpenUnknownNS.fs"|])>]
    let ``E_OpenUnknownNS_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 7, Col 6, Line 7, Col 26, "The namespace or module 'SomeUnknownNamespace' is not defined.")
        ]

    // SOURCE=E_openEnum.fs       SCFLAGS="--test:ErrorRanges"		# E_openEnum.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openEnum.fs"|])>]
    let ``E_openEnum_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 20, Col 9, Line 20, Col 10, "The value or constructor 'A' is not defined.")
            (Error 39, Line 27, Col 9, Line 27, Col 10, "The value or constructor 'B' is not defined.")
            (Error 39, Line 34, Col 9, Line 34, Col 10, "The value or constructor 'C' is not defined.")
        ]

    // SOURCE=E_openInTypeInterface.fs SCFLAGS="--test:ErrorRanges"		# E_openInTypeInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openInTypeInterface.fs"|])>]
    let ``E_openInTypeInterface_fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 0010, Line 3, Col 5, Line 3, Col 9, "Unexpected keyword 'open' in member definition. Expected 'member', 'override', 'static' or other token.")
            (Error 3567, Line 3, Col 17, Line 4, Col 5, "Expecting member body")
        ]

    // SOURCE=E_openInTypeDecl.fs SCFLAGS="--test:ErrorRanges"		# E_openInTypeDecl.fs
    [<Theory(Skip = "'open's in type is not implemented yet"); Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openInTypeDecl.fs"|])>]
    let ``E_openInTypeDecl_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:52"] // The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3879, Line 23, Col 5, Line 23, Col 23, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 0039, Line 20, Col 15, Line 20, Col 26, "The type 'IDisposable' is not defined.")
            (Error 0039, Line 7, Col 13, Line 7, Col 19, "The type 'Object' is not defined. Maybe you want one of the following:\n   obj")
            (Error 0887, Line 20, Col 15, Line 20, Col 26, "The type 'obj' is not an interface type")
            (Error 0039, Line 9, Col 37, Line 9, Col 42, "The type 'Int64' is not defined. Maybe you want one of the following:\n   int64\n   uint64\n   int8\n   int\n   int16")
            (Error 0855, Line 21, Col 21, Line 21, Col 28, "No abstract or interface member was found that corresponds to this override")
            (Error 0039, Line 12, Col 21, Line 12, Col 26, "The value, namespace, type or module 'Int32' is not defined. Maybe you want one of the following:\n   int32\n   uint32\n   int8\n   int\n   int16")
            (Error 3879, Line 28, Col 9, Line 28, Col 20, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 3879, Line 33, Col 9, Line 33, Col 20, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 3879, Line 38, Col 9, Line 38, Col 20, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 0039, Line 37, Col 44, Line 37, Col 50, "The value or constructor 'Random' is not defined.")
            (Error 0039, Line 40, Col 15, Line 40, Col 20, "The type 'Int32' is not defined. Maybe you want one of the following:\n   int32\n   uint32\n   int8\n   int\n   int16")
            (Warning 1178, Line 40, Col 6, Line 40, Col 7, "The struct, record or union type 'B' is not structurally comparable because the type 'obj' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'B' to clarify that the type is not comparable")
            (Error 0039, Line 44, Col 25, Line 44, Col 30, "The type 'Int32' is not defined. Maybe you want one of the following:\n   int32\n   uint32\n   int8\n   int\n   int16")
            (Error 0039, Line 48, Col 22, Line 48, Col 27, "The type 'Int32' is not defined. Maybe you want one of the following:\n   int32\n   uint32\n   int8\n   int\n   int16")
            (Warning 1178, Line 48, Col 6, Line 48, Col 13, "The struct, record or union type 'BRecord' is not structurally comparable because the type 'obj' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'BRecord' to clarify that the type is not comparable")
            (Error 3879, Line 59, Col 5, Line 59, Col 16, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 3879, Line 63, Col 5, Line 63, Col 26, "'open' declarations must come before all other definitions in type definitions or augmentation")
            (Error 0039, Line 71, Col 20, Line 71, Col 27, "The value, namespace, type or module 'Console' is not defined. Maybe you want one of the following:\n   Control\n   cosh")
        ]
        
    // SOURCE=openInTypeDecl.fs 		# openInTypeDecl.fs
    [<Theory(Skip = "'open's in type is not implemented yet"); Directory(__SOURCE_DIRECTORY__, Includes=[|"openInTypeDecl.fs"|])>]
    let ``openInTypeDecl_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:52"] // The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    // SOURCE=openModInFun.fs   		# openModInFun.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"openModInFun.fs"|])>]
    let ``openModInFun_fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    // SOURCE=OpenNestedModule01.fs					# OpenNestedModule01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OpenNestedModule01.fs"|])>]
    let ``OpenNestedModule01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_OpenUnqualifiedNamespace01.fs				# W_OpenUnqualifiedNamespace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_OpenUnqualifiedNamespace01.fs"|])>]
    let ``W_OpenUnqualifiedNamespace01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 893, Line 7, Col 6, Line 7, Col 13, "This declaration opens the namespace or module 'System.Collections.Generic' through a partially qualified path. Adjust this code to use the full path of the namespace. This change will make your code more robust as new constructs are added to the F# and CLI libraries.")
        ]

    // SOURCE=openDU.fs						# openDU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"openDU.fs"|])>]
    let ``openDU_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=openSystem01.fs							# openSystem01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"openSystem01.fs"|])>]
    let ``openSystem01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ImplementImportedAbstractBaseMethodsFailsIfUsed ()=
        FSharp """
module Testing

open System.Text.Json
open System.Text.Json.Serialization

type StringTrimJsonSerializer(o: JsonSerializerOptions) =
    inherit JsonConverter<string>()

    override this.Read(reader, _, _) =
        match reader.TokenType with
        | JsonTokenType.String -> reader.GetString().Trim()
        | _ -> JsonException("Type is not a string") |> raise

    override this.Write(writer, objectToWrite, options) = base.Write(writer, objectToWrite, options)

type SomeType = { AField: string }

let serialize item =
    let options = JsonSerializerOptions()
    StringTrimJsonSerializer options |> options.Converters.Add
    JsonSerializer.Serialize(item, options)

[<EntryPoint>]
let main _ =
    { AField = "a" }
    |> serialize
    |> ignore
    0"""
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1201, Line 15, Col 59, Line 15, Col 101, "Cannot call an abstract base member: 'Write'")
        ]


    [<FactForNETCOREAPP>]
    let ImplementImportedAbstractBaseMethodsFailsIfNotUsed ()=
        FSharp """
module Testing

open System.Text.Json
open System.Text.Json.Serialization

type StringTrimJsonSerializer(o: JsonSerializerOptions) =
    inherit JsonConverter<string>()
    override this.Read(reader, _, _) =
        match reader.TokenType with
        | JsonTokenType.String -> reader.GetString().Trim()
        | _ -> JsonException("Type is not a string") |> raise
    override this.Write(writer, objectToWrite, options) = base.Write(writer, objectToWrite, options)

type SomeType = { AField: int }

let serialize item =
    let options = JsonSerializerOptions()
    StringTrimJsonSerializer options |> options.Converters.Add
    JsonSerializer.Serialize(item, options)

[<EntryPoint>]
let main _ =
    { AField = 1 }
    |> serialize
    |>ignore
    0"""
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1201, Line 13, Col 59, Line 13, Col 101, "Cannot call an abstract base member: 'Write'")
        ]
