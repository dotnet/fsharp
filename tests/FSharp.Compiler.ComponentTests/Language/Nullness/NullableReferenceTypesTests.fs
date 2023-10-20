module Language.NullableReferenceTypes

open Xunit
open FSharp.Test.Compiler

let withNullnessOptions cu =
    cu
    |> withCheckNulls
    |> withWarnOn 3261
    |> withWarnOn 3262
    |> withOptions ["--warnaserror+"]

let typeCheckWithStrictNullness cu =
    cu
    |> withNullnessOptions
    |> typecheck

[<Fact>]
let ``Does not duplicate warnings`` () =
    FSharp """
module MyLib
let getLength (x: string | null) = x.Length
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 3, Col 36, Line 3, Col 44, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."]

    
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

[<Theory>]
[<InlineData("fileExists(path)")>]
[<InlineData("fileExists path")>]
[<InlineData("fileExists null")>]
[<InlineData("path |> fileExists")>]
[<InlineData("null |> fileExists")>]
[<InlineData("System.IO.File.Exists(path)")>]
[<InlineData("System.IO.File.Exists(null)")>]
[<InlineData("path |> System.IO.File.Exists")>]
[<InlineData("null |> System.IO.File.Exists")>]
[<InlineData("System.String.IsNullOrEmpty(path)")>]
let ``Calling a nullAllowing API can still infer a withoutNull type``(functionCall) =
    FSharp $"""
module MyLib

let myStrictFunc(x: string) = x.GetHashCode()
let fileExists (path:string|null) = true

let myStringReturningFunc (path) = 
    let ex = {functionCall}
    myStrictFunc(path)
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

//[<Fact>]
// TODO Tomas - as of now, this does not bring the desired result
let ``Type inference with underscore or null`` () =
    FSharp $"""
module MyLib

let myFunc (path: _ | null) =
    System.IO.File.Exists(path)
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Type inference SystemIOFileExists`` () =
    FSharp $"""
module MyLib

let test() = 
    let maybeString : string | null = null
    System.IO.File.Exists(maybeString)

let myFunc path : string =
    let exists =  path |> System.IO.File.Exists
    path
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Type inference fsharp func`` () =
    FSharp $"""module MyLib

let fileExists (path:string|null) = true
let myStringReturningFunc (pathArg) : string = 
    let ex = pathArg |> fileExists
    pathArg
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


// P1: inline or not
// P2: type annotation for function argument
// P3: type annotation for cache
let MutableBindingAnnotationCombinations =
    [|
        for functionInlineFlag in [""
                                   "inline"] do
            for xArg in [""
                         ":'T"
                         ":'T|null"
                         ": _"
                         ": _|null"
                         ": string|null"
                         ": string"] do
                // No annotation or _ must work all the time
                for cacheArg in [""
                                 ": _"] do
                    yield [|functionInlineFlag :> obj; xArg :> obj; cacheArg :> obj|]

                // If we have a named type, the same one must work for cache binding as well
                if xArg.Contains("'T") || xArg.Contains("string|null") then
                    yield [|functionInlineFlag :> obj; xArg :> obj; xArg :> obj|]

                // If we have a type WithNull, using _|null should infer the exact same type
                if xArg.Contains("|null") || xArg.Contains("string") then
                    yield [|functionInlineFlag :> obj; xArg :> obj; ":_|null" :> obj|]

                if xArg = ":'T" then
                    for guard in [" when 'T:null"
                                  " when 'T:null and 'T:not struct"] do
                        yield [|functionInlineFlag :> obj; (xArg + guard) :> obj; "" :> obj|]

                if xArg = ":'T|null" then
                    for guard in [" when 'T:not struct"
                                  " when 'T:not null"
                                  " when 'T:not struct and 'T:not null"] do
                        yield [|functionInlineFlag :> obj; (xArg + guard) :> obj; "" :> obj|]
    |]

[<MemberData(nameof MutableBindingAnnotationCombinations)>]
[<Theory>]
let ``Mutable binding with a null literal`` inln xArg cache =
    FSharp $"""module MyLib

let %s{inln} f (x %s{xArg}) = 
    let mutable cache %s{cache} = null
    cache <- x
    
    match cache with
    | null -> failwith "It was null"
    | c -> c
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Mutable string binding initially assigned to null should not need type annotation``() = 
    FSharp """
module MyLib


let name = "abc"
let mutable cache  = null 
cache <- name 
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Mutable string binding assigned to null and matched against null``() = 
    FSharp """
module MyLib

let whatEver() =
    let mutable x = null
    x <- "abc"
    x


(* This is a comment 
let name = "abc"
let mutable cache  = null 
cache <- name 

match cache with
| null -> ()
| c -> ()*)

    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Mutable cache binding initially assigned to null should not need type annotation``() = 
    FSharp """
module MyLib
open System.Collections.Concurrent
open System

let mkCacheInt32 ()   =
        let mutable topLevelCache  = null 

        fun f (idx: int32) ->
            let cache =
                match topLevelCache with
                | null ->
                    let v = ConcurrentDictionary<int32, _>(Environment.ProcessorCount, 11)
                    topLevelCache <- v
                    v
                | v -> v

            match cache.TryGetValue idx with
            | true, res -> res
            | _ ->
                let res = f idx
                cache[idx] <- res
                res

    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can  infer underscore or null``() = 
    FSharp """
module MyLib
let iAcceptNullPartiallyInferred(arg: _ | null) = 42
let iHaveMissingContraint(arg: 'a | null) = 42
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Invalid usages of WithNull syntax``() = 
    FSharp """
module MyLib
let f1(x: option<string> | null) = ()
let f2(x: int | null) = ()
let f3(x: ('a*'b) | null) = ()
let f4(x: option<'a> | null) = ()
let f5(x: ('a | null) when 'a:struct) = ()
let f6(x: 'a | null when 'a:null) = ()
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
        [ Error 3261, Line 3, Col 11, Line 3, Col 32, "Nullness warning: The type 'string option' uses 'null' as a representation value but a non-null type is expected."
          Error 3260, Line 4, Col 11, Line 4, Col 21, "The type 'int' does not support a nullness qualification."
          Error 43, Line 4, Col 11, Line 4, Col 21, "A generic construct requires that the type 'int' have reference semantics, but it does not, i.e. it is a struct"
          Error 3260, Line 5, Col 11, Line 5, Col 25, "The type '('a * 'b)' does not support a nullness qualification."
          Error 3261, Line 6, Col 11, Line 6, Col 28, "Nullness warning: The type ''a option' uses 'null' as a representation value but a non-null type is expected."
          Error 43, Line 7, Col 28, Line 7, Col 37, "The constraints 'struct' and 'not struct' are inconsistent"
          Error 43, Line 8, Col 26, Line 8, Col 33, "The constraints 'null' and 'not null' are inconsistent"]

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

let myTopFunction inferredVal = 
    printfn "This is it %s" inferredVal  // There was a regression inferring this to be (string | null)
    needsString inferredVal
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

[<Fact>]
let ``Match null on two strings`` () = 
    FSharp """module MyLibrary
let len2r (str1: string | null) (str2: string | null) =
    match str1, str2 with
    | null, _ -> -1
    | _, null -> -1
    | s1, s2 -> s1.Length + s2.Length
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

type Maybe<'T when 'T:not struct> = 'T | null

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
            [ Error 3260, Line 4, Col 18, Line 4, Col 41, "The type '{| Hello: string |}' does not support a nullness qualification."
              Error 43, Line 4, Col 44, Line 4, Col 48, "The type '{| Hello: string |}' does not have 'null' as a proper value"]
    
    
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
            [ Error 3261, Line 6, Col 12, Line 6, Col 16, "Nullness warning: The type 'obj' does not support 'null'."
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
            [ Error 43, Line 4, Col 12, Line 4, Col 16, "The constraints 'null' and 'not null' are inconsistent"]
    
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
            [ Error 43, Line 4, Col 12, Line 4, Col 16, "The constraints 'null' and 'not null' are inconsistent"]
      
[<Fact>]
let ``Supports null in generic code`` () =
    FSharp """module MyLibrary
let myGenericFunction p = 
    match p with
    | null -> ()
    | p -> printfn "%s" (p.ToString()) 

[<AllowNullLiteral>]
type X(p:int) =
    member _.P = p

let myValOfX : X = null

myGenericFunction "HiThere"
myGenericFunction ("HiThere":string | null)
myGenericFunction (System.DateTime.Now)
myGenericFunction 123
myGenericFunction myValOfX

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
            [Error 3261, Line 13, Col 19, Line 13, Col 28, "Nullness warning: The type 'string' does not support 'null'."
             Error 193, Line 15, Col 20, Line 15, Col 39, "The type 'System.DateTime' does not have 'null' as a proper value"
             Error 1, Line 16, Col 19, Line 16, Col 22, "The type 'int' does not have 'null' as a proper value"]

[<Fact>]
let ``Null assignment in generic code`` () =
    FSharp """module MyLibrary
let myNullReturningFunction p  = 
    let mutable x = p
    x <- null
    x

[<AllowNullLiteral>]
type X(p:int) =
    member _.P = p

type Y (p:int) =
    member _.P = p

let myValOfX : X = null
let myValOfY : Y = Unchecked.defaultof<Y>

myNullReturningFunction "HiThere"                    |> ignore
myNullReturningFunction ("HiThere":string | null)    |> ignore
myNullReturningFunction (System.DateTime.Now)        |> ignore
myNullReturningFunction {|Anon=42|}                  |> ignore
myNullReturningFunction (1,2,3)                      |> ignore
myNullReturningFunction myValOfX                     |> ignore
myNullReturningFunction myValOfY                     |> ignore

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
                [Error 3261, Line 17, Col 25, Line 17, Col 34, "Nullness warning: The type 'string' does not support 'null'."
                 Error 193, Line 19, Col 26, Line 19, Col 45, "The type 'System.DateTime' does not have 'null' as a proper value"
                 Error 1, Line 20, Col 25, Line 20, Col 36, "The type '{| Anon: 'a |}' does not have 'null' as a proper value"
                 Error 1, Line 21, Col 26, Line 21, Col 31, "The type '('a * 'b * 'c)' does not have 'null' as a proper value"
                 Error 1, Line 23, Col 25, Line 23, Col 33, "The type 'Y' does not have 'null' as a proper value"]


[<Fact>]
let ``Match null with int`` () =
    FSharp """module MyLibrary
let test = 
    match null with
    | null -> true
    | 42 -> false

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
             [Error 1, Line 5, Col 7, Line 5, Col 9, "The type 'int' does not have 'null' as a proper value. See also test.fs(3,10)-(3,14)."
              Error 25, Line 3, Col 11, Line 3, Col 15, "Incomplete pattern matches on this expression."]

                 
[<Fact>]
let ``Nullness support for F# types`` () = 
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
            [ Error 43, Line 21, Col 12, Line 21, Col 16, "The constraints 'null' and 'not null' are inconsistent"
              Error 3260, Line 27, Col 18, Line 27, Col 34, "The type '(int * int)' does not support a nullness qualification."
              Error 43, Line 27, Col 37, Line 27, Col 41, "The type '(int * int)' does not have 'null' as a proper value"
              Error 3261, Line 29, Col 12, Line 29, Col 19, "Nullness warning: The type 'MyDu | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 30, Col 12, Line 30, Col 21, "Nullness warning: The type 'MyRecord | null' supports 'null' but a non-null type is expected."
              Error 43, Line 40, Col 36, Line 40, Col 40, "The type 'Maybe<int * int>' does not have 'null' as a proper value"]
                
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
let ``Option ofObj for PathGetDirectoryName`` () = 
    FSharp """module MyLibrary
open System.IO

let dirName = Path.GetDirectoryName ""
let whatIsThis1 = Option.ofObj dirName
let whatIsThis2 = Option.ofObj ( Path.GetDirectoryName "" ) 
let whatIsThis3 = Option.ofObj ("" |> Path.GetDirectoryName )  // Warnings were happening at this line only
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Option ofObj with fully annotated nullsupportive func`` () = 
    FSharp """module MyLibrary

let nullSupportiveFunc (x: string | null) : string | null = x
let maybePath : string | null = null
let whatIsThis3 = Option.ofObj (maybePath |> nullSupportiveFunc)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Option ofObj with calling id inside`` () = 
    FSharp """module MyLibrary

let maybePath : string | null = null
let whatIsThis5 = Option.ofObj (id maybePath)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

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
let ``Regression: Useless usage in nested calls`` () = 
    FSharp """module MyLibrary
open System.IO

let meTry = Option.ofObj (Path.GetDirectoryName "")
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


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

[<Fact>]
let ``Obj can be passed to not null constrained methods`` () = 
    FSharp """module MyLibrary

let objVal:(obj | null) = box 42


let mappableFunc =
    match objVal with
    |Null -> 42
    |NonNull o -> o.GetHashCode()
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Importing and processing contravariant interfaces`` () = 
    
    FSharp """module MyLibrary

open System
open System.Collections.Concurrent
open System.Collections.Generic


let cmp1 : IEqualityComparer<string> = StringComparer.Ordinal
let cmp2 : IEqualityComparer<string | null> = StringComparer.Ordinal
let stringHash = cmp2.GetHashCode("abc")
let nullHash = cmp2.GetHashCode(null)
let nullEquals = cmp2.Equals("abc", null)

let dict = ConcurrentDictionary<string, int> (StringComparer.Ordinal)
dict["ok"] <- 42

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<Fact>]
let ``Notnull constraint and inline annotated value`` () = 
    FSharp """module MyLibrary
open System

let f3 (x: 'T when 'T : not null) = 1

let v3 = f3 (null: obj) 
let v4 = f3 (null: String | null) 
let v5 = f3 (Some 1) 

let w3 = (null: obj) |> f3
let w4 = (null: String | null) |> f3

let v3WithNull = f3 (null: obj | null) 
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
            [ Error 3261, Line 6, Col 14, Line 6, Col 18, "Nullness warning: The type 'obj' does not support 'null'."
              Error 3261, Line 7, Col 14, Line 7, Col 33, "Nullness warning: The type 'String | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 8, Col 14, Line 8, Col 20, "Nullness warning: The type ''a option' uses 'null' as a representation value but a non-null type is expected."
              Error 3261, Line 10, Col 11, Line 10, Col 15, "Nullness warning: The type 'obj' does not support 'null'."
              Error 3261, Line 11, Col 35, Line 11, Col 37, "Nullness warning: The type 'String | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 13, Col 22, Line 13, Col 38, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."]