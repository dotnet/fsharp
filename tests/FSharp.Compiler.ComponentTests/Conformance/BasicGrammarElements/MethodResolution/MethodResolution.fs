// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module MethodResolution =

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly (sanity test)`` () =
        FSharp """
open System.Runtime.InteropServices

type Thing =
    static member Do(o: outref<int>, [<Optional; DefaultParameterValue(7)>]i: int) =
        o <- i
        i = 7

// We expect return value to be false, and out value to be 42 here.
let returnvalue1, value1 = Thing.Do(i = 42)
// Have explicit boolean check for readability here:
if returnvalue1 <> false && value1 <> 42 then
    failwith "Mismatch: Return value should be false, and out value should be 42"

// Here, we expect return value to be true, and out value to be 7
let returnvalue2, value2 = Thing.Do()
// Have explicit boolean check for readability here:
if returnvalue2<> true && value2 <> 7 then
    failwith "Mismatch: Return value should be true, and out value should be 7"
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("OptionalAndOutParameters.fs", Realsig=BooleanOptions.Both)>]
    let ``OptionalAndOutParameters_fs`` compilation =
        compilation
        |> getCompilation
        |> ignoreWarnings
        |> verifyILBaseline

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
            (Error 501, Line 6, Col 12, Line 6, Col 22, "The member or object constructor 'Do' takes 1 argument(s) but is here given 0. The required signature is 'static member Thing.Do: [<Optional>] i: outref<bool> -> bool'.")
        ]

    [<Fact>]
    let ``optional and ParamArray parameter resolves correctly `` () =
        Fsx """
open System.Runtime.InteropServices
    
type Thing =
    static member Do(
        [<Optional; DefaultParameterValue "">] something: string, 
        [<System.ParamArray>] args: obj[]) = something, args
    static member Do2(
        [<Optional; DefaultParameterValue "">] something: string, 
        outvar: outref<int>,
        [<System.ParamArray>] args: obj[]) = 
        
        outvar <- 1
        something, args
let _, _ = Thing.Do()
let _, _ = Thing.Do("123")
let _, _ = Thing.Do("123", 1, 2, 3, 4)

let _, _ = Thing.Do2()
let _, _ = Thing.Do2("123")
let _ =
    let mutable x = 0
    Thing.Do2("123", &x)
let _ =
    let mutable x = 0
    Thing.Do2("123", &x, 1, 2, 3, 4)
    """
        |> typecheck
        |> shouldSucceed
