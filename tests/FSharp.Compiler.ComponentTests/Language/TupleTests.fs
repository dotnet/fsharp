// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

module TupleTests =

    [<Fact>]
    let ``Simple reference tuple`` () =
        FSharp """
let x, y = 1, 2
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed

    [<Fact>]
    let ``Simple reference tuple destructuring with match`` () =
        FSharp """
match 1, 2 with
| x, y -> x + y
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
     
    [<Fact>]
    let ``Simple reference tuple destructuring with for-loop`` () =
        FSharp """
for x, y in [1, 2] do ()
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed     
    
//     [<Fact>]
//     let ``Simple reference tuple destructuring in a lambda`` () =
//         FSharp """
// type Class() =
//     member _.Method(x:(int*int)[]) =
//         x |> Array.iter (fun (a, b) -> ())
//     """
//      |> ignoreWarnings
//      |> typecheck
//      |> shouldSucceed
     
    [<Fact>]
    let ``Function - Simple tuple destructuring in a lambda`` () =
        FSharp """
let f (a: (int * int) list) =
    a |> List.map fst
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
     
module StructTupleTests =

    [<Fact>]
    let ``Simple struct tuple`` () =
        FSharp """
let struct(x, y) = struct(1, 2)
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed

    [<Fact>]
    let ``Simple struct tuple destructuring with match`` () =
        FSharp """
match struct(1, 2) with
| x, y -> x + y

match struct(1, 2) with
| struct(x, y) -> x + y
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed

    [<Fact>]
    let ``Simple struct tuple destructuring with for-loop`` () =
        FSharp """
let structTuples = [ struct(1, 2) ]
for x, y in structTuples do ()
for struct(x, y) in structTuples do ()
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
     
    [<Fact>]
    let ``Class - Simple struct tuple destructuring in a lambda`` () =
        FSharp """
type Class() =
    member _.Method(x:struct(int*int)[]) =
        x |> Array.iter (fun (a, b) -> ())
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
     
    [<Fact>]
    let ``Class - Simple struct tuple destructuring in a lambda AppExpr`` () =
        FSharp """
type Class() =
    member _.Method(x:struct(int*int)[]) =
        x |> Array.iter (fun struct (a, b) -> ())
    
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed     
    
    [<Fact>]
    let ``Class - Simple struct tuple destructuring in a match lambda AppExpr`` () =
        FSharp """
type Class() =
    member _.Method(x:struct(int*int)[]) =
        x |> Array.iter (function (a, b) -> ())
        x |> Array.iter (function struct (a, b) -> ())
    
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed     

    [<Fact>]
    let ``Class - Simple struct tuple destructuring in a struct lambda argument`` () =
        FSharp """
type Class() =
    member _.Method2(x:struct(int*int)) =
        x |> fun (a, b) -> ()
        x |> fun struct(a, b) -> ()
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
    
    [<Fact>]
    let ``Class - calling a method with struct tuple parameter`` () =
        FSharp """
type Class() =
    member _.Method2(x:struct(int*int)) =
        x |> fun struct(a, b) -> ()
        
let c = Class()
c.Method2((1, 2))
c.Method2(struct(1, 2))

struct(1, 2)
|> c.Method2
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
     
    [<Fact>]
    let ``Class - Error when calling a method with struct tuple parameter(Piping)`` () =
        FSharp """
type Class() =
    member _.Method2(x:struct(int*int)) =
        x |> fun struct(a, b) -> ()
        
let c = Class()
(1, 2)
|> c.Method2
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldFail
     |> withDiagnostics [
        (Error 193, Line 8, Col 4, Line 8, Col 13, "Type constraint mismatch. The type 
    'int * int'    
is not compatible with type
    'struct (int * int)'    
")
     ]

    [<Fact>]
    let ``Function - Simple struct tuple destructuring in a struct lambda with parenthesis`` () =
        FSharp """
let f (a: struct (int * int)) =
    a |> (fun ((x, y)) -> x + y)
    a |> (fun (struct(x, y)) -> x + y)
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
    
//     [<Fact>]
//     let ``Function - Simple struct tuple destructuring in a lambda`` () =
//         FSharp """
// let f (a: struct (int * int) list) =
//     a |> List.map fst
//     """
//      |> ignoreWarnings
//      |> typecheck
//      |> shouldSucceed
     
    [<Fact>]
    let ``Function - Simple struct tuple destructuring in a match lambda AppExpr`` () =
        FSharp """
let f (x: struct (int * int) []) =
    x |> Array.iter (function (a, b) -> ())
    x |> Array.iter (function struct (a, b) -> ())
    
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed

    [<Fact>]
    let ``Function - Simple struct tuple destructuring in a struct lambda`` () =
        FSharp """
let f (a: struct (int * int)) =
    a |> (fun (x, y) -> x + y)
    a |> (fun struct(x, y) -> x + y)
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed
    
    [<Fact>]
    let ``Function - struct tuple destructuring in a CE`` () =
        FSharp """
let doSomething() =
    async {
        return struct (1, 2)
    }
    
let consumeSomething() =
    task {
        let! x, y = doSomething()
        let! struct (a, b) = doSomething()
        return x + y + a + b
    }
    """
     |> ignoreWarnings
     |> typecheck
     |> shouldSucceed