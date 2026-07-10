module FSharp.Compiler.Service.Tests.ParameterInfoGenericsTests

open Xunit

[<Fact>]
let ``Single.Generics.Typeof`` () =
    assertNoParameterInfo "typeof<int>({caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.MathAbs`` () =
    assertParameterInfoOverloads (List.replicate 7 ["value"]) """
open System
Math.Abs({caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.ExchangeInt`` () =
    assertParameterInfoOverloads (List.replicate 7 ["location1"; "value"]) """
open System.Threading
Interlocked.Exchange<int>({caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.Exchange`` () =
    assertParameterInfoOverloads (List.replicate 7 ["location1"; "value"]) """
open System.Threading
Interlocked.Exchange({caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.ExchangeUnder`` () =
    assertParameterInfoOverloads (List.replicate 7 ["location1"; "value"]) """
open System.Threading
Interlocked.Exchange<_> ({caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.Dictionary`` () =
    assertParameterInfoOverloads [ []; ["capacity"]; ["comparer"]; ["capacity"; "comparer"]; ["dictionary"]; ["dictionary"; "comparer"] ] """
System.Collections.Generic.Dictionary<_, option<int>>({caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.List`` () =
    assertParameterInfoOverloads [ []; ["capacity"]; ["collection"] ] """
new System.Collections.Generic.List< _ > ( {caret}"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.ListInt`` () =
    assertParameterInfoOverloads [ []; ["capacity"]; ["collection"] ] """
System.Collections.Generic.List<int>({caret}"""

[<Fact>]
let ``Single.Locations.GenericCtorWithNamespace`` () =
    assertHasParameterInfo "let _ = new System.Collections.Generic.Dictionary<_, _>({caret})"

[<Fact>]
let ``Single.Locations.GenericCtor`` () =
    assertHasParameterInfo """
open System.Collections.Generic
let _ = new Dictionary<_, _>({caret})"""

[<Fact>]
let ``Single.Locations.Multiline.IdentOnPrevLineWithGenerics`` () =
    assertHasParameterInfo """
open System.Collections.Generic
let d = Dictionar{caret}y<_, option< int >>
                (  )"""

[<Fact>]
let ``Single.Locations.GenericCtorWithoutNew`` () =
    assertHasParameterInfo "let d = System.Collections.Generic.Dictionar{caret}y<_, option< int >>   ( )"

[<Fact>]
let ``Single.Locations.Multiline.GenericTyargsOnTheSameLine`` () =
    assertHasParameterInfo "let dict3 = System.Collections.Generic.Dictionar{caret}y<_, \n                option< int>>( )"

[<Fact>]
let ``ParameterInfo.LocationOfParams.Bug112340`` () =
    assertHasParameterInfo """let a = typeof<N.
{caret}printfn "%A" a"""

[<Fact>]
let ``LocationOfParams.Generics1`` () =
    assertHasParameterInfo """
            let f<'T,'U>(x:'T, y:'U) = (y,x)
            let r = f{caret}<int,string>(42,"")"""

[<Fact>]
let ``LocationOfParams.Generics2`` () =
    assertHasParameterInfo """let x = System.Collections.Generic.Dictionar{caret}y<int,int>(42,null)"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.EvenWhenOverloadResolutionFails.Case2`` () =
    assertHasParameterInfo """
            open System.Collections.Generic
            open System.Linq
            let l = List<int>([||])
            l.Aggregate({caret}) // was once a bug"""

[<Fact>]
let ``Multi.Generic.Dictionary`` () =
    assertParameterInfoContains ["int"; "System.Collections.Generic.IEqualityComparer<obj>"] "System.Collections.Generic.Dictionar{caret}y<_, option<int>>(12,"

[<Fact(Skip = "95862 - [Unittests] parseInfo(TypeCheckResult.TypeCheckInfo).GetMethods cannot get MethodOverloads")>]
let ``Multi.Generic.HashSet`` () =
    assertParameterInfoContains ["Seq<'a>"; "System.Collections.Generic.IEqualityComparer<'a>"] "System.Collections.Generic.HashSet<int>({ 1 ..12 },{caret}"

[<Fact(Skip = "95862 - [Unittests] parseInfo(TypeCheckResult.TypeCheckInfo).GetMethods cannot get MethodOverloads")>]
let ``Multi.Generic.SortedList`` () =
    assertParameterInfoContains ["int"; "System.Collections.Generic.IComparer<'TKey>"] "System.Collections.Generic.SortedList<_,option<int>> (12,{caret}"
