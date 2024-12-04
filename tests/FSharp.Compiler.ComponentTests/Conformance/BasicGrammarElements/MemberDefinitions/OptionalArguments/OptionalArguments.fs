// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDefinitions_OptionalArguments =

    let csTestLib =
        CSharpFromPath (__SOURCE_DIRECTORY__ ++ "TestLib.cs")
        |> withName "CSLibraryWithAttributes"

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

    // SOURCE=E_OptionalNamedArgs.fs SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# E_OptionalNamedArgs.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OptionalNamedArgs.fs"|])>]
    let ``E_OptionalNamedArgs_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 7, Col 42, Line 7, Col 48, "This expression was expected to have type\n    'string option'    \nbut here has type\n    'string'    ")
        ]

    // SOURCE=NullOptArgsFromCS.fs   SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# NullOptArgsFromCS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullOptArgsFromCS.fs"|])>]
    let ``NullOptArgsFromCS_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=NullOptArgsFromVB.fs   SCFLAGS="-r:TestLibVB.dll" PRECMD="\$VBC_PIPE /t:library TestLibVB.vb"	# NullOptArgsFromVB.fs
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullOptArgsFromVB.fs"|])>]
    //let ``NullOptArgsFromVB_fs`` compilation =
    //    compilation
    //    |> verifyCompileAndRun
    //    |> shouldSucceed

    // SOURCE=OptionalArgOnAbstract01.fs			# OptionalArgOnAbstract01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OptionalArgOnAbstract01.fs"|])>]
    let ``OptionalArgOnAbstract01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityOptArgsFromCS.fs SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# SanityOptArgsFromCS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityOptArgsFromCS.fs"|])>]
    let ``SanityOptArgsFromCS_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SanityCheck.fs"|])>]
    let ``E_SanityCheck_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1212, Line 7, Col 22, Line 7, Col 32, "Optional arguments must come at the end of the argument list, after any non-optional arguments")
            (Error 39, Line 8, Col 5, Line 8, Col 8, "The type 'Foo' does not define the field, constructor or member 'Bar'.")
        ]

    // SOURCE=E_SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SanityCheck02.fs"|])>]
    let ``E_SanityCheck02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1212, Line 7, Col 22, Line 7, Col 29, "Optional arguments must come at the end of the argument list, after any non-optional arguments")
            (Error 39, Line 8, Col 5, Line 8, Col 8, "The type 'Foo' does not define the field, constructor or member 'Bar'.")
        ]

    // SOURCE=optionalOfOptOptA.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"optionalOfOptOptA.fs"|])>]
    let ``optionalOfOptOptA_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck03.fs"|])>]
    let ``SanityCheck03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Optional Arguments can't be a ValueOption+StructAttribute attribute with langversion=9`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M([<StructAttribute>] ?x) =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X.M(ValueSome 1)
    X.M(ValueNone)
    X.M()
    0
            """
        source
            |> asLibrary
            |> withLangVersion90
            |> withNoWarn 25
            |> compile
            |> shouldFail
            |> withDiagnostics [
                Error 1, Line 6, Col 11, Line 6, Col 22, "This expression was expected to have type
    ''a option'    
but here has type
    ''b voption'    "
                Error 1, Line 7, Col 11, Line 7, Col 20, "This expression was expected to have type
    ''a option'    
but here has type
    ''b voption'    "]

    [<Fact>]
    let ``Optional Arguments wrap Option`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M(?x) =
        match x with
        | Some x -> printfn "Some %A" x
        | None -> printfn "None"

[<EntryPoint>]
let main _ =
    X.M(Some 1)
    X.M(None)
    X.M(1)
    X.M()
    0
            """
        source
            |> asExe
            |> withLangVersionPreview
            |> withNoWarn 25
            |> compile
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["Some Some 1"; "Some None"; "Some 1"; "None"]

    [<Fact>]
    let ``Optional Arguments wrap ValueOption`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M(?x) =
        match x with
        | Some x -> printfn "Some %A" x
        | None -> printfn "None"

[<EntryPoint>]
let main _ =
    X.M(ValueSome 1)
    X.M(ValueNone)
    X.M(1)
    X.M()
    0
            """
        source
            |> asExe
            |> withLangVersionPreview
            |> withNoWarn 25
            |> compile
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["Some ValueSome 1"; "Some ValueNone"; "Some 1"; "None"]

    [<Fact>]
    let ``Optional Struct Arguments wrap Option`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M([<Struct>] ?x) =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X.M(Some 1)
    X.M(None)
    X.M(1)
    X.M()
    0
            """
        source
            |> asExe
            |> withLangVersionPreview
            |> withNoWarn 25
            |> compile
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome Some 1"; "VSome None"; "VSome 1"; "VNone"]

    [<Fact>]
    let ``Optional Struct Arguments wrap ValueOption`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M([<Struct>] ?x) =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X.M(ValueSome 1)
    X.M(ValueNone)
    X.M(1)
    X.M()
    0
            """
        source
            |> asExe
            |> withLangVersionPreview
            |> withNoWarn 25
            |> compile
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome ValueSome 1"; "VSome ValueNone"; "VSome 1"; "VNone"]


    [<Fact>]
    let ``Optional Arguments can be a ValueOption+StructAttribute attribute with langversion=preview`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M([<StructAttribute>] ?x) =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X.M(ValueSome 1)
    X.M(ValueNone)
    X.M(1)
    X.M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome ValueSome 1"; "VSome ValueNone"; "VSome 1"; "VNone"]
            |> verifyIL ["""
.method public static void  M<a>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a> x) cil managed
{
    .param [1]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (!!a V_0,
            class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1,
            class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2)
    IL_0000:  ldarga.s   x
    IL_0002:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0032

    IL_000c:  ldarga.s   x
    IL_000e:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>::get_Item()
    IL_0013:  stloc.0
    IL_0014:  ldstr      "VSome %A"
    IL_0019:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::.ctor(string)
    IL_001e:  stloc.1
    IL_001f:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0024:  ldloc.1
    IL_0025:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002a:  ldloc.0
    IL_002b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0030:  pop
    IL_0031:  ret

    IL_0032:  ldstr      "VNone"
    IL_0037:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_003c:  stloc.2
    IL_003d:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0042:  ldloc.2
    IL_0043:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [runtime]System.IO.TextWriter,
                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0048:  pop
    IL_0049:  ret
}
        """]


    [<Fact>]
    let ``Optional Arguments can be a ValueOption+Struct attribute with langversion=preview`` () =
        let source =
            FSharp """
module Program
type X() =
    static member M([<StructAttribute>] ?x) =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X.M(ValueSome 1)
    X.M(ValueNone)
    X.M(1)
    X.M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome ValueSome 1"; "VSome ValueNone"; "VSome 1"; "VNone"]
            |> verifyIL ["""
.method public static void  M<a>(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a> x) cil managed
{
    .param [1]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (!!a V_0,
            class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1,
            class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2)
    IL_0000:  ldarga.s   x
    IL_0002:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0032

    IL_000c:  ldarga.s   x
    IL_000e:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>::get_Item()
    IL_0013:  stloc.0
    IL_0014:  ldstr      "VSome %A"
    IL_0019:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::.ctor(string)
    IL_001e:  stloc.1
    IL_001f:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0024:  ldloc.1
    IL_0025:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [runtime]System.IO.TextWriter,
                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002a:  ldloc.0
    IL_002b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0030:  pop
    IL_0031:  ret

    IL_0032:  ldstr      "VNone"
    IL_0037:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_003c:  stloc.2
    IL_003d:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0042:  ldloc.2
    IL_0043:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [runtime]System.IO.TextWriter,
                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0048:  pop
    IL_0049:  ret
}
        """]

    [<Fact>]
    let ``Optional Arguments in constructor wrap Option`` () =
        let source =
            FSharp """
module Program
type X<'T>(?x: 'T) =
    member _.M() =
        match x with
        | Some x -> printfn "Some %A" x
        | None -> printfn "None"

[<EntryPoint>]
let main _ =
    X(Some 1).M()
    X(None).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["Some Some 1"; "Some None"; "Some 1"; "None"]

    [<Fact>]
    let ``Optional Arguments in constructor wrap ValueOption`` () =
        let source =
            FSharp """
module Program
type X<'T>(?x: 'T) =
    member _.M() =
        match x with
        | Some x -> printfn "Some %A" x
        | None -> printfn "None"

[<EntryPoint>]
let main _ =
    X(ValueSome 1).M()
    X(ValueNone).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["Some ValueSome 1"; "Some ValueNone"; "Some 1"; "None"]

    [<Fact>]
    let ``Optional Struct Arguments in constructor wrap Option`` () =
        let source =
            FSharp """
module Program
type X<'T>([<Struct>] ?x: 'T) =
    member _.M() =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X(Some 1).M()
    X(None).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome Some 1"; "VSome None"; "VSome 1"; "VNone"]

    [<Fact>]
    let ``Optional Struct Arguments in constructor wrap ValueOption`` () =
        let source =
            FSharp """
module Program
type X<'T>([<Struct>] ?x: 'T) =
    member _.M() =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X(ValueSome 1).M()
    X(ValueNone).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome ValueSome 1"; "VSome ValueNone"; "VSome 1"; "VNone"]

    [<Fact>]
    let ``Optional Arguments in constructor can be a ValueOption+StructAttribute attribute with langversion=preview`` () =
        let source =
            FSharp """
module Program
type X<'T>([<Struct>] ?x: 'T) =
    member _.M() =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X(ValueSome 1).M()
    X(ValueNone).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersionPreview
            |> asExe
            |> compile

        compilation
            |> shouldSucceed
            |> run
            |> shouldSucceed
            |> withOutputContainsAllInOrder ["VSome ValueSome 1"; "VSome ValueNone"; "VSome 1"; "VNone"]
            
    [<Fact>]
    let ``Optional Arguments in constructor can't be a ValueOption+StructAttribute attribute with langversion=90`` () =
        let source =
            FSharp """
module Program
type X<'T>([<Struct>] ?x: 'T) =
    member _.M() =
        match x with
        | ValueSome x -> printfn "VSome %A" x
        | ValueNone -> printfn "VNone"

[<EntryPoint>]
let main _ =
    X(ValueSome 1).M()
    X(ValueNone).M()
    X(1).M()
    X().M()
    0
            """
        let compilation =
            source
            |> withLangVersion90
            |> withNoWarn 25
            |> asLibrary
            |> compile

        compilation
            |> shouldFail
            |> withDiagnostics [
                Error 1, Line 6, Col 11, Line 6, Col 22, "This expression was expected to have type
    ''T option'    
but here has type
    ''a voption'    "
                Error 1, Line 7, Col 11, Line 7, Col 20, "This expression was expected to have type
    ''T option'    
but here has type
    ''a voption'    "
            ]