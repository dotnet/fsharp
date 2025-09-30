// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ErrorOnInvalidDeclsInTypeDefinitions =
    module ``Module inside interface`` =
        [<Fact>]
        let ``Error when module is inside interface``() =
            Fsx """
module TestModule

type IFace =
    abstract F : int -> int
    module M =
        let f () = ()
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``Version10: Error when module is inside interface verbose syntax``() =
            Fsx """
module TestModule

type IFace =
    interface
        abstract F : int -> int
        module M =
            let f () = f ()
    end
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 546, Line 5, Col 5, Line 5, Col 14, "Unmatched 'class', 'interface' or 'struct'");
                (Error 10, Line 7, Col 9, Line 7, Col 15, "Unexpected keyword 'module' in member definition");
                (Error 10, Line 9, Col 5, Line 9, Col 8, "Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.")
            ]
            
        [<Fact>]
        let ``Error when module is inside interface verbose syntax``() =
            Fsx """
module TestModule

type IFace =
    interface
        abstract F : int -> int
        module M =
            let f () = f ()
    end
            """
            |> withLangVersion90
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 546, Line 5, Col 5, Line 5, Col 14, "Unmatched 'class', 'interface' or 'struct'");
                (Error 10, Line 7, Col 9, Line 7, Col 15, "Unexpected keyword 'module' in member definition");
                (Error 10, Line 9, Col 5, Line 9, Col 8, "Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.")
            ]
            
        [<Fact>]
        let ``No Error when module is inside interface``() =
            Fsx """
module TestModule

type IFace =
    abstract F : int -> int
    module M =
        let f () = ()
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module inside class`` =
        [<Fact>]
        let ``Error when module is inside class``() =
            Fsx """
module TestModule

type C () =
    member _.F () = 3
    module M2 =
        let f () = ()
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error when module is inside class``() =
            Fsx """
module TestModule

type C () =
    member _.F () = 3
    module M2 =
        let f () = ()
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
        

    module ``Module inside class with constructor`` =
        [<Fact>]
        let ``Error when module is inside class with constructor``() =
            Fsx """
module TestModule

type MyClass(x: int) =
    let mutable value = x
    member _.Value = value
    module InternalModule =
        let helper() = 42
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]

        [<Fact>]
        let ``No Error when module is inside class with constructor``() =
            Fsx """
module TestModule

type MyClass(x: int) =
    let mutable value = x
    member _.Value = value
    module InternalModule =
        let helper() = 42
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module inside union`` =
        [<Fact>]
        let ``Error when module is inside discriminated union``() =
            Fsx """
module TestModule

type U =
    | A
    | B
    module M3 =
        let f () = ()
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]

        [<Fact>]
        let ``No Error when module is inside discriminated union``() =
            Fsx """
module TestModule

type U =
    | A
    | B
    module M3 =
        let f () = ()
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module inside record`` =
        [<Fact>]
        let ``Error when module is inside record``() =
            Fsx """
module TestModule

type R =
    { A : int }
    module M4 =
        let f () = ()
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error when module is inside record``() =
            Fsx """
module TestModule

type R =
    { A : int }
    module M4 =
        let f () = ()
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module inside struct`` =
        [<Fact>]
        let ``Error when module is inside struct``() =
            Fsx """
module TestModule

[<Struct>]
type MyStruct =
    val X: int
    val Y: int
    module InvalidModule = 
        let helper = 10
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 8, Col 5, Line 8, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error when module is inside struct``() =
            Fsx """
module TestModule

[<Struct>]
type MyStruct =
    val X: int
    val Y: int
    module InvalidModule = 
        let helper = 10
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module after delegate`` =
        [<Fact>]
        let ``Error when module appears after delegate``() =
            Fsx """
module TestModule

type MyDelegate = delegate of int * int -> int
    module InvalidModule =
        let x = 1
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 5, Col 5, Line 5, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error when module appears after delegate``() =
            Fsx """
module TestModule

type MyDelegate = delegate of int * int -> int
    module InvalidModule =
        let x = 1
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module after type members`` =
        [<Fact>]
        let ``Error when module appears after type members``() =
            Fsx """
module TestModule

type ClassWithMembers() =
    member _.Method1() = 1
    member _.Method2() = 2
    member _.Property = 3
    module LateModule =
        let x = 4
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 8, Col 5, Line 8, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]

        [<Fact>]
        let ``No Error when module appears after type members``() =
            Fsx """
module TestModule

type ClassWithMembers() =
    member _.Method1() = 1
    member _.Method2() = 2
    member _.Property = 3
    module LateModule =
        let x = 4
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Module after static members`` =
        [<Fact>]
        let ``Error when module appears after static members``() =
            Fsx """
module TestModule

type ClassWithStatic() =
    static member StaticMethod() = 1
    static member StaticProperty = 2
    module InvalidModule =
        let x = 3
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error when module appears after static members``() =
            Fsx """
module TestModule

type ClassWithStatic() =
    static member StaticMethod() = 1
    static member StaticProperty = 2
    module InvalidModule =
        let x = 3
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    module ``Exception inside type`` =
        [<Fact>]
        let ``Error when exception is inside type``() =
            Fsx """
module TestModule

type A = 
    | A
    exception MyException of string
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 14, "Exceptions must be defined at module level, not inside types.");
                (Error 10, Line 6, Col 5, Line 6, Col 14, "Unexpected keyword 'exception' in member definition")
            ]
            
        [<Fact>]
        let ``No Error when exception is inside type``() =
            Fsx """
module TestModule

type A = 
    | A
    exception MyException of string
            """
            |> withLangVersion90
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 10, Line 6, Col 5, Line 6, Col 14, "Unexpected keyword 'exception' in member definition")
            ]
    
    module ``Open inside type`` =
        [<Fact>]
        let ``Error when open declaration is inside type``() =
            Fsx """
module TestModule

type A = 
    | A
    open System
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 9, "'open' declarations must appear at module level, not inside types.")
                (Error 10, Line 6, Col 5, Line 6, Col 9, "Unexpected keyword 'open' in member definition")
            ]
            
        [<Fact>]
        let ``No Error when open declaration is inside type``() =
            Fsx """
module TestModule

type A = 
    | A
    open System
            """
            |> withLangVersion90
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 10, Line 6, Col 5, Line 6, Col 9, "Unexpected keyword 'open' in member definition")
            ]
    
    module ``Type inside type`` =
        [<Fact>]
        let ``Error when type is nested inside another type``() =
            Fsx """
module TestModule

type OuterType =
    | Case1
    | Case2
    type InnerType = int
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 9, "Nested type definitions are not allowed. Types must be defined at module or namespace level.")
            ]
    
        [<Fact>]
        let ``No Error when type is nested inside another type``() =
            Fsx """
module TestModule

type OuterType =
    | Case1
    | Case2
    type InnerType = int
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Multiple invalid constructs`` =
        [<Fact>]
        let ``Error for all invalid nested constructs in single type``() =
            Fsx """
module TestModule

type MultiTest =
    | Case1
    | Case2
    module NestedModule = begin end
    type NestedType = int
    exception NestedExc of string
    open System.Collections
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 8, Col 5, Line 8, Col 9, "Nested type definitions are not allowed. Types must be defined at module or namespace level.")
                (Error 58, Line 9, Col 5, Line 9, Col 14, "Exceptions must be defined at module level, not inside types.")
                (Error 58, Line 10, Col 5, Line 10, Col 9, "'open' declarations must appear at module level, not inside types.")
            ]
            
        [<Fact>]
        let ``No Error for all invalid nested constructs in single type``() =
            Fsx """
module TestModule

type MultiTest =
    | Case1
    | Case2
    module NestedModule = begin end
    type NestedType = int
    exception NestedExc of string
    open System.Collections
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

    module ``Multiple modules in single type`` =
        [<Fact>]
        let ``Error for each module inside type``() =
            Fsx """
module TestModule

type B =
    | B
    module M1 = begin end
    module M2 = begin end
    module M3 = begin end
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 8, Col 5, Line 8, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error for each module inside type``() =
            Fsx """
module TestModule

type B =
    | B
    module M1 = begin end
    module M2 = begin end
    module M3 = begin end
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Deeply nested invalid constructs`` =
        [<Fact>]
        let ``Error for invalid constructs in deeply nested context``() =
            Fsx """
module OuterModule

module InnerModule =
    module DeeplyNested =
        type IndentedType =
            | Case1
            | Case2
            type NestedType = int
            module NestedModule =
                let x = 1
            exception NestedExc of string
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 9, Col 13, Line 9, Col 17, "Nested type definitions are not allowed. Types must be defined at module or namespace level.")
                (Error 58, Line 10, Col 13, Line 10, Col 19, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 12, Col 13, Line 12, Col 22, "Exceptions must be defined at module level, not inside types.")
            ]
            
        [<Fact>]
        let ``No Error for invalid constructs in deeply nested context``() =
            Fsx """
module OuterModule

module InnerModule =
    module DeeplyNested =
        type IndentedType =
            | Case1
            | Case2
            type NestedType = int
            module NestedModule =
                let x = 1
            exception NestedExc of string
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Abstract class with invalid constructs`` =
        [<Fact>]
        let ``Error for invalid constructs in abstract class``() =
            Fsx """
module TestModule

[<AbstractClass>]
type AbstractBase() =
    abstract member Method : unit -> int
    module InvalidModule = begin end
    type InvalidType = string
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 8, Col 5, Line 8, Col 9, "Nested type definitions are not allowed. Types must be defined at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error for invalid constructs in abstract class``() =
            Fsx """
module TestModule

[<AbstractClass>]
type AbstractBase() =
    abstract member Method : unit -> int
    module InvalidModule = begin end
    type InvalidType = string
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Type augmentation with invalid constructs`` =
        [<Fact>]
        let ``Error for module in type augmentation``() =
            Fsx """
module TestModule

type Original = | A | B

type Original with
    member _.Extended() = 1
    module ExtensionModule =
        let x = 2
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 8, Col 5, Line 8, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 10, Line 8, Col 5, Line 8, Col 11, "Unexpected keyword 'module' in type definition. Expected incomplete structured construct at or before this point, 'end' or other token.")
                (Error 10, Line 10, Col 1, Line 10, Col 13, "Incomplete structured construct at or before this point in implementation file")
            ]
            
        [<Fact>]
        let ``No Error for module in type augmentation``() =
            Fsx """
module TestModule

type Original = | A | B

type Original with
    member _.Extended() = 1
    module ExtensionModule =
        let x = 2
            """
            |> withLangVersion90
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 10, Line 8, Col 5, Line 8, Col 11, "Unexpected keyword 'module' in type definition. Expected incomplete structured construct at or before this point, 'end' or other token.");
                (Error 10, Line 10, Col 1, Line 10, Col 13, "Incomplete structured construct at or before this point in implementation file")
            ]
    
    module ``Do binding with invalid constructs`` =
        [<Fact>]
        let ``Error for invalid constructs after do binding``() =
            Fsx """
module TestModule

type TypeWithDo() =
    do printfn "Initialized"
    type NestedType = int
    module NestedModule = begin end
    open System
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 6, Col 5, Line 6, Col 9, "Nested type definitions are not allowed. Types must be defined at module or namespace level.")
                (Error 58, Line 7, Col 5, Line 7, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
                (Error 58, Line 8, Col 5, Line 8, Col 9, "'open' declarations must appear at module level, not inside types.")
            ]
    
    
        [<Fact>]
        let ``No Error for invalid constructs after do binding``() =
            Fsx """
module TestModule

type TypeWithDo() =
    do printfn "Initialized"
    type NestedType = int
    module NestedModule = begin end
    open System
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Inherit with invalid constructs`` =
        [<Fact>]
        let ``Error for invalid constructs after inherit``() =
            Fsx """
module TestModule

type Base() = class end

type Derived() =
    inherit Base()
    module InvalidModule = begin end
            """
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 58, Line 8, Col 5, Line 8, Col 11, "Modules cannot be nested inside types. Define modules at module or namespace level.")
            ]
            
        [<Fact>]
        let ``No Error for invalid constructs after inherit``() =
            Fsx """
module TestModule

type Base() = class end

type Derived() =
    inherit Base()
    module InvalidModule = begin end
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
    
    module ``Valid module placement`` =
        [<Fact>]
        let ``No Error for modules at correct level``() =
            Fsx """
module TestModule

type A = A

module ValidModule1 = begin end
module ValidModule2 = begin end

type B = B

module ValidModule3 =
    let f () = ()
            """
            |> withLangVersion10
            |> typecheck
            |> shouldSucceed
            
        [<Fact>]
        let ``No Error for modules at correct level 2``() =
            Fsx """
module TestModule

type A = A

module ValidModule1 = begin end
module ValidModule2 = begin end

type B = B

module ValidModule3 =
    let f () = ()
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``No Error for let bindings inside class``() =
            Fsx """
module TestModule

type ClassWithLet() =
    let helper x = x + 1
    let mutable state = 0
    member _.Method() = helper state
            """
            |> withLangVersion10
            |> typecheck
            |> shouldSucceed
            
        [<Fact>]
        let ``No Error for let bindings inside class 2``() =
            Fsx """
module TestModule

type ClassWithLet() =
    let helper x = x + 1
    let mutable state = 0
    member _.Method() = helper state
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``No Error for modules at same indentation as type``() =
            Fsx """
module TestModule

type A = A
module B = begin end  // Same column as type, not nested
            """
            |> withLangVersion10
            |> typecheck
            |> shouldSucceed
            
        [<Fact>]
        let ``No Error for modules at same indentation as type 2``() =
            Fsx """
module TestModule

type A = A
module B = begin end  // Same column as type, not nested
            """
            |> withLangVersion90
            |> typecheck
            |> shouldSucceed
