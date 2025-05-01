namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics


module TypeAttributeTests = 

    [<Fact>]
    let ``Attribute can be applied to type definition``() = 
        CompilerAssert.Pass
            """
[<Struct>]
type Point = {x:int; y:int}
            """
    [<Fact>]
    let ``Attribute cannot be applied to optional type extension``() = 
        CompilerAssert.TypeCheckSingleError
            """
open System

[<NoEquality>]
type String with 
    member this.test = 42
            """
            FSharpDiagnosticSeverity.Error
            3246
            (4, 1, 4, 15)
            "Attributes cannot be applied to type extensions."

    [<Fact>]
    let ``Attribute cannot be applied to intrinsic type extension``() = 
        CompilerAssert.TypeCheckSingleError
            """
type Point = {x:int; y:int}

[<NoEquality>]
type Point with 
    member this.test = 42
            """
            FSharpDiagnosticSeverity.Error
            3246
            (4, 1, 4, 15)
            "Attributes cannot be applied to type extensions."
