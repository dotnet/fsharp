// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Simpleresolution =

    // dotnet/fsharp#8509: on .NET Core, --simpleresolution previously triggered
    // a flood of FS0078 "Unable to find the file" errors for .NET Framework
    // assemblies (mscorlib, System.dll, System.Windows.Forms, ...). After the
    // fix, the option is recognised but ignored on coreclr with a single
    // warning (FS3888) and the compilation succeeds.

    let private helloWorld = """
module M
let main () = printfn "hello"
"""

    [<FactForNETCOREAPP>]
    let ``--simpleresolution on coreclr warns and compiles successfully``() =
        FSharp helloWorld
        |> asExe
        |> withOptions ["--simpleresolution"]
        |> compile
        |> shouldSucceed
        |> withWarningCode 3888
        |> withDiagnosticMessageMatches "simpleresolution"
        |> ignore

    [<FactForNETCOREAPP>]
    let ``--simpleresolution on coreclr does not emit FS0078``() =
        FSharp helloWorld
        |> asExe
        |> withOptions ["--simpleresolution"]
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ Warning 3888, Line 0, Col 1, Line 0, Col 1, "The --simpleresolution option is not supported on .NET Core; ignoring." ]
        |> ignore

    [<FactForNETCOREAPP>]
    let ``--simpleresolution combined with --noframework on coreclr still emits the single FS3888 warning``() =
        FSharp helloWorld
        |> asExe
        |> withOptions ["--simpleresolution"; "--noframework"]
        |> compile
        |> shouldSucceed
        |> withWarningCode 3888
        |> ignore

    [<FactForNETCOREAPP>]
    let ``--simpleresolution on coreclr with explicit -r still compiles``() =
        FSharp helloWorld
        |> asExe
        |> withOptions ["--simpleresolution"; "-r:System.Net.Http.dll"]
        |> compile
        |> shouldSucceed
        |> withWarningCode 3888
        |> ignore

    // Negative guard: a genuinely-missing user reference must still produce FS0078,
    // even with --simpleresolution. This ensures the fix only suppresses the
    // framework-resolution failures, not the user-driven ones.
    [<FactForNETCOREAPP>]
    let ``--simpleresolution on coreclr still reports FS0078 for a user-supplied missing -r``() =
        FSharp helloWorld
        |> asExe
        |> withOptions ["--simpleresolution"; "-r:Nonexistent.Definitely.Missing.dll"]
        |> compile
        |> shouldFail
        |> withErrorCode 78
        |> ignore
