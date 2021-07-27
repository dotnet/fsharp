namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler

module ``Verify visibility of properties`` =

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
}"""

    let fsharpBaseClass = """
namespace ExhaustiveCombinations

open System

type FSharpBaseClass () =
    member this.GetPublicSetInternal    with public   get() = "" and internal set (parameter:string) = ignore parameter
    member this.GetPublicSetPrivate     with public   get() = "" and private  set (parameter:string) = ignore parameter
    member this.SetPublicGetInternal    with internal get() = "" and public   set (parameter:string) = ignore parameter
    member this.SetPublicGetPrivate     with private  get() = "" and public   set (parameter:string) = ignore parameter"""

    [<Fact>]
    let ``C# class F# derived class - access public`` () =

        let csharpLib = CSharp csharpBaseClass |> withName "csLib"

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
        Fsx fsharpSource
        |> withLangVersion50
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 491, Line 19, Col 9, Line 19, Col 41,
             "The member or object constructor 'GetPublicSetInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 22, Col 9, Line 22, Col 49,
             "The member or object constructor 'GetPublicSetPrivateProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 31, Col 9, Line 31, Col 40,
             "The member or object constructor 'GetPublicSetPrivate' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")]

    [<Fact>]
    let ``C# class F# non-derived class - access public`` () =

        let csharpLib = CSharp csharpBaseClass |> withName "csLib"

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

        Fsx fsharpSource
        |> withLangVersion50
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 491, Line 19, Col 9, Line 19, Col 39,
             "The member or object constructor 'GetPublicSetInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 22, Col 9, Line 22, Col 47,
             "The member or object constructor 'GetPublicSetPrivateProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 25, Col 9, Line 25, Col 48,
             "The member or object constructor 'GetPublicSetProtectedInternal' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 28, Col 9, Line 28, Col 40,
             "The member or object constructor 'GetPublicSetProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 29, Col 17, Line 29, Col 41,
             "The member or object constructor 'SetPublicGetProtected' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.");
            (Error 491, Line 31, Col 9, Line 31, Col 38,
             "The member or object constructor 'GetPublicSetPrivate' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")]

    [<Fact>]
    let ``F# base F# derived class - access public`` () =

        let csharpLib = CSharp csharpBaseClass |> withName "csLib"

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

        ()"""
        Fsx fsharpSource
        |> withLangVersion50
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 810, Line 21, Col 9, Line 21, Col 33, "Property 'GetPublicSetPrivate' cannot be set")
            (Error 807, Line 28, Col 17, Line 28, Col 41, "Property 'SetPublicGetPrivate' is not readable")]

    [<Fact>]
    let ``F# class F# non-derived class - access public`` () =

        let csharpLib = CSharp csharpBaseClass |> withName "csLib"

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
        ()"""

        Fsx fsharpSource
        |> withLangVersion50
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 810, Line 21, Col 9, Line 21, Col 31, "Property 'GetPublicSetPrivate' cannot be set")
