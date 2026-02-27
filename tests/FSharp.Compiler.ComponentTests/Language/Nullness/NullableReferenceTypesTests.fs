module Language.NullableReferenceTypes

open Xunit
open FSharp.Test.Compiler

let withNullnessOptions cu =
    cu
    |> withCheckNulls
    |> withWarnOn 3261
    |> withWarnOn 3262
    |> withNoWarn 52 //The value has been copied to ensure the original..
    |> withNoWarn 60 // Override implementations in augmentations are now deprecated...
    |> withOptions ["--warnaserror+"]

let typeCheckWithStrictNullness cu =
    cu
    |> withNullnessOptions
    |> typecheck


[<Fact>]
let ``Can imply notstruct for classconstraint`` () =
    FSharp """module Foo =
    let failIfNull<'a when 'a : null> (a : 'a) : 'a option =
        (a |> Option.ofObj)
    """
    |> asLibrary
    |> typecheck  // This has nullable off!
    |> shouldSucceed  

[<Fact>]
let ``Warning on nullness hidden behind interface upcast`` () =
    FSharp """module Test

open System.IO
open System

// This is bad - input is nullable, output is not = must warn
let whatisThis (s:Stream|null) : IDisposable =
    s"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 8, Col 5, Line 8, Col 6, "Nullness warning: The types 'IDisposable' and 'IDisposable | null' do not have compatible nullability."]

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Report warning when applying anon record to a nullable generic return value`` () =
    FSharp """
open System.Text.Json
type R = { x: int }
type RA = {| x: int |}

[<EntryPoint>]
let main _args =
    let a = JsonSerializer.Deserialize<{| x: int |}> "null"
    let _a = a.x

    let b = JsonSerializer.Deserialize<RA> "null"
    let _b = b.x

    let c = JsonSerializer.Deserialize<R> "null"
    let _c = c.x
    0"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
            [ Error 3265, Line 8, Col 13, Line 8, Col 60, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for '{| x: int |}'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 11, Col 13, Line 11, Col 50, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for '{| x: int |}'. Nullness warnings won't be reported correctly for such types."
              Error 3261, Line 15, Col 14, Line 15, Col 17, "Nullness warning: The types 'R' and 'R | null' do not have compatible nullability."]

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Report warning when generic type instance creates a null-disallowed type`` () =
    FSharp """
open System.Text.Json

[<Measure>]
type mykg
type mykgalias = int<mykg>

[<MeasureAnnotatedAbbreviation>]  // Despite being an abbreviation, this points to a string - does not warn
type string<[<Measure>] 'Measure> = string|null

[<EntryPoint>]
let main _args =
    let a = JsonSerializer.Deserialize<{| x: int |}> "null"
    let a = JsonSerializer.Deserialize<int> "null"
    let a = JsonSerializer.Deserialize<int * float> "null"
    let a = JsonSerializer.Deserialize<struct(int * float)> "null" 
    let a = JsonSerializer.Deserialize<int<mykg>> "null"
    let a = JsonSerializer.Deserialize<mykgalias> "null"

    // Should be ok from here below
    let b = JsonSerializer.Deserialize<int list> "null"
    let b = JsonSerializer.Deserialize<mykgalias array> "null"
    let b = JsonSerializer.Deserialize<Set<int<mykg>>> "null"
    let b = JsonSerializer.Deserialize<string<mykg>> "null"

    0"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
            [ Error 3265, Line 13, Col 13, Line 13, Col 60, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for '{| x: int |}'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 14, Col 13, Line 14, Col 51, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for 'System.Int32'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 15, Col 13, Line 15, Col 59, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for 'int * float'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 16, Col 13, Line 16, Col 67, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for 'struct (int * float)'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 17, Col 13, Line 17, Col 57, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for 'int<mykg>'. Nullness warnings won't be reported correctly for such types."
              Error 3265, Line 18, Col 13, Line 18, Col 57, "Application of method 'Deserialize' attempted to create a nullable type ('T | null) for 'int<mykg>'. Nullness warnings won't be reported correctly for such types."]
   



[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Does report when null goes to DateTime Parse`` () =

    FSharp """module TestLib
open System
let parsedDate = DateTime.Parse(null:(string|null))
let parseDate2(s:string|null) = DateTime.Parse(s)
let parsedDate3 = DateTime.Parse(null)
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics     
                [Error 3261, Line 3, Col 18, Line 3, Col 52, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
                 Error 3261, Line 4, Col 33, Line 4, Col 50, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
                 Error 3261, Line 5, Col 19, Line 5, Col 39, "Nullness warning: The type 'string' does not support 'null'."]

[<Fact>]
let ``Downcasts and typetests with nullables``() = 
    FSharp """module MyLib
type AB = A | B

let warnOnCastFromNull (o: objnull) = o :?> AB
let warnOnCastFromNonNullToNull(o:obj) = o :?> (AB | null)
let warnOnTypeTestNullable(o:objnull) = o :? (AB|null)
let warnOnTypeTestRepeatedNestedNullable(o:obj) = o :? (list<((AB | null) array | null) > |null)

let doNotWarnOnCastFromNullToOption(o:objnull) = o :?> Option<AB>
let doNotWarnOnTypeTestOption(o:objnull) = o :? Option<AB>
let doNotWarnOnGenericCastToNullableGeneric<'b when 'b:not null and 'b:not struct>(a: objnull) = a :?> ('b|null)
let doNotWarnOnDownCastNestedNullable(o:obj) = o :? list<AB|null>
let doNotWarnOnDowncastRepeatedNestedNullable(o:objnull) = o :? list<((AB | null) array | null) > 

    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics
                [ Error 3264, Line 4, Col 39, Line 4, Col 47, "Nullness warning: Downcasting from 'objnull' into 'AB' can introduce unexpected null values. Cast to 'AB|null' instead or handle the null before downcasting."
                  Error 3060, Line 5, Col 42, Line 5, Col 59, "This type test or downcast will erase the provided type 'AB | null' to the type 'AB'"
                  Error 3060, Line 6, Col 41, Line 6, Col 55, "This type test or downcast will erase the provided type 'AB | null' to the type 'AB'"
                  Error 3060, Line 7, Col 51, Line 7, Col 97, "This type test or downcast will erase the provided type 'List<(AB | null) array | null> | null' to the type 'List<AB array>'"]


[<Fact>]
let ``Can infer nullable type if first match handler returns null`` () =
    FSharp """module TestLib

let myFunc x =
    match x with
    | 0 -> null
    | i -> "x"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can NOT infer nullable type if second match handler returns null`` () =
    FSharp """module TestLib

let myFunc x defaultValue =
    match x with
    | 0 -> defaultValue
    | 1 -> null
    | i -> "y"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 7, Col 12, Line 7, Col 15, "Nullness warning: The type 'string' does not support 'null'.. See also test.fs(7,11)-(7,14)."]

[<Fact>]
let ``Can infer nullable type if first match handler returns masked null`` () =
    FSharp """module TestLib

let thisIsNull : string|null = null
let myFunc x =
    match x with
    | 0 -> thisIsNull
    | i -> "x"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can infer nullable type from first branch of ifthenelse`` () =
    FSharp """module TestLib

let myFunc x =
    if x = 0 then null else "x"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can NOT infer nullable type from second branch of ifthenelse`` () =
    FSharp """module TestLib

let myFunc x =
    if x = 0 then "x" else null
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 4, Col 28, Line 4, Col 32, "Nullness warning: The type 'string' does not support 'null'."]

[<Fact>]
let ``Can NOT infer nullable type from second branch of nested elifs`` () =
    FSharp """module TestLib

let myFunc x =
    if x = 0 then "x" elif x=1 then null else "y"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 4, Col 37, Line 4, Col 41, "Nullness warning: The type 'string' does not support 'null'."]

[<Fact>]
let ``Can convert generic value to objnull arg`` () =
    FSharp """module TestLib

let writeObj(tw:System.IO.TextWriter, a:'a) =
    tw.Write(a)

writeObj(System.IO.TextWriter.Null,null)
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can pass nulll to objnull arg`` () =
    FSharp """module TestLib
let doStuff args = 
    let ty = typeof<string>
    let m = ty.GetMethod("ToString") |> Unchecked.nonNull
    m.Invoke(null,args)    
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can cast from objTy to interfaceTy`` () =
    FSharp """module TestLib
open System
let safeHolder : IDisposable =
    { new obj() with
            override x.Finalize() = (x :?> IDisposable).Dispose()
        interface IDisposable with
            member x.Dispose() =
                GC.SuppressFinalize x
    }
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can _use_ a nullable IDisposable`` () =
    FSharp """module TestLib
open System
let workWithResource (getD:int -> (IDisposable|null)) =
    use _ = getD 15
    15

    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

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
let ``Does report warning on obj to static member`` () =
    FSharp """
type Test() =
    member _.XX(o:obj) = ()
    static member X(o: obj) = ()
    static member XString(x:string) = ()
let x: obj | null = null
Test.X x // warning expected
let y2 = Test.X(x) // warning also expected
Test.X(null:(obj|null)) // warning also expected
let t = Test()
t.XX(x)
Test.XString(null)
Test.XString("x":(string|null))
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
              [ Error 3261, Line 7, Col 8, Line 7, Col 9, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
                Error 3261, Line 7, Col 1, Line 7, Col 9, "Nullness warning: The types 'obj' and 'obj | null' do not have compatible nullability."
                Error 3261, Line 8, Col 17, Line 8, Col 18, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
                Error 3261, Line 8, Col 10, Line 8, Col 19, "Nullness warning: The types 'obj' and 'obj | null' do not have compatible nullability."
                Error 3261, Line 9, Col 8, Line 9, Col 23, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
                Error 3261, Line 9, Col 1, Line 9, Col 24, "Nullness warning: The types 'obj' and 'obj | null' do not have compatible nullability."
                Error 3261, Line 11, Col 6, Line 11, Col 7, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
                Error 3261, Line 11, Col 1, Line 11, Col 8, "Nullness warning: The types 'obj' and 'obj | null' do not have compatible nullability."
                Error 3261, Line 12, Col 14, Line 12, Col 18, "Nullness warning: The type 'string' does not support 'null'."
                Error 3261, Line 13, Col 14, Line 13, Col 31, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Typar infered to nonnull obj`` () =

    FSharp """module Tests
let asObj(x:obj) = x
let asObjNull(x:objnull) = x

let genericWithoutNull x = asObj x
let genericWithNull x = asObjNull x

let result0 = genericWithoutNull null
let result1 = genericWithoutNull ("":(obj|null))
let result2 = genericWithoutNull 15
let result3 = genericWithoutNull "xxx"
let result4 = genericWithoutNull ("xxx":(string|null))
let result5 = genericWithNull null
let result6 = genericWithNull 15
let result7 = genericWithNull "xxx"
let result8 = genericWithNull ("":(obj|null))

    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
            [ Error 43, Line 8, Col 34, Line 8, Col 38, "The constraints 'null' and 'not null' are inconsistent"
              Error 3261, Line 9, Col 35, Line 9, Col 48, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 12, Col 35, Line 12, Col 54, "Nullness warning: The type 'string | null' supports 'null' but a non-null type is expected."]

    
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
        Error 3261, Line 4, Col 49, Line 4, Col 50, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]
 
[<Fact>]
let ``Can have nullable prop of same type T within a custom type T``() =
    FSharp """
module MyLib
type T () =
    let mutable v : T | null = null
    member val P : T | null = null with get, set
    member this.M() =
        v <- null
        this.P <- null
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

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
          Error 3260, Line 5, Col 11, Line 5, Col 25, "The type ''a * 'b' does not support a nullness qualification."
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

[<Literal>]
let duWithExtensionProvidedToString = """A | B 
module Functions =
    let printMyType (x:MyCustomType) : string = "xxx" 
type MyCustomType with
    override x.ToString() : string = Functions.printMyType x
    """

[<Literal>]
let duWithExtensionProvidedNullableToString = """A | B 

type MyCustomType with
    override x.ToString() = null
    """

let toStringCodeSnippet myTypeDef = 
    FSharp $"""module MyLibrary

type MyCustomType = {myTypeDef}

let onlyWantNotNullString(x:string) = ()

let processBool (x:MyCustomType) =
    onlyWantNotNullString(x.ToString())
"""
    |> asLibrary
    |> typeCheckWithStrictNullness

[<Theory>]
[<InlineData("A | B ")>]
[<InlineData( """A | B with override this.ToString() : string = "xyz" """)>]
[<InlineData( """A | B with override this.ToString() = "xyz" """)>]
[<InlineData( """A | B with override this.ToString() = System.Console.ReadLine() |> string """)>]
[<InlineData(duWithExtensionProvidedToString)>]
[<InlineData("{F : int} ")>]
[<InlineData(" {| F : string |} ")>]
[<InlineData(" (struct{| F : string |}) ")>]
[<InlineData(" int * string ")>]
[<InlineData(" (struct(string * int)) ")>]
let ``Generated ToString() methods are not nullable`` (myTypeDef) = 
    toStringCodeSnippet myTypeDef
    |> shouldSucceed

[<Theory>]
[<InlineData( """A | B with override this.ToString() : (string|null) = null """)>]
[<InlineData(duWithExtensionProvidedNullableToString)>]
let ``ToString override warns if it returns nullable`` (myTypeDef) = 
    toStringCodeSnippet myTypeDef
    |> shouldFail
    |> withDiagnosticMessage "With nullness checking enabled, overrides of .ToString() method must return a non-nullable string. You can handle potential nulls via the built-in string function."

// https://github.com/dotnet/fsharp/issues/17539
[<Theory>]
[<InlineData("int", "processInt")>]
[<InlineData("float", "processFloat")>]
[<InlineData("decimal", "processDecimal")>]
let ``ToString on value type with UoM is not nullable`` (valueType: string, funcName: string) =
    FSharp $"""module MyLibrary

[<Measure>]
type mykg

let onlyWantNotNullString(x:string) = ()

let {funcName} (x:{valueType}<mykg>) =
    onlyWantNotNullString(x.ToString())
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``ToString on UoM type alias is not nullable`` () =
    FSharp """module MyLibrary

[<Measure>]
type mykg

type mykgalias = int<mykg>

let onlyWantNotNullString(x:string) = ()

let processAlias (x:mykgalias) =
    onlyWantNotNullString(x.ToString())
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
let myGenericFunctionForInnerNotNull (p:_|null) = 
    match p with
    | null -> ()
    | nnp -> printfn "%s" (nnp.ToString()) 

let myGenericFunctionSupportingNull (p) = 
    match p with
    | null -> 0
    | nnp -> hash nnp


[<AllowNullLiteral>]
type X(p:int) =
    member _.P = p

let myValOfX : X = null

myGenericFunctionForInnerNotNull "HiThere"
myGenericFunctionForInnerNotNull ("HiThere":string | null)
myGenericFunctionSupportingNull myValOfX |> ignore
myGenericFunctionSupportingNull ("HiThere":string | null) |> ignore

"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

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
                 Error 1, Line 21, Col 26, Line 21, Col 31, "The type ''a * 'b * 'c' does not have 'null' as a proper value"
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
              Error 3260, Line 27, Col 18, Line 27, Col 34, "The type 'int * int' does not support a nullness qualification."
              Error 43, Line 27, Col 37, Line 27, Col 41, "The type 'int * int' does not have 'null' as a proper value"
              Error 3261, Line 29, Col 12, Line 29, Col 19, "Nullness warning: The type 'MyDu | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 30, Col 12, Line 30, Col 21, "Nullness warning: The type 'MyRecord | null' supports 'null' but a non-null type is expected."
              Error 43, Line 40, Col 36, Line 40, Col 40, "The type 'Maybe<int * int>' does not have 'null' as a proper value"]
    
    
[<Fact>]
let ``Nullness support for flexible types`` () =
    FSharp """module MyLibrary
open System
let dispose (x: IDisposable | null) : unit =
    match x with
    | null -> ()
    | d -> d.Dispose()

let useThing (thing: #IDisposable) =
    try
        printfn "%O" thing
    finally
        dispose thing // Warning used to be here, should not warn! """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Nullness support for flexible types - opposite`` () =
    FSharp """module MyLibrary
open System
let dispose (x: IDisposable) : unit =
    x.Dispose()

let useThing (thing: #IDisposable | null) =
    try
        printfn "%O" thing
    finally
        dispose thing // Warning should be here, because 'thing' can be null!        
        """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 10, Col 17, Line 10, Col 22, "Nullness warning: The types 'IDisposable' and ''a | null' do not have compatible nullability."]


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
         Error 3261, Line 9, Col 42, Line 9, Col 51, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]


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
let ``Useless null check when used in piping`` () = 
    FSharp """module MyLibrary

let foo = "test"
let bar = foo |> Option.ofObj // Should produce FS3262, but did not
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3262, Line 4, Col 18, Line 4, Col 30, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."]

[<Fact>]
let ``Useless null check when used in multi piping`` () = 
    FSharp """module MyLibrary

let myFunc whateverArg = 
    whateverArg |> string |> Option.ofObj
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3262, Line 4, Col 30, Line 4, Col 42, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."]

[<Fact>]
let ``Useless null check when used in more exotic pipes`` () = 
    FSharp """module MyLibrary

let functionComposition x = x |> (string >> Option.ofObj)
let doublePipe x = ("x","y") ||> (fun x _y -> Option.ofObj x)
let intToint x = x + 1
let pointfree = intToint >> string >> Option.ofObj
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics 
        [ Error 3262, Line 3, Col 45, Line 3, Col 57, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
          Error 3262, Line 4, Col 60, Line 4, Col 61, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
          Error 3262, Line 6, Col 39, Line 6, Col 51, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."]

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

[<InlineData("t :?> 'T")>]
[<InlineData("(downcast t) : 'T")>]
[<InlineData("t |> unbox<'T>")>]
[<InlineData("(t : objnull) :?> 'T")>]
[<Theory>]
let ``Unsafe cast should not insist on not null constraint``(castOp:string) =

    FSharp $"""module MyLibrary
let test<'T> () =
    let t = obj()
    {castOp}"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed


[<InlineData("null")>]
[<InlineData("if false then null else []")>]
[<InlineData("if false then [] else null")>]
[<InlineData("(if false then [] else null) : (_ list | null)")>]
[<InlineData("[] : (_ list | null)")>]
[<Theory>]
let ``Nullness in inheritance chain`` (returnExp:string) = 
    
    FSharp $"""module MyLibrary

[<AbstractClass>]
type Generator<'T>() =
    abstract Values: unit -> 'T

[<Sealed>]
type ListGenerator<'T>() =
    inherit Generator<List<'T> | null>()

    override _.Values() = {returnExp}
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
              Error 3261, Line 11, Col 11, Line 11, Col 15, "Nullness warning: The type 'String | null' supports 'null' but a non-null type is expected."
              Error 3261, Line 13, Col 22, Line 13, Col 38, "Nullness warning: The type 'obj | null' supports 'null' but a non-null type is expected."]


[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Option type detected as null for NullabilityInfoContext`` () =

    Fsx """

open System
open System.Reflection

type MyClass(StringOrNull: string | null, StringOption: string option, ObjNull: objnull, ObjOrNull: obj | null) =
    member val StringOrNull = StringOrNull
    member val StringOption = StringOption
    member val ObjNull = ObjNull
    member val ObjOrNull = ObjOrNull

let classType = typeof<MyClass>
let ctor = classType.GetConstructors() |> Seq.exactlyOne
let paramInfos = ctor.GetParameters()


let nrtContext = NullabilityInfoContext()
for paramInfo in paramInfos do
    let nrtInfo = nrtContext.Create(paramInfo)
    let readState = nrtInfo.ReadState
    let writeState = nrtInfo.WriteState
    printfn $"{paramInfo.Name} => {readState} / {writeState}" """
    |> withNullnessOptions
    |> compile
    |> run
    |> verifyOutputContains [|"StringOption => Nullable / Nullable"|]

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Option type has WithNull annotation in IL`` () =
    FSharp """
module Test

let myOption () : option<string> = None  """
    |> withNullnessOptions
    |> compile
    // The option<> itself is nullable (2), the string inside is not (1)
    |> verifyIL ["      
      .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> myOption() cil managed
      {
        .param [0]
        .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 02 01 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  ret
      }"]

// Regression https://github.com/dotnet/fsharp/issues/18286
[<Fact>]
let ``Equality and hashcode augmentation is null safe`` () =

    Fsx """

type Bar = { b: string | null  }
type Foo = { f: Bar | null }
type DUFoo = WithNull of (Foo|null)
let a = { f = null }
let b = { f = null }

let c = WithNull(null)
let d = WithNull(null)

let e = WithNull(a)
let f = WithNull(b)

[<EntryPoint>]
let main _ = 
    printf "Test %A;" ({b = null} = {b = null})
    printf ",1 %A" (a = b)
    printf ",2 %A" (a.GetHashCode() = b.GetHashCode())
    printf ",3 %A" (c = d)
    printf ",4 %A" (c.GetHashCode() = d.GetHashCode())
    printf ",5 %A" (e = f)
    printf ",6 %A" (e = c)
    printf ",7 %A" (e.GetHashCode() = f.GetHashCode())
    printf ",8 %A" (e.GetHashCode() = c.GetHashCode())

    printf ",9 %A" (a > b)
    printf ",10 %A" (c > d)
    printf ",11 %A" (e > f)
    printf ",12 %A" (e > c)
    0
"""
    |> withNullnessOptions
    |> withOptimization false
    |> asExe
    |> compile
    //|> verifyIL ["abc"]
    |> run
    |> verifyOutputContains [|"Test true;,1 true,2 true,3 true,4 true,5 true,6 false,7 true,8 false,9 false,10 false,11 false,12 true"|]

[<Fact>]
let ``No nullness warning when casting non-nullable to IEquatable`` () =
    FSharp """module Test

open System

let x = ""
let y = x :> IEquatable<string> // Should not warn about nullness
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// Regression for https://github.com/dotnet/fsharp/issues/18488
// This tests the exact scenario from the issue with obj | null return type
[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Match null branch should refine variable to non-null - exact issue scenario`` () =
    FSharp """module Test

let bar : obj =
    let getEnvironmentVariable : string -> (obj|null) = fun _ -> null

    match "ENVVAR" |> getEnvironmentVariable with
    | null -> obj() // return some obj in null case
    | x -> x // x should be obj, not obj | null - no warning expected
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// Regression for https://github.com/dotnet/fsharp/issues/18488
[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Match null branch should refine variable to non-null in subsequent branches - type alias`` () =
    FSharp """module Test

// Type alias for nullable obj
type objnull = obj | null

let getEnvAliasObj (_: string) : objnull = failwith "stub"

// After matching null case, x should be refined to non-null
let valueAliasObj =
    match getEnvAliasObj "ENVVAR" with
    | null -> "missing"
    | x -> x.GetType().Name // x should be obj, not obj | null - no warning expected
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// Regression for https://github.com/dotnet/fsharp/issues/18488
[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Match null branch should refine variable to non-null in subsequent branches - direct nullable type`` () =
    FSharp """module Test

let getValue () : string | null = failwith "stub"

// After matching null case, x should be refined to non-null
let result =
    match getValue () with
    | null -> "missing"
    | x -> x.ToUpper() // x should be string, not string | null - no warning expected
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// Regression for https://github.com/dotnet/fsharp/issues/18488
[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Match null branch should refine variable to non-null - Environment.GetEnvironmentVariable`` () =
    FSharp """module Test

// Real-world scenario from issue #18488
let value =
    match System.Environment.GetEnvironmentVariable "ENVVAR" with
    | null -> "missing"
    | x -> x.ToLower() // x should be string, not string | null - no warning expected
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// https://github.com/dotnet/fsharp/issues/17727
[<Fact>]
let ``Option.toObj infers input as non-nullable option - issue 17727`` () =
    let nullAgnosticLib =
        FSharp """
module NullAgnosticLib

let work (x: string) = x.Length
        """
        |> withName "NullAgnosticLib"

    FSharp """
module NullAwareCode

open NullAgnosticLib

let whatever x =
    work (Option.toObj x)

let testCorrectInference : string option -> int = whatever
    """
    |> asLibrary
    |> withReferences [nullAgnosticLib]
    |> withCheckNulls
    |> ignoreWarnings
    |> compile
    |> shouldSucceed

// https://github.com/dotnet/fsharp/issues/18013
[<Fact>]
let ``Pipe operator error location points to nullable argument - issue 18013`` () =
    FSharp """module Test

let foo a b = "hello" + a + b
let bar : string | null = "test"
let result = bar |> foo "mr"
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 5, Col 14, Line 5, Col 17, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Left pipe operator error location with nullness`` () =
    FSharp """module Test

let foo a b = "hello" + a + b
let bar : string | null = "test"
let result = foo "mr" <| bar
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 5, Col 26, Line 5, Col 29, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``User-defined pipe operator error location points to nullable argument`` () =
    FSharp """module Test

let inline (|>!) (x: 'a) (f: 'a -> 'b) = f x

let foo (s: string) = s.Length
let bar : string | null = "test"
let result = bar |>! foo
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 7, Col 14, Line 7, Col 17, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``AllowNullLiteral type constructor call does not produce false positive - issue 18021`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyClass() = class end

let x : MyClass = MyClass()

let y : MyClass = MyClass()
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``AllowNullLiteral type variable still warns when passed to non-null API - issue 18021`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyClass() = class end

let consumeNonNull<'T when 'T : not null> (value: 'T) = value

let nullableValue : MyClass | null = null
let result = consumeNonNull nullableValue
    """
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 8, Col 21, Line 8, Col 35, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
        Error 3261, Line 9, Col 29, Line 9, Col 42, "Nullness warning: The type 'MyClass | null' supports 'null' but a non-null type is expected."
    ]

// https://github.com/dotnet/fsharp/issues/18334
let private iLookupTypeExtensionSource = """
open System.Collections.Generic
open System.Linq

type ILookup<'Key, 'Value when 'Key : not null> with
    static member Empty = Seq.empty<KeyValuePair<'Key,'Value>>.ToLookup((fun kv -> kv.Key), (fun kv -> kv.Value))
"""

[<Fact>]
let ``Type extension with not null constraint on ILookup should compile - Regression18334`` () =
    FSharp ("module TestModule" + iLookupTypeExtensionSource)
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Type extension not null constraint rejects nullable type arg at call site`` () =
    FSharp ("module TestModule" + iLookupTypeExtensionSource + "\nlet badLookup : ILookup<string | null, int> = ILookup.Empty\n")
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 8, Col 47, Line 8, Col 60, "Nullness warning: The type 'string | null' supports 'null' but a non-null type is expected."
    ]

[<Fact>]
let ``Type extension not null constraint accepts non-null type arg at call site`` () =
    FSharp ("module TestModule" + iLookupTypeExtensionSource + "\nlet goodLookup : ILookup<string, int> = ILookup.Empty\n")
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Type extension with comparison constraint on List should fail`` () =
    FSharp """module TestModule
open System.Collections.Generic

type List<'T when 'T : comparison> with
    static member Sorted (lst: List<'T>) = lst |> Seq.sort |> List
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 957, Line 4, Col 6, Line 4, Col 10, "One or more of the declared type parameters for this type extension have a missing or wrong type constraint not matching the original type constraints on 'List<_>'"
        Error 340, Line 1, Col 1, Line 1, Col 1, "The signature and implementation are not compatible because the declaration of the type parameter 'T' requires a constraint of the form 'T: comparison"
    ]

// https://github.com/dotnet/fsharp/issues/18034
[<Fact>]
let ``Computation expression with SRTP Delay should work with nullness`` () =
    FSharp """module TestModule

type ResultBuilder() =
    member _.Return(x: 'T) : Result<'T,'E> = Ok x
    member _.Bind(m: Result<'T,'E>, f: 'T -> Result<'U,'E>) : Result<'U,'E> = Result.bind f m
    member _.Delay(f: unit -> Result<'T,'E>) : Result<'T,'E> = f()
    member _.Zero() : Result<unit, 'E> = Ok ()

let result = ResultBuilder()

type SomeRecordWrapper = { Value: int }

let repro () : Result<SomeRecordWrapper, string> =
    result {
        let! parsed = Ok 42
        return { Value = parsed }
    }
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// https://github.com/dotnet/fsharp/issues/18034
let private fsharpPlusSrtpDelaySource =
    FSharp """module TestModule

open System.Runtime.InteropServices

type Delay =
    static member inline Delay(call: unit -> Result<'T,'E>, _output: Result<'T,'E>, [<Optional>]_mthd: obj) : Result<'T,'E> = call()

let inline invokeDelay (call: unit -> ^R) : ^R =
    ((^R or Delay) : (static member Delay : (unit -> ^R) * ^R * obj -> ^R) call, Unchecked.defaultof<_>, Unchecked.defaultof<_>)

type MonadBuilder() =
    member inline _.Return(x: 'T) : Result<'T,'E> = Ok x
    member inline _.ReturnFrom(x: Result<'T,'E>) : Result<'T,'E> = x
    member inline _.Bind(m: Result<'T,'E>, f: 'T -> Result<'U,'E>) : Result<'U,'E> = Result.bind f m
    member inline _.Delay(body: unit -> ^R) : ^R = invokeDelay body
    member inline _.Zero() : Result<unit,'E> = Ok ()

let monad = MonadBuilder()

type SomeRecordWrapper = { Value: int }

let repro () : Result<SomeRecordWrapper, string> =
    monad {
        let! parsed = Ok 42
        return { Value = parsed }
    }
"""
    |> asLibrary
    |> withOptions ["--nowarn:64"]

[<Fact>]
let ``FSharpPlus-style SRTP Delay without nullness should work`` () =
    fsharpPlusSrtpDelaySource
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``FSharpPlus-style SRTP Delay with nullness should work`` () =
    fsharpPlusSrtpDelaySource
    |> typeCheckWithStrictNullness
    |> shouldSucceed

// https://github.com/dotnet/fsharp/issues/19042
[<Fact>]
let ``Tuple null elimination with restricting non-nullable int pattern`` () =
    FSharp """module MyLibrary

let test (s: string) = ()

let main () =
    let x: string | null = null
    let y: int = 5
    match x, y with
    | null, _ -> ()
    | s, 5 -> test s
    | s, _ -> test s
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Tuple null elimination with nullable first position`` () =
    FSharp """module MyLibrary

let test (s: string) = ()

let main () =
    let x: string | null = null
    let y: int = 5
    match x, y with
    | null, _ -> ()
    | s, _ -> test s
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Tuple null elimination with multiple clauses handles each clause independently`` () =
    FSharp """module MyLibrary

let test (s: string) = ()

let main () =
    let x: string | null = null
    let y: int = 5
    match x, y with
    | null, 0 -> ()
    | null, _ -> ()
    | s, _ -> test s
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Tuple null elimination blocked when multiple nullable elements are non-wild`` () =
    FSharp """module MyLibrary

let test (s: string) (t: string) = ()

let main () =
    let x: string | null = null
    let y: string | null = null
    match x, y with
    | null, _ -> ()
    | s, t -> test s t
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 10, Col 22, Line 10, Col 23, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
    ]

[<Fact>]
let ``Signature conformance rejects impl with not null constraint missing from sig`` () =
    let sigSource = """
module MyLib

val doWork<'T> : 'T -> int
"""
    let implSource = """
module MyLib

let doWork<'T when 'T : not null> (x: 'T) = 42
"""
    Fsi sigSource
    |> withAdditionalSourceFile (FsSource implSource)
    |> withNullnessOptions
    |> compile
    |> shouldFail
    |> withDiagnostics [
        Error 340, Line 4, Col 12, Line 4, Col 14, "The signature and implementation are not compatible because the declaration of the type parameter 'T' requires a constraint of the form 'T: not null"
    ]

[<Fact>]
let ``Signature conformance rejects impl missing not null constraint from sig`` () =
    let sigSource = """
module MyLib

val doWork<'T when 'T : not null> : 'T -> int
"""
    let implSource = """
module MyLib

let doWork<'T> (x: 'T) = 42
"""
    Fsi sigSource
    |> withAdditionalSourceFile (FsSource implSource)
    |> withNullnessOptions
    |> compile
    |> shouldFail
    |> withDiagnostics [
        Error 341, Line 4, Col 12, Line 4, Col 14, "The signature and implementation are not compatible because the type parameter 'T' has a constraint of the form 'T: not null but the implementation does not. Either remove this constraint from the signature or add it to the implementation."
    ]

[<Fact>]
let ``Signature conformance accepts matching not null constraint`` () =
    let sigSource = """
module MyLib

val doWork<'T when 'T : not null> : 'T -> int
"""
    let implSource = """
module MyLib

let doWork<'T when 'T : not null> (x: 'T) = 42
"""
    Fsi sigSource
    |> withAdditionalSourceFile (FsSource implSource)
    |> withNullnessOptions
    |> compile
    |> shouldSucceed

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``ToString on MeasureAnnotatedAbbreviation reference type is still nullable`` () =
    FSharp """module MyLibrary

[<Measure>]
type mykg

[<MeasureAnnotatedAbbreviation>]
type mystring<[<Measure>] 'Measure> = string

let onlyWantNotNullString(x:string) = ()

let processMyStr (x:mystring<mykg>) =
    onlyWantNotNullString(x.ToString())
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 12, Col 27, Line 12, Col 39, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
        Error 3261, Line 12, Col 27, Line 12, Col 39, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
    ]

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``ToString on reference type still returns nullable string`` () =
    FSharp """module MyLibrary

let onlyWantNotNullString(x:string) = ()

let processObj (x:obj) =
    onlyWantNotNullString(x.ToString())
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 6, Col 27, Line 6, Col 39, "Nullness warning: The types 'string' and 'string | null' do not have compatible nullability."
        Error 3261, Line 6, Col 27, Line 6, Col 39, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."
    ]

[<Fact>]
let ``Double pipe operator error location with nullness`` () =
    FSharp """module Test

let foo (a: string) (b: string) = a + b
let bar : string | null = null
let result = (bar, "hello") ||> foo
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 5, Col 15, Line 5, Col 27, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Triple pipe operator error location with nullness`` () =
    FSharp """module Test

let foo3 (a: string) (b: string) (c: string) = a + b + c
let bar : string | null = null
let result = (bar, "hello", "world") |||> foo3
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 5, Col 15, Line 5, Col 36, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Non-pipe partial application with nullable captured arg`` () =
    FSharp """module Test

let apply (x: string) (f: string -> int) = f x
let bar : string | null = null
let result = apply bar (fun s -> s.Length)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 5, Col 20, Line 5, Col 23, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Deeply nested pipe error points to original nullable value`` () =
    FSharp """module Test

let step1 (s: string) = s + "!"
let step2 (s: string) = s.Length
let bar : string | null = null
let result = bar |> step1 |> step2
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [Error 3261, Line 6, Col 14, Line 6, Col 17, "Nullness warning: A non-nullable 'string' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression."]

[<Fact>]
let ``Pipe with non-nullable value produces no warning`` () =
    FSharp """module Test

let step (s: string) = s + "!"
let bar : string = "hello"
let result = bar |> step
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Unchecked defaultof on AllowNullLiteral type does not warn when consumed directly`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyClass() = class end
let consumeNonNull (x: MyClass) = ()
let x = Unchecked.defaultof<MyClass>
consumeNonNull x
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``AllowNullLiteral static factory return is not widened to nullable`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyClass() =
    static member Create() : MyClass = MyClass()
let consumeNonNull (x: MyClass) = ()
let x = MyClass.Create()
consumeNonNull x
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

let private allowNullLiteralConsumeNonNullPreamble = """module Test

[<AllowNullLiteral>]
type MyClass() = class end

let consumeNonNull<'T when 'T : not null> (x: 'T) = ()
"""

[<Fact>]
let ``AllowNullLiteral constructor does not warn but defaultof and explicit nullable do for not null constraint`` () =
    let source =
        allowNullLiteralConsumeNonNullPreamble + """
consumeNonNull (MyClass())

consumeNonNull (Unchecked.defaultof<MyClass>)

let nullable : MyClass | null = null
consumeNonNull nullable
"""
    FSharp source
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 10, Col 17, Line 10, Col 45, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
        Error 3261, Line 12, Col 16, Line 12, Col 30, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
        Error 3261, Line 13, Col 16, Line 13, Col 24, "Nullness warning: The type 'MyClass | null' supports 'null' but a non-null type is expected."
    ]

[<Fact>]
let ``Constructor of AllowNullLiteral type does not warn for generic not null constraint`` () =
    let source =
        allowNullLiteralConsumeNonNullPreamble + """
let test () = consumeNonNull (MyClass())
"""
    FSharp source
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Static factory of AllowNullLiteral type checked against generic not null constraint`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyClass() =
    static member Create() : MyClass = MyClass()

let consumeNonNull<'T when 'T : not null> (x: 'T) = ()

let test () = consumeNonNull (MyClass.Create())
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 9, Col 31, Line 9, Col 47, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
    ]

[<Fact>]
let ``Let-bound AllowNullLiteral constructor result checked against generic not null constraint`` () =
    let source =
        allowNullLiteralConsumeNonNullPreamble + """
let instance = MyClass()
let test () = consumeNonNull instance
"""
    FSharp source
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Explicit nullable AllowNullLiteral binding fails generic not null constraint`` () =
    let source =
        allowNullLiteralConsumeNonNullPreamble + """
let maybeNull : MyClass | null = MyClass()
let test () = consumeNonNull maybeNull
"""
    FSharp source
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 8, Col 17, Line 8, Col 31, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
        Error 3261, Line 9, Col 30, Line 9, Col 39, "Nullness warning: The type 'MyClass | null' supports 'null' but a non-null type is expected."
    ]

[<Fact>]
let ``Mutable AllowNullLiteral binding warns for not null constraint`` () =
    // Mutable bindings strip KnownFromConstructor — they can be reassigned to null.
    let source =
        allowNullLiteralConsumeNonNullPreamble + """
let mutable x = MyClass()
x <- null
let test () = consumeNonNull x
"""
    FSharp source
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnostics [
        Error 3261, Line 10, Col 30, Line 10, Col 31, "Nullness warning: The type 'MyClass' supports 'null' but a non-null type is expected."
    ]

[<Fact>]
let ``AllowNullLiteral constructor piped compiles without error`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyWrapper(value: string) =
    member _.Value = value

let result = "hello" |> MyWrapper
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``AllowNullLiteral constructor piped with interface object expression compiles without error`` () =
    FSharp """module Test

type IMyInterface =
    abstract Name: string

[<AllowNullLiteral>]
type MyWrapper(impl: IMyInterface) =
    member _.Impl = impl

let result =
    { new IMyInterface with
        member _.Name = "test" }
    |> MyWrapper
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``AllowNullLiteral constructor piped with value type arg compiles without error`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyWrapper(value: int) =
    member _.Value = value

let result = 5 |> MyWrapper
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``AllowNullLiteral constructor used as first-class function compiles without error`` () =
    FSharp """module Test

[<AllowNullLiteral>]
type MyWrapper(value: string) =
    member _.Value = value

let items = ["a"; "b"; "c"] |> List.map MyWrapper
let f : string -> MyWrapper = MyWrapper
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FSharp.Test.FactForNETCOREAPPAttribute>]
let ``Type with comparison constraint compiles and runs correctly under strict nullness`` () =
    FSharp """module Test

type MyWrapper<'T when 'T : comparison>(value: 'T) =
    member _.Value = value
    override this.Equals(other: obj) =
        match other with
        | :? MyWrapper<'T> as o -> this.Value = o.Value
        | _ -> false
    override this.GetHashCode() = hash this.Value
    interface System.IComparable with
        member this.CompareTo(other: obj) =
            match other with
            | :? MyWrapper<'T> as o -> compare this.Value o.Value
            | _ -> 0

let w1 = MyWrapper(1)
let w2 = MyWrapper(2)
let result = compare w1 w2
printf "%d" result

[<EntryPoint>]
let main _ = 0
"""
    |> withNullnessOptions
    |> asExe
    |> compile
    |> run
    |> verifyOutputContains [|"-1"|]
