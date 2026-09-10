module FSharp.Compiler.Service.Tests.ParameterInfoStringsInterpolationTests

open Xunit

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; call ident on a previous line, GetMethods returns no group at the lower-line `(`")>]
let ``Single.Locations.Multiline.IdentOnPrevLine`` () =
    assertHasParameterInfo """
open System
do Console.WriteLine
        ({caret}"Multiline")"""

[<Fact>]
let ``LocationOfParams.GenericMethodExplicitTypeArgs()`` () =
    assertHasParameterInfo """
type T<'a> =
    static member M(x:int, y:string) = x + y.Length
let x = T<int>.M{caret}(1, "test")    """
