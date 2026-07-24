module FSharp.Compiler.Service.Tests.TooltipGenericsTests

open System
open Xunit
open FSharp.Compiler.Symbols

[<Fact>]
let ``Regression.Generic.3773a`` () =
    assertTooltipContains "val M2: a: 'a -> obj" (markAtEndOfMarker "let rec M2<'a>(a:'a) = M2(a)" "let rec M")

[<Fact>]
let ``Regression.RecursiveDefinition.Generic.3773b`` () =
    assertTooltipContains "val M1: a: int -> 'a" (markAtEndOfMarker "let rec M1<'a>(a:'a) = M1(0)" "let rec M")

[<Fact(Skip = ".NET classes are treated differently now. Should this be revisited? Probably not.")>]
let ``FrameworkClass`` () =
    let source = "let l = new System.Collections.Generic.List<int>()"
    let marked = markAtEndOfMarker source "Generic.List"
    assertTooltipContains "member Capacity: int\n" marked
    assertTooltipContains "member Clear: unit -> unit\n" marked
    assertTooltipDoesNotContain "get_Capacity" marked
    assertTooltipDoesNotContain "set_Capacity" marked
    assertTooltipDoesNotContain "get_Count" marked
    assertTooltipDoesNotContain "set_Count" marked

[<Fact>]
let ``FrameworkClassNoMethodImpl`` () =
    assertTooltipDoesNotContain
        "System.Collections.ICollection.IsSynchronized"
        (markAtEndOfMarker "let l = new System.Collections.Generic.LinkedList<int>()" "Generic.LinkedList")

    assertTooltipContains
        "LinkedList"
        (markAtEndOfMarker "let l = new System.Collections.Generic.LinkedList<int>()" "Generic.LinkedList")

[<Fact>]
let ``Regression.ExtensionMethods.DocComments.Bug6028`` () =
    let source =
        """open System.Linq
let rec query: System.Linq.IQueryable<_> = null
let _ = query.Al{caret}l"""

    assertTooltipContains "IQueryable.All" source

    match (Checker.getSymbolUse source).Symbol with
    | :? FSharpMemberOrFunctionOrValue as m ->
        if m.XmlDocSig <> "M:System.Linq.Queryable.All``1(System.Linq.IQueryable{``0},System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}})" then
            failwithf "Unexpected XmlDocSig for query.All: %s" m.XmlDocSig

        let expectedAssembly =
#if NETCOREAPP
            "System.Linq.Queryable.dll"
#else
            "System.Core.dll"
#endif
        let basename = m.Assembly.FileName |> Option.map System.IO.Path.GetFileName

        if basename <> Some expectedAssembly then
            failwithf "Expected query.All to be defined in %s, but got %A" expectedAssembly basename
    | sym -> failwithf "Expected a member symbol for query.All, but got %A" sym

[<Fact>]
let ``GenericDotNetMethodShowsComment`` () =
    let source = "let _ = System.Linq.ParallelEnumerable.ElementA{caret}t"

    match (Checker.getSymbolUse source).Symbol with
    | :? FSharpMemberOrFunctionOrValue as m ->
        let expected =
            "M:System.Linq.ParallelEnumerable.ElementAt``1(System.Linq.ParallelQuery{``0},System.Int32"

        if not (m.XmlDocSig.Contains expected) then
            failwithf "Unexpected XmlDocSig for ParallelEnumerable.ElementAt: %s" m.XmlDocSig
    | sym -> failwithf "Expected a member symbol for ParallelEnumerable.ElementAt, but got %A" sym
