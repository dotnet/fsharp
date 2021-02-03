// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes
open FSharp.Test.Utilities

module AssemblySigning =

#if !NETCOREAPP
    [<Fact>]
#endif
    let ``--keycontainer:foo`` () =
        FSharp """
module SignMe

open System
open System.Reflection

            """
        |> withOptions ["--keycontainer:myContainer"]
        |> compile
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--keycontainer' has been deprecated. Use '--keyfile' instead."
        |> ignore


    //Expects: warning FS3392: The 'AssemblyKeyNameAttribute' has been deprecated. Use 'AssemblyKeyFileAttribute' instead.
#if !NETCOREAPP
    [<Fact>]
#endif
    let AssemblyKeyNameAttribute () =
        FSharp """
module SignMe

open System
open System.Reflection

[<assembly:AssemblyKeyNameAttribute("myContainer")>]
do ()
            """
             |> asFs
             |> compile
             |> shouldFail
             |> withWarningCode 3392
             |> withDiagnosticMessageMatches "The 'AssemblyKeyNameAttribute' has been deprecated. Use 'AssemblyKeyFileAttribute' instead."
             |> ignore
