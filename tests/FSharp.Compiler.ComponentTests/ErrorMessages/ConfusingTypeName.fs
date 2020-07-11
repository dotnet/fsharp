// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Confusing Type Name`` =

    [<Fact>]
    let ``Checks expected types with multiple references``() =
        let csLibAB = """
public class A { }
public class B<T> { }
            """
        let csLibACmpl =
             CompilationUtil.CreateCSharpCompilation(csLibAB, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30, name = "libA")
             |> CompilationReference.Create

        let csLibBCmpl =
            CompilationUtil.CreateCSharpCompilation(csLibAB, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30, name = "libB")
             |> CompilationReference.Create

        let fsLibC = """
module AMaker
let makeA () : A = A()
let makeB () = B<_>()
        """

        let fsLibD = """
module OtherAMaker
let makeOtherA () : A = A()
let makeOtherB () = B<_>()
        """

        let fsLibCCmpl =
            Compilation.Create(fsLibC, Fs, Library, cmplRefs = [csLibACmpl], name = "libC")
            |> CompilationReference.CreateFSharp

        let fsLibDCmpl =
            Compilation.Create(fsLibD, Fs, Library, cmplRefs = [csLibBCmpl], name = "libD")
            |> CompilationReference.CreateFSharp
        
        let app = """
module ConfusingTypeName
let a = AMaker.makeA()
let otherA = OtherAMaker.makeOtherA()
printfn "%A %A" (a.GetType().AssemblyQualifiedName) (otherA.GetType().AssemblyQualifiedName)
printfn "%A" (a = otherA)

let b = AMaker.makeB<int>()
let otherB = OtherAMaker.makeOtherB<int>()
printfn "%A %A" (b.GetType().AssemblyQualifiedName) (otherB.GetType().AssemblyQualifiedName)
printfn "%A" (b = otherB)
        """

        let appCmpl =
            Compilation.Create(app, Fs, Library, cmplRefs = [csLibACmpl; csLibBCmpl; fsLibCCmpl; fsLibDCmpl])

        CompilerAssert.CompileWithErrors(
            appCmpl,
            [|
             (FSharpErrorSeverity.Error, 1, (6, 19, 6, 25), ("This expression was expected to have type\n    'A (libA, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    \nbut here has type\n    'A (libB, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    "))
             (FSharpErrorSeverity.Error, 1, (11, 19, 11, 25), ("This expression was expected to have type\n    'B<Microsoft.FSharp.Core.int> (libA, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    \nbut here has type\n    'B<Microsoft.FSharp.Core.int> (libB, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'    "))
             |], true)
