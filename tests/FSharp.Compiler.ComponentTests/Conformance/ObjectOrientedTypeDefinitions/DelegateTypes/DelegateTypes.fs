// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DelegateTypes =

    // Error tests

    [<Theory; FileInlineData("E_InvalidSignature01.fs")>]
    let ``E_InvalidSignature01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 949

    [<Theory; FileInlineData("E_InvalidSignature02.fs")>]
    let ``E_InvalidSignature02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 950

    // Success tests

    [<Theory; FileInlineData("ByrefArguments01.fs")>]
    let ``ByrefArguments01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValidSignature_MultiArg01.fs")>]
    let ``ValidSignature_MultiArg01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValidSignature_ReturningValues01.fs")>]
    let ``ValidSignature_ReturningValues01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // PRECMD test migrated from fsharpqa/Source/Conformance/ObjectOrientedTypeDefinitions/DelegateTypes
    // Original: PRECMD="$CSC_PIPE /t:library IDelegateBinding.cs" SCFLAGS="-r:IDelegateBinding.dll"
    // Regression for SP1 bug 40222 in Devdiv2(dev11) - delegate binding invocation
    [<Fact>]
    let ``DelegateBindingInvoke01 - C# delegate binding interface`` () =
        let csharpLib =
            CSharp """
using System;

namespace Ninject.Planning.Bindings
{
    public interface IRequest
    {
    }

    public interface IBinding
    {
        Type Service { get; }
        Func<IRequest, bool> Condition { get; set; }
    }

    public class Binding : IBinding
    {
        public Type Service { get; private set; }
        public Func<IRequest, bool> Condition { get; set; }
    }

    public class Request : IRequest
    {
    }
}
"""
            |> withName "IDelegateBinding"

        FSharp """
open Ninject.Planning.Bindings
 
// Verify the runtime doesn't throw InvalidCastException when invoking delegate binding.
let DelegateBindingInvoke = 
    try
        let h = new Binding( Condition = fun x -> true );
 
        let k = new Binding();
        k.Condition <- fun x -> false
 
        // Invoke the 2 Conditions and verify runtime behavior
        let t1 = k.Condition.Invoke(new Request())
        let t2 = h.Condition.Invoke(new Request())
        if t1 = false && t2 = true then 0 else 1
    with
        | :? System.InvalidCastException as e -> printfn "InvalidCastException caught!" 
                                                 1
"""
        |> asExe
        |> withReferences [csharpLib]
        |> compileExeAndRun
        |> shouldSucceed
