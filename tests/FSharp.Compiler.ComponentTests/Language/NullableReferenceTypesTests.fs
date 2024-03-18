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
let ``Cannot pass possibly null value to a strict function``() =
    FSharp """
module MyLib
let strictFunc(x:string) = ()
let nonStrictFunc(x:string | null) = strictFunc(x)
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 4, Col 49, Line 4, Col 50, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."]

[<Fact>]
let ``Boolean literal to string is not nullable`` () = 
    FSharp """module MyLibrary
let onlyWantNotNullString(x:string) = ()

let processBool () : string =
    onlyWantNotNullString (true.ToString())
    onlyWantNotNullString (false.ToString())

    true.ToString()
"""
    |> asLibrary
    |> withNoWarn 52 // The value has been copied to ensure the original is not mutated...
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Boolean to string is not nullable`` () = 
    FSharp """module MyLibrary
let onlyWantNotNullString(x:string) = ()

let processBool (b:bool) : string =
    let asString = b.ToString()  
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
let interpolated = $"This is fine %s{maybeNull}"
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
let ``Type inference with sprintfn`` () = 
    FSharp """module MyLibrary
let needsString(x:string) = ()

let myTopFunction inferedVal = 
    printfn "This is it %s" inferedVal  // There was a regression infering this to be (string | null)
    needsString inferedVal
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


[<InlineData("null")>]
[<InlineData(""" null | "" """)>]
[<InlineData(""" "" | null """)>]
[<InlineData(""" "" | " " | null """)>]
[<InlineData("(null)")>]
[<InlineData("(null) as _myUselessNullValue")>]
[<Theory>]
let ``Eliminate nullness after matching`` (tp) = 
    FSharp $"""module MyLibrary

let myFunction (input : string | null) : string = 
    match input with
    | {tp} -> ""
    | nonNullString -> nonNullString
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<InlineData("""(null,_aVal) | (_aVal, null) """)>]
[<InlineData("""(null,("" | null | _)) | (_, null)""")>]
[<Theory>]
let ``Eliminate tupled nullness after matching`` (tp) = 
    FSharp $"""module MyLibrary

let myFunction (input1 : string | null) (input2 : string | null): (string*string) = 
    match input1,input2 with
    | {tp} -> "",""
    | nns1,nns2 -> nns1,nns2
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<InlineData("""(null,"a") | ("b",null) """)>]
[<InlineData("(null,null)")>]
[<InlineData(""" null, a """)>]
[<InlineData(""" "a", "b" """)>]
[<InlineData(""" (_a,_b) when System.Console.ReadLine() = "lucky"  """)>]
[<InlineData("(_,null)")>]
[<Theory>]
let ``Should NOT eliminate tupled nullness after matching`` (tp) = 
    FSharp $"""module MyLibrary

let myFunction (input1 : string | null) (input2 : string | null): (string*string) = 
    match input1,input2 with
    | %s{tp} ->  "",""
    | nns1,nns2 -> nns1,nns2
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withErrorCode 3261
    
[<Fact>]
let ``WithNull used on anon type`` () = 
    FSharp """module MyLibrary

let strictFunc(arg: 'x when 'x : not null) = arg.ToString()    
let looseFunc(arg: _ | null) = arg

strictFunc({|ZZ=15;YZ="a"|}) |> ignore
looseFunc({|ZZ=15;YZ="a"|}) |> ignore

let maybeAnon : _ | null = {|Hello="there"|}
let maybeAnon2 : _ | null = null

strictFunc(maybeAnon) |> ignore
looseFunc(maybeAnon) |> ignore

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed
    
    
[<Fact>]
let ``WithNull on a DU`` () = 
    FSharp """module MyLibrary
type MyDu = A | B


let strictFunc(arg: 'x when 'x : not null) =
    printfn "%A" arg
    arg
    
let looseFunc(arg: _ | null) = arg

strictFunc(A) |> ignore
looseFunc(A) |> ignore

let maybeDu : _ | null = MyDu.A
let maybeDu2 : _ | null = null

strictFunc(maybeDu) |> ignore
strictFunc(maybeDu2) |> ignore

looseFunc(maybeDu2) |> ignore
looseFunc(maybeDu2) |> ignore

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed
    
[<Fact>]
let ``Nullnesss support for F# types`` () = 
    FSharp """module MyLibrary
type MyDu = A | B
type MyRecord = {X:int;Y:string}

let strictFunc(arg: 'x when 'x : not null) =
    printfn "%A" arg
    arg
    
let looseFunc(arg: _ | null) = arg

strictFunc(A) |> ignore
strictFunc({X=1;Y="a"}) |> ignore
strictFunc({|ZZ=15;YZ="a"|}) |> ignore
strictFunc((1,2,3)) |> ignore

looseFunc(A) |> ignore
looseFunc({X=1;Y="a"}) |> ignore
looseFunc({|ZZ=15;YZ="a"|}) |> ignore
looseFunc((1,2,3)) |> ignore

strictFunc(null) |> ignore
looseFunc(null) |> ignore

let maybeDu : _ | null = MyDu.A
let maybeRecd : MyRecord | null = {X=1;Y="a"}
let maybeAnon : _ | null = {|Hello="there"|}
let maybeTuple : (int*int) | null = null

strictFunc(maybeDu) |> ignore
strictFunc(maybeRecd) |> ignore
strictFunc(maybeAnon) |> ignore
strictFunc(maybeTuple) |> ignore

looseFunc(maybeDu) |> ignore
looseFunc(maybeRecd) |> ignore
looseFunc(maybeAnon) |> ignore
looseFunc(maybeTuple) |> ignore

type Maybe<'T> = 'T | null
let maybeTuple2 : Maybe<int*int> = null
strictFunc(maybeTuple2) |> ignore
looseFunc(maybeTuple2) |> ignore


"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed