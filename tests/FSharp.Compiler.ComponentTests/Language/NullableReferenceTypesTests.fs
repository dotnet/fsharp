module Language.NullableReferenceTypes

open Xunit
open FSharp.Test.Compiler

let typeCheckWithStrictNullness cu =
    cu
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> typecheck

[<Fact>]
let ``Boolean to string is not nullable`` () = 
    FSharp """module MyLibrary
let onlyWantNotNullString(x:string) = ()

let processBool (b:bool) : (string|null) =
    let asString = b.ToString()
    onlyWantNotNullString asString
    onlyWantNotNullString (true.ToString())
    onlyWantNotNullString (false.ToString())

    asString
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Printing a nullable string should pass`` () = 
    FSharp """module MyLibrary
let maybeNull : string | null = null
let nonNullString = "abc"
let printedValueNotNull = sprintf "This is not null: %s" nonNullString
let printedValueNull = sprintf "This is null: %s" maybeNull
let interpolated = $"This is fine {maybeNull}"
let interpolatedAnnotatedNotNull = $"This is fine %s{nonNullString}"
let interpolatedAnnotatedNullable = $"This is not null %s{maybeNull}"
let interpolateNullLiteral = $"This is not null %s{null}"
let sprintfnNullLiteral = sprintf "This is null: %s" null
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<Fact>]
let ``Printing a nullable object should pass`` () = 
    FSharp """module MyLibrary
let maybeNull : string | null = null
let maybeUri : System.Uri | null = null
let okString = "abc"
let printViaO = sprintf "This is null: %O and this is null %O and this is not null %O" maybeNull maybeUri okString
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<Fact>]
let ``Printing a nullable array via percent A should pass`` () = 
    FSharp """module MyLibrary
let maybeArray : ((string array) | null) = null
let arrayOfMaybes : ((string | null) array ) = [|null|]
let printViaA = sprintf "This is null: %A and this has null inside %A" maybeArray arrayOfMaybes
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``WhatIf the format itself is null`` () = 
    FSharp """module MyLibrary
[<Literal>]
let thisCannotBeAFormat : string | null = null
[<Literal>]
let maybeLiteral : string | null = "abc"
[<Literal>]
let maybeLiteralWithHole : string | null = "Look at me %s"
[<Literal>]
let notNullLiteral : string = "abc"
let doStuff() = 
    printfn notNullLiteral
    printfn maybeLiteral
    printfn maybeLiteralWithHole thisCannotBeAFormat
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed