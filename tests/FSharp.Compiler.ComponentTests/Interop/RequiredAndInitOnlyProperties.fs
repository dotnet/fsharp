// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ``Required and init-only properties`` =

    let withCSharpLanguageVersion (ver: CSharpLanguageVersion) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | CS cs -> CS { cs with LangVersion = ver }
        | _ -> failwith "Only supported in C#"


    let csharpBaseClass = 
        CSharp """
    namespace RequiredAndInitOnlyProperties
    {
        public sealed class RAIO
        {
            public int GetSet { get; set; }
            public int GetInit { get; init; }
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"

    let csharpRBaseClass = 
        CSharp """
    // Until we move to .NET7 runtime (or use experimental)
    namespace System.Runtime.CompilerServices
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public sealed class RequiredMemberAttribute : Attribute { }
        [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
        public sealed class CompilerFeatureRequiredAttribute : Attribute
        {
            public CompilerFeatureRequiredAttribute(string featureName)
            {
                FeatureName = featureName;
            }
            public string FeatureName { get; }
            public bool IsOptional { get; init; }
            public const string RefStructs = nameof(RefStructs);
            public const string RequiredMembers = nameof(RequiredMembers);
        }
    }
    
    namespace RequiredAndInitOnlyProperties
    {
        public sealed class RAIO
        {
            public required int GetSet { get; set; }
            public required int GetInit { get; init; }
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"


    [<Fact>]
    let ``F# can init both set and init-only`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)

    if raio.GetSet <> 1 then
        failwith $"Unexpected result %d{raio.GetSet}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``F# can change set property`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)

    if raio.GetSet <> 1 then
        failwith $"Unexpected result %d{raio.GetSet}"

    raio.GetSet <- 0

    if raio.GetSet <> 0 then
        failwith $"Unexpected result %d{raio.GetSet}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed
    
    [<Fact>]
    let ``F# can change set property via calling an explicit setter`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)

    if raio.GetSet <> 1 then
        failwith $"Unexpected result %d{raio.GetSet}"

    raio.set_GetSet(0)

    if raio.GetSet <> 0 then
        failwith $"Unexpected result %d{raio.GetSet}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``F# can get property via calling an explicit getter`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)

    if raio.get_GetSet <> 1 then
        failwith $"Unexpected result %d{raio.GetSet}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``F# cannot change init-only property`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)
    raio.GetInit <- 0

    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 810, Line 9, Col 5, Line 9, Col 17, "Init-only property 'GetInit' cannot be set outside the initialization code. See https://aka.ms/fsharp-assigning-values-to-properties-at-initialization"
        ]

    [<Fact>]
    let ``F# cannot change init-only property via calling an explicit setter`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)
    raio.set_GetInit(0)

    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 810, Line 9, Col 5, Line 9, Col 21, "Cannot call 'set_GetInit' - a setter for init-only property, please use object initialization instead. See https://aka.ms/fsharp-assigning-values-to-properties-at-initialization"
        ]


    [<Fact>]
    let ``F# should produce compile-time error when required properties are not specified in the initializer`` () =

        let csharpLib = csharpRBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO()

    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed