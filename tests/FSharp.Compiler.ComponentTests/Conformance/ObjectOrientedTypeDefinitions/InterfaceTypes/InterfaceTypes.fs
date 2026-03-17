// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module InterfaceTypes =

    // Error tests

    [<Theory; FileInlineData("E_InheritInterface.fs")>]
    let ``E_InheritInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1207

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl01.fs")>]
    let ``E_InterfaceNotFullyImpl01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 366

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl02.fs")>]
    let ``E_InterfaceNotFullyImpl02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 855

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl03.fs")>]
    let ``E_InterfaceNotFullyImpl03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 366

    [<Theory; FileInlineData("E_MultipleInterfaceInheritance.fs")>]
    let ``E_MultipleInterfaceInheritance_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_AnonymousTypeInInterface01.fs")>]
    let ``E_AnonymousTypeInInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 715

    [<Theory; FileInlineData("E_MultipleInst07.5.0.fs")>]
    let ``E_MultipleInst07_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3360

    [<Theory; FileInlineData("E_ImplementGenIFaceTwice01_5.0.fs")>]
    let ``E_ImplementGenIFaceTwice01_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3360

    // Success tests

    [<Theory; FileInlineData("ObjImplementsInterfaceGenWithConstraint.fs")>]
    let ``ObjImplementsInterfaceGenWithConstraint_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InterfaceMember_NameCollisions.fs")>]
    let ``InterfaceMember_NameCollisions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleInst01.5.0.fs")>]
    let ``MultipleInst01_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleInst02.fs")>]
    let ``MultipleInst02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 362

    [<Theory; FileInlineData("MultipleInst03.fs")>]
    let ``MultipleInst03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [362; 855; 888]

    [<Theory; FileInlineData("MultipleInst05.fs")>]
    let ``MultipleInst05_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 362

    [<Theory; FileInlineData("MultipleInst06.fs")>]
    let ``MultipleInst06_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [362; 855; 888]

    [<Theory; FileInlineData("MultipleInst04.5.0.fs")>]
    let ``MultipleInst04_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Inheritance_OverrideInterface.fs")>]
    let ``Inheritance_OverrideInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritFromIComparable01.fs")>]
    let ``InheritFromIComparable01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritedInterface.fs")>]
    let ``InheritedInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ObjImplementsInterface.fs")>]
    let ``ObjImplementsInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface001.fs")>]
    let ``interface001_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface002.fs")>]
    let ``interface002_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface001e.fs")>]
    let ``interface001e_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 887

    [<Theory; FileInlineData("interface002e.fs")>]
    let ``interface002e_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("interface003.fs")>]
    let ``interface003_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ImplementGenIFaceTwice02_5.0.fs")>]
    let ``ImplementGenIFaceTwice02_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--nowarn:221"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("EmptyInterface01.fs")>]
    let ``EmptyInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritDotNetInterface.fs")>]
    let ``InheritDotNetInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // PRECMD tests migrated from fsharpqa/Source/Conformance/ObjectOrientedTypeDefinitions/InterfaceTypes
    // Original: PRECMD="$CSC_PIPE /t:library TwoInstantiationOfTheSameInterfaceDLL.cs" SCFLAGS=-r:TwoInstantiationOfTheSameInterfaceDLL.dll
    // Regression test for FSHARP1.0:5465 - generic interface at multiple instantiations
    [<Fact>]
    let ``TwoInstantiationOfTheSameInterface - C# class implementing generic interface twice`` () =
        let csLib =
            CSharp """
// Regression test for FSHARP1.0:5465

// A generic interface
interface I<T>
{
    void K(T x);    // Some generic method
}

// A class implementing 2 instantiation of the generic interface I<T>
public class C : I<int>, I<string>
{
    public void K(int x) { }
    public void K(string x) { }
}
"""
            |> withName "TwoInstantiationOfTheSameInterfaceDLL"

        FSharp """
module Test
// Regression test for FSHARP1.0:5465
// Title: problem unifying types which implement generic interfaces at multiple instantiations

type D =
    member __.M0(a:string, n:C)= ()
    member __.M1(a:string) (n:C)= ()

let M (d:D, c:C) = d.M0("aa",c)
let N (d:D, c:C) = d.M1 "aa" c  // Used to give error: Type constraint mismatch
"""
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed

    // Original: PRECMD="$CSC_PIPE /t:library ConsumeFromFS.cs" SCFLAGS="-r:ConsumeFromFS.dll"
    // Verify consumption of C# interfaces from F#
    [<Fact>]
    let ``CallCSharpInterface - consume C# interfaces from F#`` () =
        let csLib =
            CSharp """
// Verify consumption from F#

//Interface - empty
public interface I_000<T> { }

//Interface with inherits-decl 
public interface I_001<T> : I_000<T> { }

//Interface with type-defn-members 
public interface I_002<T>
{
    T Me();
}

//Interface with inherits-decl & type-defn-members 
public interface I_003<T> : I_001<T>
{
    T Home(T t);
}
"""
            |> withName "ConsumeFromFS"

        FSharp """
// Verify consumption of C# interfaces from F#

let result = {new I_003<int> with member x.Home(i) = i }.Home({new I_002<int> with member x.Me() = 0}.Me())
if result = 0 then 0 else 1
"""
        |> asExe
        |> withReferences [csLib]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed

    // Original: PRECMD="$CSC_PIPE /t:library MultipleInterfaceInheritanceFromCS.cs" SCFLAGS="-r:MultipleInterfaceInheritanceFromCS.dll"
    // Consume C# type implementing multiple instantiations of an interface
    [<Fact>]
    let ``ConsumeMultipleInterfaceFromCS - C# type with multiple interface implementations`` () =
        let csLib =
            CSharp """
//Interface - empty
public interface I_000<T> { }


//Interface with inherits-decl 
public interface I_001 : I_000<char>, I_000<string> { }


//Interface with type-defn-members 
public interface I_002<T>
{
    int Me(T t);
}

//Interface with inherits-decl & type-defn-members 
public interface I_003<T> : I_002<string>, I_002<char>
{
    T Home(T t);
}


public class T : I_003<int>
{
    static void Main()
    { }

    public int Home(int i) { return 0; }
    public int Me(char c) { return 1; }
    public int Me(string s) { return 2; }
}
"""
            |> withName "MultipleInterfaceInheritanceFromCS"

        FSharp """
let mutable res = true
let t = new T()
if (t.Me("F#") <> 2) then
  System.Console.WriteLine("t.Me(string) failed")
  res <- false
  
if (t.Me('a') <> 1) then
  System.Console.WriteLine("t.Me(char) failed")
  res <- false

if (t.Home(0) <> 0) then
  System.Console.WriteLine("t.Home failed")
  res <- false

// Check we can use an object expression inheriting from a C# type implementing multiple instantiations of an interface
if (({new T() with 
               member x.ToString() = "a"
      } :> I_003<int>).Home (4)  <> 0 ) then
            System.Console.WriteLine("T.Home obj expr failed")         
            res <- false
  
// Check we can create an object of a C# type implementing multiple instantiations of an interface
if T().Home(4) <> 0 then
            System.Console.WriteLine("T.Home failed")         
            res <- false


// Check we can inherit from a C# type implementing multiple instantiations of an interface
type D() = 
    inherit T()

if (D() :> I_003<int>).Home(5) <> 0 then
            System.Console.WriteLine("D.Home failed")         
            res <- false


if res = true then 0 else 1
"""
        |> asExe
        |> withReferences [csLib]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed

    // Original: PRECMD="$CSC_PIPE /t:library MultipleInterfaceInheritanceFromCS.cs" SCFLAGS="-r:MultipleInterfaceInheritanceFromCS.dll"
    // F# class inheriting C# type with multiple interface implementations and overriding interface
    [<Fact>]
    let ``ClassConsumeMultipleInterfaceFromCS - F# class inherits C# type and overrides interface`` () =
        let csLib =
            CSharp """
//Interface - empty
public interface I_000<T> { }

//Interface with inherits-decl 
public interface I_001 : I_000<char>, I_000<string> { }

//Interface with type-defn-members 
public interface I_002<T>
{
    int Me(T t);
}

//Interface with inherits-decl & type-defn-members 
public interface I_003<T> : I_002<string>, I_002<char>
{
    T Home(T t);
}

public class T : I_003<int>
{
    static void Main()
    { }

    public int Home(int i) { return 0; }
    public int Me(char c) { return 1; }
    public int Me(string s) { return 2; }
}
"""
            |> withName "MultipleInterfaceInheritanceFromCS2"

        FSharp """
let mutable res = true

type D() = 
    inherit T()
    interface I_003<int> with
       member xxx.Home(i) = i

if (D() :> I_003<int>).Home(5) <> 5 then
            System.Console.WriteLine("D.Home failed")
            res <- false

if res = true then 0 else 1
"""
        |> asExe
        |> withReferences [csLib]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
