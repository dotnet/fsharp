// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AbstractMembers =

    // Error tests

    [<Theory; FileInlineData("E_CallToAbstractMember01.fs")>]
    let ``E_CallToAbstractMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_CallToAbstractMember02.fs")>]
    let ``E_CallToAbstractMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 365

    [<Theory; FileInlineData("E_CallToAbstractMember03.fs")>]
    let ``E_CallToAbstractMember03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 365

    [<Theory; FileInlineData("E_CallToAbstractMember04.fs")>]
    let ``E_CallToAbstractMember04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_CallToUnimplementedMethod01.fs")>]
    let ``E_CallToUnimplementedMethod01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_InlineVirtualMember01.fs")>]
    let ``E_InlineVirtualMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151

    [<Theory; FileInlineData("E_InlineVirtualMember02.fs")>]
    let ``E_InlineVirtualMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151

    // Success tests

    [<Theory; FileInlineData("DerivedClassSameAssembly.fs")>]
    let ``DerivedClassSameAssembly_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember01.fs")>]
    let ``CallToVirtualMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember02.fs")>]
    let ``CallToVirtualMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember03.fs")>]
    let ``CallToVirtualMember03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // PRECMD tests migrated from fsharpqa/Source/Conformance/ObjectOrientedTypeDefinitions/AbstractMembers
    // Original: PRECMD="$FSC_PIPE -a BaseClassAndInterface.fs" SCFLAGS="-r BaseClassAndInterface.dll -d FSLIBRARY"
    // Regression test for TFS#834683 - DerivedClass with F# base library
    [<Fact>]
    let ``DerivedClass with F# base library`` () =
        let fsLib =
            FSharp """
module FSLibrary

// Interface with a member
type I = abstract M : unit -> unit

type C = 
    new() = {}
    abstract member M : unit -> unit
    default this.M() = System.Console.WriteLine("I am my method")
    interface I with
        member this.M() = 
            System.Console.WriteLine("I am going via the interface")
            this.M()
"""
            |> asLibrary
            |> withName "BaseClassAndInterface"

        FSharp """
open FSLibrary

type D = 
    inherit C
    new() = {}
    override this.M() = System.Console.WriteLine("I am method M in D")
                        base.M()

let d = D()
d.M()
let di = d :> I
di.M()
"""
        |> asExe
        |> withReferences [fsLib]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed

    // Original: PRECMD="$CSC_PIPE /t:library BaseClassAndInterface.cs" SCFLAGS="-r BaseClassAndInterface.dll -d CSLIBRARY"
    // Regression test for TFS#834683 - DerivedClass with C# base library
    [<Fact>]
    let ``DerivedClass with C# base library`` () =
        let csLib =
            CSharp """
namespace CSLibrary
{
    public interface I
    {
        void M();
    }

    public class C : I
    {
        public virtual void M() { System.Console.WriteLine("I am my method"); }

        void I.M()
        {
            System.Console.WriteLine("I am going via the interface");
            M();
        }
    }
}
"""
            |> withName "BaseClassAndInterface"

        FSharp """
open CSLibrary

type D = 
    inherit C
    new() = {}
    override this.M() = System.Console.WriteLine("I am method M in D")
                        base.M()

let d = D()
d.M()
let di = d :> I
di.M()
"""
        |> asExe
        |> withReferences [csLib]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed

    // Original: PRECMD="$CSC_PIPE /t:library BaseClass.cs" SCFLAGS="-r:BaseClass.dll"
    // Regression test for FSHARP1.0:5815 - calling unimplemented base methods
    [<Fact>]
    let ``E_CallToUnimplementedMethod02 with C# abstract base class`` () =
        let csLib =
            CSharp """
/// A C# class with virtual and abstract methods
/// Consumed by E_CallToUnimplementedMethod02.fs
public abstract class B
{
    public virtual double M(int i)
    {
        return 1.0;
    }

    public abstract void M(string i);
}
"""
            |> withName "BaseClass"

        FSharp """
module Test
// Regression test for FSHARP1.0:5815
// It's illegal to call unimplemented base methods

type C() = 
    inherit B()
    override x.M(a:int) = base.M(a)
    override x.M(a:string) = base.M(a)
"""
        |> asLibrary
        |> withReferences [csLib]
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1201
        |> withDiagnosticMessageMatches "Cannot call an abstract base member: 'M'"

    // Original: PRECMD="$CSC_PIPE /t:library AbstractTypeLib.cs" SCFLAGS="-r:AbstractTypeLib.dll"
    // Dev10:921995/Dev11:15622 - error when instantiating abstract class from C#
    [<Fact>]
    let ``E_CreateAbstractTypeFromCS01 - cannot instantiate abstract C# class`` () =
        let csLib =
            CSharp """
using System;

namespace TestLib
{
    public abstract class A : IComparable<A>
    {
        public A() { }
        public abstract int CompareTo(A other);
    }

    public abstract class B<T> : IComparable<B<T>>
    {
        public B() { }
        public abstract int CompareTo(B<T> other);
    }
}
"""
            |> withName "AbstractTypeLib"

        FSharp """
module Test
let a = { new TestLib.A() with
          member this.CompareTo x = 0 }

let b = { new TestLib.B<string>() with
          member this.CompareTo x = 0 }

let x1 = new TestLib.A()
let x2 = TestLib.A()
let x3 = TestLib.B<int>()
let x4 = new TestLib.B<string>()
"""
        |> asExe
        |> withReferences [csLib]
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 759
