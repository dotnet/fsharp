namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module StaticClassTests =

    [<Fact>]
    let ``Sealed and AbstractClass on a type in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T = class end
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Sealed and AbstractClass on a type in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T = class end
        """
         |> withLangVersionPreview
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Sealed and AbstractClass on a type with constructor in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T() = class end
        """
         |> withLangVersionPreview
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Sealed and AbstractClass on a type with constructor in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T() = class end
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``Sealed and AbstractClass on a type with constructor with arguments in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T(x: int) = class end
        """
         |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
                (Error 3552, Line 3, Col 8, Line 3, Col 14, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No constructor arguments are allowed")
         ]

    [<Fact>]
    let ``Sealed and AbstractClass on a type with constructor with arguments in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T(x: int) = class end
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``When Sealed and AbstractClass on a type with additional constructors in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T =
    new () = {}
        """
         |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
             (Error 3552, Line 4, Col 5, Line 4, Col 16, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No additional constructors are allowed")
         ]

    [<Fact>]
    let ``When Sealed and AbstractClass on a type with additional constructors in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T =
    new () = {}
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``When Sealed and AbstractClass on a type with a primary(parameters) and additional constructor in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type T(x: int) =
    new () = T(42)
        """
         |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
             (Error 3552, Line 3, Col 8, Line 3, Col 14, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No constructor arguments are allowed")
             (Error 3552, Line 4, Col 5, Line 4, Col 19, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No additional constructors are allowed")
         ]
         
    [<Fact>]
    let ``When Sealed and AbstractClass on a type with explicit fields and constructor in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type B =
    val F : int
    val mutable G : int
    new () = { F = 3; G = 3 }
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed
    [<Fact>]
    let ``When Sealed and AbstractClass on a generic type with constructor in lang version70`` () =
        Fsx """
[<Sealed; AbstractClass>]
type ListDebugView<'T>(l: 'T list) = class end
        """
         |> withLangVersion70
         |> compile
         |> shouldSucceed
         
    [<Fact>]
    let ``When Sealed and AbstractClass on a generic type with constructor in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type ListDebugView<'T>(l: 'T list) = class end
        """
         |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
             (Error 3552, Line 3, Col 24, Line 3, Col 34, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No constructor arguments are allowed")
         ]

    [<Fact>]
    let ``When Sealed and AbstractClass on a type with explicit fields and constructor in lang preview`` () =
        Fsx """
[<Sealed; AbstractClass>]
type B =
    val F : int
    val mutable G : int
    new () = { F = 3; G = 3 }
        """
         |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
             (Error 3552, Line 6, Col 5, Line 6, Col 30, "if a type uses both [<Sealed>] and [<AbstractClass>] it means it is static. No additional constructors are allowed")
         ]

    [<Theory>]
    [<InlineData("preview")>]
    [<InlineData("7.0")>]
    let ``Mutually recursive type definition that using custom attributes``(langVersion) =
        let code = """
        module Test

        open System.Diagnostics

        [<DefaultAugmentation(false)>]
        [<DebuggerTypeProxyAttribute(typedefof<MyCustomListDebugView<_>>)>]
        [<DebuggerDisplay("{DebugDisplay,nq}")>]
        [<CompiledName("MyCustomList")>]
        type MyCustomList<'T> = 
            | Empty
            | NonEmpty of Head: 'T * Tail: MyCustomList<'T>
        
        and MyImbaAlias<'T> = MyCustomList<'T>

        //-------------------------------------------------------------------------
        // List (debug view)
        //-------------------------------------------------------------------------

        and
            MyCustomListDebugView<'T>(l: MyCustomList<'T>) =
                let asList =
                    let rec toList ml = 
                        match ml with
                        | Empty -> []
                        | NonEmpty (head,tail) -> head :: (toList tail)
                    toList l

                [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
                member x.Items = asList |> List.toArray

                [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
                member x._FullList = asList |> List.toArray

        """
        Fs code
        |> withLangVersion langVersion
        |> compile
        |> shouldSucceed