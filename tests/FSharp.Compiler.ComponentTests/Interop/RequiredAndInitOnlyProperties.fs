// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System

module ``Required and init-only properties`` =

    let csharpBaseClass = 
        CSharp """
    namespace RequiredAndInitOnlyProperties
    {
        public sealed class RAIO
        {
            public int GetSet { get; set; }
            public int GetInit { get; init; }
            public RAIO GetThis() => this;
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"

    let csharpRBaseClass = 
        CSharp """
    namespace RequiredAndInitOnlyProperties
    {
        public sealed class RAIO
        {
            public required int GetSet { get; set; }
            public required int GetInit { get; init; }
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"


#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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
    
#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can get property via calling an explicit getter`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet = 1, GetInit = 2)

    if raio.get_GetSet() <> 1 then
        failwith $"Unexpected result %d{raio.GetSet}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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

 #if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# cannot change init-only property via calling an initializer on instance`` () =

        let csharpLib = csharpBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO()
    raio.GetThis(GetSet=2, GetInit = 42) |> ignore
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 810, Line 9, Col 38, Line 9, Col 40, "Init-only property 'GetInit' cannot be set outside the initialization code. See https://aka.ms/fsharp-assigning-values-to-properties-at-initialization"
        ]


#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
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
        |> shouldFail
        |> withDiagnostics [
            Error 3545, Line 8, Col 16, Line 8, Col 22, "The following required properties have to be initalized:" + Environment.NewLine + "   property RAIO.GetSet: int with get, set" + Environment.NewLine + "   property RAIO.GetInit: int with get, set"
        ]

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# should produce compile-time error when some required properties are not specified in the initializer`` () =

        let csharpLib = csharpRBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet=1)

    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 3545, Line 8, Col 16, Line 8, Col 30, "The following required properties have to be initalized:" + Environment.NewLine + "   property RAIO.GetInit: int with get, set"
        ]

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# should not produce compile-time error when all required properties are specified in the initializer`` () =

        let csharpLib = csharpRBaseClass

        let fsharpSource =
            """
open System
open RequiredAndInitOnlyProperties

[<EntryPoint>]
let main _ =

    let raio = RAIO(GetSet=1, GetInit=2)

    if raio.GetSet <> 1 then
        failwith "Unexpected value"

    if raio.GetInit <> 2 then
        failwith "Unexpected value"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

    #if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
    #else
    [<Fact>]
    #endif
    let ``F# should only be able to explicitly call constructors which set SetsRequiredMembersAttribute`` () =

        let csharpLib =
            CSharp """        
        namespace RequiredAndInitOnlyProperties
        {
            using System.Runtime.CompilerServices;
            using System.Diagnostics.CodeAnalysis;

            public sealed class RAIO
            {
                public required int GetSet { get; set; }
                public required int GetInit { get; init; }
                [SetsRequiredMembers]
                public RAIO(int foo) {} // Should be legal to call any constructor which does have "SetsRequiredMembersAttribute"
                public RAIO(int foo, int bar) {} // Should be illegal to call any constructor which does not have "SetsRequiredMembersAttribute"
            }

        }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"

        let fsharpSource =
            """
    open System
    open RequiredAndInitOnlyProperties

    [<EntryPoint>]
    let main _ =
        let _raio = RAIO(1)
        0
    """
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

        let fsharpSource2 =
            """
    open System
    open RequiredAndInitOnlyProperties

    [<EntryPoint>]
    let main _ =
        let _raio = RAIO(1,2)
        0
    """
        FSharp fsharpSource2
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3545, Line 7, Col 21, Line 7, Col 30, "The following required properties have to be initalized:" + Environment.NewLine + "   property RAIO.GetSet: int with get, set" + Environment.NewLine + "   property RAIO.GetInit: int with get, set")

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# should produce a warning if RequiredMemberAttribute is specified`` () =
        let fsharpSource =
            """
namespace FooBarBaz
open System
open System.Runtime.CompilerServices
type RAIOFS() =
    [<RequiredMember>]
    member val GetSet = 0 with get, set
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 202, Line 6, Col 7, Line 6, Col 21, "This attribute is currently unsupported by the F# compiler. Applying it will not achieve its intended effect.")