namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module MultiDimensionalArrayTests =

    [<Theory>]
    [<InlineData(1,"array2d")>]
    [<InlineData(5,"array6d")>]
    [<InlineData(31,"array32d")>]
    let ``MultiDimensional array type can be written with or without backticks``(commas:int,shortcut:string) =
        let commaString = System.String(',',commas)
        FSharp $"""
module MultiDimArrayTests
let backTickStyle :  int ``[{commaString}]`` = Unchecked.defaultof<_>
let cleanStyle : int [{commaString}] = backTickStyle
let shortCutStyle : int {shortcut} = cleanStyle
        """
        |> compile
        |> shouldSucceed

    [<Fact>]    
    let ``Multidimensional array - reports an error if types are not matching`` () = 
        let commaString = System.String(',',30)
        FSharp $"""
module MultiDimArrayErrorTests
let cleanStyle : int [{commaString}] = Unchecked.defaultof<_>
let shortCutStyle : int array32d = cleanStyle
        """
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches """This expression was expected to have type.*"""

    [<Fact>]   
    let ``Multidimensional with rank over 32 cannot be defined`` () = 
        let commaString = System.String(',',42)
        FSharp $"""
module MultiDimArrayErrorTests
let cleanStyle : int [{commaString}] = Unchecked.defaultof<_>
        """
        |> compile
        |> shouldFail


