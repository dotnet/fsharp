// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.MethodResolution

open Xunit
open FSharp.Test.Compiler

module ParametersResolution =

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly`` () =
        Fsx """
open System.Runtime.InteropServices

type Thing =
    static member Do(_: outref<bool>, [<Optional; DefaultParameterValue(1)>]i: int) = true
let _, _ = Thing.Do(i = 1)
let _, _ = Thing.Do()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly (examples from original issue: https://github.com/dotnet/fsharp/issues/12515)`` () =
        Fsx """
open System.Runtime.InteropServices;

// Define a member with both outref and default parameters. The compiler's implicit outref handling can handle this
// if required and optional parameters are provided, but not if the default parameters are left out

type Thing =
    static member Do(x: int,
                     fast: outref<bool>,
                     think: outref<float>, 
                     [<Optional;
                       DefaultParameterValue(System.Threading.CancellationToken())>] 
                     token: System.Threading.CancellationToken
                    ) : bool = 
                     true
     static member Also(x: int,
                     [<Optional;
                       DefaultParameterValue(System.Threading.CancellationToken())>] 
                     token: System.Threading.CancellationToken,
                     fast: outref<bool>,
                     think: outref<float>                     
                    ) : bool = true

// Works, was error because we can't strip the default `token` parameter for some reason
let ok, fast, think = Thing.Do(1)

// works because the outrefs are detected and provided by the compiler
let ok2, fast2, think2 = Thing.Do(1, token = System.Threading.CancellationToken.None)

// Works, was error because we can't strip the default `token` parameter for some reason
let ok3, fast3, think3 = Thing.Also(1)

// works because the outrefs are detected and provided by the compiler
let ok4, fast4, think4 = Thing.Also(1, token = System.Threading.CancellationToken.None)

// works but requires a lot of work for the user
let mutable fast5 = Unchecked.defaultof<bool>
let mutable think5 = Unchecked.defaultof<float>

let ok5 = Thing.Do(1, &fast5, &think5)
        """
    [<Fact>]
    let ``Method with same optional and out parameter does not resolve`` () =
        Fsx """
open System.Runtime.InteropServices
        
type Thing =
    static member Do([<Optional>]i: outref<bool>) = true
let _, _ = Thing.Do()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 12, Line 6, Col 22, "The member or object constructor 'Do' takes 1 argument(s) but is here given 0. The required signature is 'static member Thing.Do: i: outref<bool> -> bool'.")
        ]

