module FSharp.Compiler.Service.Tests2.TestASTVisit

open NUnit.Framework
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests2.ASTVisit


[<Test>]
let ``Top level stuff extraction2`` () =
    let parseResults = 
        getParseResults
            """
namespace A
let x = 3

namespace B
type X = int * int

namespace C
module A =
    let x = 3

namespace D
[<AutoOpen>]
module D1 =
    module D2 =
        let x = 3
    
namespace D
module D1 =
    module D2 =
        let x = 3
"""
    let top = topModuleOrNamespaces parseResults
    printfn $"%+A{top}"

[<Test>]
let ``Top level stuff extraction`` () =
    let parseResults = 
        getParseResults
            """

module A1 = let a = 3
module A2 = let a = 3
module A3 = let a = 3
module A4 =
    
    type AAttribute(name : string) =
        inherit System.Attribute()
    
    let a = 3
    module A1 =
        let a = 3
    type X = int * int
    type Y = Y of int

module B =
    open A2
    let b = [|
        A1.a
        A2.a
        A3.a
    |]
    let c : A4.X = 2,2
    [<A4.A("name")>]
    let d : A4.Y = A4.Y 2
    type Z =
        {
            X : A4.X
            Y : A4.Y
        }

let c = A4.a
let d = A4.A1.a
open A4
let e = A1.a
open A1
let f = a
"""

    let stuff = extractModuleRefs parseResults
    let top = topModuleOrNamespaces parseResults
    printfn $"%+A{top}"
    printfn $"%+A{stuff}"
    ()

[<Test>]
let ``Test two`` () =
    
    let A =
        """
module A
open B
let x = B.x
"""
    let B =
        """
module B
let x = 3
"""
    
    let parsedA = parseSourceCode("A.fs", A)
    let visitedA = extractModuleRefs parsedA
    let parsedB = parseSourceCode("B.fs", B)
    let topB = topModuleOrNamespaces parsedB
    printfn $"Top B: %+A{topB}"
    printfn $"A refs: %+A{visitedA}"
    ()

[<Test>]
let ``Test big.fs`` () =
    let code = System.IO.File.ReadAllText("Big.fs")
    let parsedA = getParseResults code
    let visitedA = extractModuleRefs parsedA
    printfn $"A refs: %+A{visitedA}"
