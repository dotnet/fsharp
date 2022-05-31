namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module TypeAttributeTests = 

    [<Test>]
    let ``Attribute can be applied to type definition``() = 
        CompilerAssert.Pass
            """
[<Struct>]
type Point = {x:int; y:int}
            """
    [<Test>]
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

    [<Test>]
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
