module Language.NullableReferenceTypes

open Xunit
open FSharp.Test.Compiler

let typeCheckWithStrictNullness cu =
    cu
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> withWarnOn 3262
    |> withOptions ["--warnaserror+"]
    |> compile

    
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
let ``Eliminate aliased nullness after matching`` () = 
    FSharp $"""module MyLibrary

type Maybe<'T> = 'T | null

let myFunction (input : string Maybe) : string = 
    match input with
    | null -> ""
    | nonNullString -> nonNullString
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed
    
[<Fact>]
let ``WithNull used on anon type`` () = 
    FSharp """module MyLibrary

let maybeAnon : _ | null = {|Hello="there"|}
let maybeAnon2 : {|Hello:string|} | null = null
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
            [ Error 3260, Line 4, Col 18, Line 4, Col 41, "The type '{| Hello: string |}' does not support a nullness qualitification."
              Error 3261, Line 4, Col 44, Line 4, Col 48, "Nullness warning: The type '{| Hello: string |}' does not support 'null'."]
    
    
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
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 18, Col 12, Line 18, Col 19, "Nullness warning: The type 'MyDu | null' supports 'null' but a non-null type is expected."
        Error 3261, Line 19, Col 12, Line 19, Col 20, "Nullness warning: The type ''a | null' supports 'null' but a non-null type is expected."]
    
[<Fact>]
let ``Strict func handling of obj type`` () = 
    FSharp """module MyLibrary
let strictFunc(arg: 'x when 'x : not null) = printfn "%s" (arg.ToString())
 
strictFunc("hi") |> ignore
strictFunc({|Anon=5|}) |> ignore
strictFunc(null:obj) |> ignore
strictFunc(null:(obj|null)) |> ignore
strictFunc(null:(string|null)) |> ignore
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
            [ Error 3261, Line 6, Col 12, Line 6, Col 20, "Nullness warning: The type 'obj' supports 'null' but a non-null type is expected."
              Error 3261, Line 7, Col 18, Line 7, Col 26, "Nullness warning: The type 'obj' supports 'null' but a non-null type is expected."
              Error 3261, Line 7, Col 12, Line 7, Col 27, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 8, Col 12, Line 8, Col 30, "Nullness warning: The type 'string | null' supports 'null' but a non-null type is expected."]
        
        

[<Fact>]
let ``Strict func null literal`` () = 
    FSharp """module MyLibrary
let strictFunc(arg: 'x when 'x : not null) = printfn "%s" (arg.ToString()) 

strictFunc(null) |> ignore    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
            [ Error 3261, Line 4, Col 12, Line 4, Col 16, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."]
    
[<Fact>]
let ``Strict func null literal2`` () = 
    FSharp """module MyLibrary
let strictFunc(arg: 'x when 'x : not null) = printfn "%s" (arg.ToString()) 

strictFunc(null) |> ignore
strictFunc({|Anon=5|}) |> ignore
strictFunc("hi") |> ignore   """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
            [ Error 3261, Line 4, Col 12, Line 4, Col 16, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."]
         
    
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
    |> shouldFail
    |> withDiagnostics     
            [ Error 3260, Line 27, Col 18, Line 27, Col 34, "The type '(int * int)' does not support a nullness qualitification."
              Error 3261, Line 27, Col 37, Line 27, Col 41, "Nullness warning: The type '(int * int)' does not support 'null'."
              Error 3261, Line 29, Col 12, Line 29, Col 19, "Nullness warning: The type 'MyDu | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 30, Col 12, Line 30, Col 21, "Nullness warning: The type 'MyRecord | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 40, Col 36, Line 40, Col 40, "Nullness warning: The type 'Maybe<int * int>' does not support 'null'."]
                
[<Fact>]
let ``Static member on Record with null arg`` () =
    FSharp """module MyLibrary

type MyRecord = {X:string;Y:int}
    with static member Create(x:string) = {X=x;Y = 42}

let thisWorks = MyRecord.Create("xx")
let thisShouldWarn = MyRecord.Create(null)
let maybeNull : string | null = "abc"
let thisShouldAlsoWarn = MyRecord.Create(maybeNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics  
        [Error 3261, Line 7, Col 38, Line 7, Col 42, "Nullness warning: The type 'string' does not support 'null'."
         Error 3261, Line 9, Col 42, Line 9, Col 51, "Nullness warning: The types 'string' and 'string | null' do not have equivalent nullability."]


[<Fact>]
let ``Option ofObj should remove nullness when used in a function`` () = 
    FSharp """module MyLibrary
let processOpt2 (s: string | null) : string option = Option.ofObj s"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Option ofObj should remove nullness when piping`` () = 
    FSharp """module MyLibrary
let processOpt (s: string | null) : string option =
    let stringOpt = Option.ofObj s
    stringOpt
let processOpt3 (s: string | null) : string option = s |> Option.ofObj
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Option ofObj called in a useless way raises warning`` () = 
    FSharp """module MyLibrary

let processOpt1 (s: string) = Option.ofObj s
let processOpt2 (s: string) : option<string> = 
    Option.ofObj s
let processOpt3 (s: string) : string option = 
    let sOpt = Option.ofObj s
    sOpt
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
        [ Error 3262, Line 3, Col 44, Line 3, Col 45, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
          Error 3262, Line 5, Col 18, Line 5, Col 19, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
          Error 3262, Line 7, Col 29, Line 7, Col 30, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."]


[<Fact>]
let ``Option ofObj called on a string literal`` () = 
    FSharp """module MyLibrary
let whatIsThis = Option.ofObj "abc123"
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withErrorCodes [3262]

[<Fact>]
let ``Useless null pattern match`` () = 
    FSharp """module MyLibrary

let clearlyNotNull = "42"
let mappedVal = 
    match clearlyNotNull with
    | null -> 42
    | _ -> 43
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 6, Col 7, Line 6, Col 11, "Nullness warning: The type 'string' does not support 'null'."]

[<Fact>]
let ``Useless usage of nonNull utility from fscore`` () = 
    FSharp """module MyLibrary

let clearlyNotNull = "42"
let mappedVal = nonNull clearlyNotNull
let maybeNull : string | null = null
let mappedMaybe = nonNull maybeNull
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3262, Line 4, Col 25, Line 4, Col 39, "Value known to be without null passed to a function meant for nullables: You can remove this `nonNull` assertion."]

[<Fact>]
let ``Useless usage of null active patterns from fscore`` () = 
    FSharp """module MyLibrary

let clearlyNotNull = "42"
let mapped1 = 
    match clearlyNotNull with
    | NonNullQuick safe -> safe

let mapped2 =
    match clearlyNotNull with
    |Null -> 0
    |NonNull _ -> 1
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
        [ Error 3262, Line 6, Col 7, Line 6, Col 24, "Value known to be without null passed to a function meant for nullables: You can remove this |NonNullQuick| pattern usage."
          Error 3262, Line 10, Col 6, Line 10, Col 10, "Value known to be without null passed to a function meant for nullables: You can remove this |Null|NonNull| pattern usage."
          Error 3262, Line 11, Col 6, Line 11, Col 15, "Value known to be without null passed to a function meant for nullables: You can remove this |Null|NonNull| pattern usage."]