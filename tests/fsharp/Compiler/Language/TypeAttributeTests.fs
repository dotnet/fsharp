namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module TypeAttributeTests = 

    [<Test>]
    let ``Attribute can be assigned to type definition``() = 
        CompilerAssert.Pass
            """
[<Struct>]
type Point = {x:int; y:int}
            """
    [<Test>]
    let ``Attribute cannot be assigned to optional type extension``() = 
        CompilerAssert.TypeCheckSingleError
            """
open System

[<NoEquality>]
type String with 
    member this.test = 42
            """
            FSharpErrorSeverity.Error
            3245
            (4, 1, 4, 15)
            "Attributes cannot be assigned to type extensions."

    [<Test>]
    let ``Attribute cannot be assigned to intrinsic type extension``() = 
        CompilerAssert.TypeCheckSingleError
            """
type Point = {x:int; y:int}

[<NoEquality>]
type Point with 
    member this.test = 42
            """
            FSharpErrorSeverity.Error
            3245
            (4, 1, 4, 15)
            "Attributes cannot be assigned to type extensions."
