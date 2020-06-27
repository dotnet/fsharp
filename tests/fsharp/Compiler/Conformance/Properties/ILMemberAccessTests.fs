// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices
open FSharp.Reflection
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open NUnit.Framework

[<TestFixture>]
module ILMemberAccessTests =

    let csharpBaseClass = """
namespace ExhaustiveCombinations
{
    public class CSharpBaseClass
    {
        public string GetPublicSetInternal { get; internal set; }
        public string GetPublicSetProtected { get; protected set; }
        public string GetPublicSetPrivateProtected { get; private protected set; }
        public string GetPublicSetProtectedInternal { get; protected internal set; }
        public string GetPublicSetPrivate { get; private set; }

        public string SetPublicGetInternal { internal get; set; }
        public string SetPublicGetProtected { protected get; set; }
        public string SetPublicGetPrivateProtected { private protected get; set; }
        public string SetPublicGetProtectedInternal { protected internal get; set; }
        public string SetPublicGetPrivate { private get; set; }
    }
}
"""

    let fsharpBaseClass = """
namespace ExhaustiveCombinations

open System

type FSharpBaseClass () =

    member this.GetPublicSetInternal    with public   get() = "" and internal set (parameter:string) = ignore parameter
    member this.GetPublicSetPrivate     with public   get() = "" and private  set (parameter:string) = ignore parameter
    member this.SetPublicGetInternal    with internal get() = "" and public   set (parameter:string) = ignore parameter
    member this.SetPublicGetPrivate     with private  get() = "" and public   set (parameter:string) = ignore parameter

"""


    [<Test>]
    let ``VerifyVisibility of Properties C# class F# derived class -- AccessPublicStuff`` () =

        let fsharpSource =
            fsharpBaseClass + """
open System
open ExhaustiveCombinations

type MyFSharpClass () =
    inherit CSharpBaseClass()

    member this.AccessPublicStuff() =

        this.GetPublicSetInternal <- "1"            // Inaccessible
        let _ = this.GetPublicSetInternal           // Accessible

        this.GetPublicSetPrivateProtected <- "1"    // Accessible
        let _ = this.GetPublicSetPrivateProtected   // Accessible

        this.GetPublicSetProtectedInternal <- "1"   // Accessible
        let _ = this.GetPublicSetProtectedInternal  // Accessible

        this.GetPublicSetProtected <- "1"           // Accessible
        let _ = this.GetPublicSetProtected          // Accessible

        this.GetPublicSetPrivate <- "1"             // Inaccessible
        let _ = this.GetPublicSetPrivate            // Accessible
        ()
"""

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpBaseClass, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fsx, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 491, (22, 9, 22, 41),
             "The member or object constructor 'GetPublicSetInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (25, 9, 25, 49),
             "The member or object constructor 'GetPublicSetPrivateProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (34, 9, 34, 40),
             "The member or object constructor 'GetPublicSetPrivate' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")|])


    [<Test>]
    let ``VerifyVisibility of Properties C# class F# non-derived class -- AccessPublicStuff`` () =

        let fsharpSource =
            fsharpBaseClass + """
open System
open ExhaustiveCombinations

type MyFSharpClass () =

    member _.AccessPublicStuff() =
        let bc = new CSharpBaseClass()

        bc.GetPublicSetInternal <- "1"              // Inaccessible
        let _ = bc.GetPublicSetInternal             // Accessible

        bc.GetPublicSetPrivateProtected <- "1"      // Inaccessible
        let _ = bc.GetPublicSetPrivateProtected     // Accessible

        bc.GetPublicSetProtectedInternal <- "1"     // Accessible
        let _ = bc.GetPublicSetProtectedInternal    // Inaccessible

        bc.GetPublicSetProtected <- "1"             // Inaccessible
        let _ = bc.SetPublicGetProtected            // Accessible

        bc.GetPublicSetPrivate <- "1"               // Inaccessible
        let _ = bc.GetPublicSetPrivate              // Accessible
        ()
"""

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpBaseClass, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fsx, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 491, (22, 9, 22, 39),
             "The member or object constructor 'GetPublicSetInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (25, 9, 25, 47),
             "The member or object constructor 'GetPublicSetPrivateProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (28, 9, 28, 48),
             "The member or object constructor 'GetPublicSetProtectedInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (31, 9, 31, 40),
             "The member or object constructor 'GetPublicSetProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (32, 17, 32, 41),
             "The member or object constructor 'SetPublicGetProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (FSharpErrorSeverity.Error, 491, (34, 9, 34, 38),
             "The member or object constructor 'GetPublicSetPrivate' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")|])


    [<Test>]
    let ``VerifyVisibility of Properties F# base F# derived class -- AccessPublicStuff`` () =

        let fsharpSource =
            fsharpBaseClass + """
open System
open ExhaustiveCombinations

type MyFSharpClass () =
    inherit FSharpBaseClass()

    member this.AccessPublicStuff() =

        this.GetPublicSetInternal <- "1"            // Inaccessible
        let _ = this.GetPublicSetInternal           // Accessible

        this.GetPublicSetPrivate <- "1"             // Inaccessible
        let _ = this.GetPublicSetPrivate            // Accessible

        this.SetPublicGetInternal <- "1"            // Accessible
        let _ = this.SetPublicGetInternal           // Inaccessible

        this.SetPublicGetPrivate <- "1"             // Accessible
        let _ = this.SetPublicGetPrivate            // accessible

        ()
"""

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpBaseClass, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fsx, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 810, (25, 9, 25, 33),
             "Property 'GetPublicSetPrivate' cannot be set");
            (FSharpErrorSeverity.Error, 807, (32, 17, 32, 41),
             "Property 'SetPublicGetPrivate' is not readable")
        |])


    [<Test>]
    let ``VerifyVisibility of Properties F# class F# non-derived class -- AccessPublicStuff`` () =

        let fsharpSource =
            fsharpBaseClass + """
open System
open ExhaustiveCombinations

type MyFSharpClass () =

    member _.AccessPublicStuff() =
        let bc = new FSharpBaseClass()

        bc.GetPublicSetInternal <- "1"              // Inaccessible
        let _ = bc.GetPublicSetInternal             // Accessible

        bc.GetPublicSetPrivate <- "1"               // Inaccessible
        let _ = bc.GetPublicSetPrivate              // Accessible
        ()
"""

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpBaseClass, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fsx, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 810, (25, 9, 25, 31),
             "Property 'GetPublicSetPrivate' cannot be set")|])



