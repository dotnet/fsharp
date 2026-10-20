// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Quotations.Patterns

module CodeQuotationsTests =

    [<Fact>]
    let ``Quotation on op_UnaryPlus(~+) compiles and runs`` () =
        Fsx """
open FSharp.Linq.RuntimeHelpers
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns

let eval q = LeafExpressionConverter.EvaluateQuotation q

let inline f x = <@ (~+) x @>
let x = <@ f 1 @>
let y : unit =
    match f 1 with
    | Call(_, methInfo, _) when methInfo.Name = "op_UnaryPlus" ->
        ()
    | e ->
        failwithf "did not expect expression for 'y': %A" e
let z : unit =
    match f 5 with
    | (CallWithWitnesses(_, methInfo, methInfoW, _, _) as e) when methInfo.Name = "op_UnaryPlus" && methInfoW.Name = "op_UnaryPlus$W" ->
        if ((eval e) :?> int) = 5 then
            ()
        else
            failwith "did not expect evaluation false"
    | e ->
        failwithf "did not expect expression for 'z': %A" e
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Delegate construction quotations are unaffected by the direct delegate optimization`` () =
        Fsx """
open System
open FSharp.Quotations.Patterns

let handlerCurried (x: int) (y: int) : unit = ()

type C(k: int) =
    member _.AddC (x: int) (y: int) : unit = ignore k

let check (label: string) (target: string) (expr: Quotations.Expr) =
    match expr with
    | NewDelegate(dty, _, _) when dty = typeof<Action<int, int>> -> ()
    | e -> failwithf "%s: expected NewDelegate of Action<int,int>, got %A" label e
    if not ((string expr).Contains target) then
        failwithf "%s: expected the quotation to reference target '%s', got %A" label target expr

let o = C(1)

// non-eta-expanded known function
check "nonEta" "handlerCurried" <@ Action<int, int>(handlerCurried) @>
// eta-expanded known function
check "etaCurried" "handlerCurried" <@ Action<int, int>(fun a b -> handlerCurried a b) @>
// non-eta-expanded instance method
check "instanceNonEta" "AddC" <@ Action<int, int>(o.AddC) @>
// eta-expanded instance method
check "instanceEta" "AddC" <@ Action<int, int>(fun a b -> o.AddC a b) @>

printfn "ok"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Quotation on decimal literal compiles and runs`` () =
        FSharp """
open Microsoft.FSharp.Quotations.DerivedPatterns

[<Literal>]
let x = 7m

let expr = <@ x @>

match expr with
| Decimal n -> printfn "%M" n
| _ -> failwith (string expr)
        """
        |> asExe
        |> withLangVersion80
        |> compileAndRun
        |> shouldSucceed

    // Tests for issues #11131 and #15648 - anonymous record field ordering
    // Note: The fix is in FSharp.Core/Linq.fs - these tests verify that queries
    // with anonymous records work correctly regardless of field order.
    // The expression tree structure tests are in FSharp.Core.UnitTests which directly
    // references the modified FSharp.Core.

    [<Fact>]
    let ``Anonymous records with both field orders produce equivalent results - issue 11131 and 15648`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type Wrapper = { Person: Person }

let data = [
    { Person = { Name = "One"; Id = 1 } }
    { Person = { Name = "Two"; Id = 2 } }
  ]

// Both orders should produce same results when executed
let resultsAlpha = 
    data.AsQueryable().Select(fun x -> {| A = x.Person.Name; B = x.Person.Id |}).ToList()
      
let resultsNonAlpha = 
    data.AsQueryable().Select(fun x -> {| B = x.Person.Id; A = x.Person.Name |}).ToList()

// Verify results are equivalent
if resultsAlpha.Count <> resultsNonAlpha.Count then
    failwith "Result counts don't match"

for i in 0 .. resultsAlpha.Count - 1 do
    if resultsAlpha.[i].A <> resultsNonAlpha.[i].A then
        failwithf "A values don't match at index %d" i
    if resultsAlpha.[i].B <> resultsNonAlpha.[i].B then
        failwithf "B values don't match at index %d" i
    
printfn "Both field orders produce equivalent results: %d items" resultsAlpha.Count
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Nested anonymous records work correctly`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type Wrapper = { Person: Person }

let data = [
    { Person = { Name = "One"; Id = 1 } }
  ]

// Nested anonymous records should work
let queryNested = 
    data.AsQueryable().Select(fun x -> {| Other = {| Name = x.Person.Name; Id = x.Person.Id |} |}).ToList()

if queryNested.Count <> 1 then
    failwith "Expected 1 result"
    
if queryNested.[0].Other.Name <> "One" then
    failwithf "Expected Name='One', got '%s'" queryNested.[0].Other.Name
    
if queryNested.[0].Other.Id <> 1 then
    failwithf "Expected Id=1, got %d" queryNested.[0].Other.Id
    
printfn "Nested anonymous record works correctly"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``F# record with non-declaration field order works correctly`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type PartialPerson = { LastName: string; ID : int }

let data = [ { Name = "One"; Id = 1 }; { Name = "Two"; Id = 2 } ]

// Declaration order
let query1 = data.AsQueryable().Select(fun p -> { LastName = p.Name; ID = p.Id }).ToList()
      
// Non-declaration order (swapped)
let query2 = data.AsQueryable().Select(fun p -> { ID = p.Id; LastName = p.Name }).ToList()

if query1.Count <> query2.Count then
    failwith "Result counts don't match"

for i in 0 .. query1.Count - 1 do
    if query1.[i].LastName <> query2.[i].LastName then
        failwithf "LastName values don't match at index %d" i
    if query1.[i].ID <> query2.[i].ID then
        failwithf "ID values don't match at index %d" i
    
printfn "Both F# record field orderings produce equivalent results"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    // [<ReflectedDefinition(true)>] on a parameter + inferred generic recursion must not ICE (FS0192 Iterate2D).
    [<Fact>]
    let ``ReflectedDefinition parameter with inferred generic recursion compiles - issue 20379`` () =
        FSharp """
module Test20379

open Microsoft.FSharp.Quotations

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0

let rec loop x =
    Capture.Run(fun n -> loop x)
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    // Original attachment's essential 'append' shape: inferred generic recursion under an auto-quoted parameter.
    [<Fact>]
    let ``ReflectedDefinition parameter with recursive append shape compiles - issue 20379`` () =
        FSharp """
module Test20379Append

open Microsoft.FSharp.Quotations

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<'T list -> 'T list>) : 'T list = []

let rec append xs ys =
    Capture.Run(fun zs ->
        match xs with
        | [] -> ys
        | h :: t -> h :: append t ys)
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    // Runtime seed: inferred two-parameter generic recursion must preserve exact generic-argument order,
    // and the auto-quoted definition must agree with the executed value.
    [<Fact>]
    let ``ReflectedDefinition parameter preserves generic argument order at runtime - issue 20379`` () =
        Fsx """
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers

let mutable captured: Expr option = None

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) =
        captured <- Some body
        7

let rec loop x y =
    Capture.Run(fun n -> if n = 0 then 7 else loop x y)

let check<'T, 'U> (x: 'T) (y: 'U) =
    if loop x y <> 7 then failwith "Unexpected result"
    match captured with
    | Some(WithValue(value, _, (Lambda(v, IfThenElse(_, _, Call(None, method, [_; _]))) as definition))) ->
        if v.Name <> "n" then failwith "Lambda parameter changed"
        if method.GetGenericArguments() <> [|typeof<'T>; typeof<'U>|] then failwith "Incorrect generic arguments"
        let actual = unbox<int -> int> value
        let reflected = LeafExpressionConverter.EvaluateQuotation definition |> unbox<int -> int>
        if actual 1 <> 7 || reflected 1 <> 7 then failwith "Value and quotation disagree"
    | expression -> failwithf "Unexpected quotation: %A" expression

check 42 "text"
check "text" 42
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    [<Fact>]
    let ``Mutual inferred generic recursion under auto-quote compiles both directions - issue 20379`` () =
        FSharp """
module Test20379Mutual

open Microsoft.FSharp.Quotations

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0

let rec ping x y =
    Capture.Run(fun n -> pong x y)
and pong x y =
    Capture.Run(fun n -> ping x y)
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    [<Fact>]
    let ``Auto-quote with nested lambdas captured and shadowed names agrees value and quotation - issue 20379`` () =
        Fsx """
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers

let mutable captured: Expr option = None

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) =
        captured <- Some body
        7

let rec loop x y =
    let k = 3
    let rec helper (m: int) = if m <= 0 then k else helper (m - 1)
    Capture.Run(fun n ->
        let n = n + helper 2
        if n = 0 then 7 else loop x y)

let check<'T, 'U> (x: 'T) (y: 'U) =
    if loop x y <> 7 then failwith "Unexpected result"
    match captured with
    | Some(WithValue(value, _, definition)) ->
        let actual = unbox<int -> int> value
        let reflected = LeafExpressionConverter.EvaluateQuotation definition |> unbox<int -> int>
        if actual 1 <> reflected 1 then failwith "Value and quotation disagree"
    | expression -> failwithf "Unexpected quotation: %A" expression

check 42 "text"
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    [<Theory>]
    [<InlineData("static member Run", "Capture.Run")>]
    [<InlineData("member _.Run", "(Capture()).Run")>]
    let ``Auto-quote inferred generic recursion works for static and instance methods - issue 20379`` (decl: string) (call: string) =
        FSharp(sprintf """
module Test20379Methods

open Microsoft.FSharp.Quotations

type Capture() =
    %s ([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0

let rec loop x y =
    %s(fun n -> if n = 0 then 7 else loop x y)
        """ decl call)
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    [<Fact>]
    let ``Auto-quote inferred generic recursion works across an F# assembly boundary - issue 20379`` () =
        let lib =
            FSharp """
namespace Api
open Microsoft.FSharp.Quotations
type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0
            """
            |> withName "Api20379"
            |> asLibrary

        FSharp """
module Consumer20379
open Api
let rec loop x y =
    Capture.Run(fun n -> if n = 0 then 7 else loop x y)
        """
        |> withReferences [ lib ]
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379
    [<Fact>]
    let ``Auto-quoted argument is constructed exactly once - issue 20379`` () =
        Fsx """
open Microsoft.FSharp.Quotations

let mutable constructions = 0

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0

let makeBody () =
    constructions <- constructions + 1
    (fun (n: int) -> n)

let rec loop x y =
    Capture.Run(makeBody ())

let _ = loop 1 2
if constructions <> 1 then failwithf "Expected exactly one construction, got %d" constructions
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379 - controls that must keep working
    [<Theory>]
    [<InlineData("false-attribute", "type Capture = static member Run([<ReflectedDefinition(false)>] body: Expr<int -> int>) = 0\nlet rec loop x y = Capture.Run(fun n -> if n = 0 then 7 else loop x y)")>]
    [<InlineData("nonrecursive", "type Capture = static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0\nlet go x = Capture.Run(fun n -> n + 1)")>]
    [<InlineData("ordinary-parameter", "type Capture = static member Run(body: int -> int) = 0\nlet rec loop x y = Capture.Run(fun n -> if n = 0 then 7 else loop x y)")>]
    [<InlineData("explicit-quotation", "type Capture = static member Run(body: Expr<int -> int>) = 0\nlet rec loop x y = Capture.Run(<@ fun n -> if n = 0 then 7 else loop x y @>)")>]
    [<InlineData("monomorphic-recursion", "type Capture = static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0\nlet rec loop (x: int) = Capture.Run(fun n -> if n = 0 then 7 else loop x)")>]
    [<InlineData("explicit-generic-recursion", "type Capture = static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0\nlet rec loop<'T, 'U> (x: 'T) (y: 'U) = Capture.Run(fun n -> if n = 0 then 7 else loop x y)")>]
    let ``Auto-quote controls keep compiling - issue 20379`` (_name: string) (body: string) =
        FSharp(sprintf "module Test20379Controls\nopen Microsoft.FSharp.Quotations\n%s" body)
        |> asLibrary
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/20379 - inner generic functions remain unsupported (FS1230), not an ICE
    [<Fact>]
    let ``Inner generic function inside quotation remains FS1230 - issue 20379`` () =
        FSharp """
module Test20379InnerGeneric
open Microsoft.FSharp.Quotations
type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) = 0
let rec loop x y =
    Capture.Run(fun n ->
        let inline g (z: 'a) = z
        if n = 0 then 7 else loop (g x) (g y))
        """
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 1230

    // https://github.com/dotnet/fsharp/issues/20379
    // A monomorphic recursive *data* value (lazy-initialized 'let rec') gains nothing from fixup-link
    // preservation. Its recursive-use fixup node is re-mutated to a lazy 'Force' by the initialization-graph
    // elimination, so preserving the link would leak that 'Force' into the captured quotation. The auto-quoted
    // definition must reference the value directly, exactly as before the issue 20379 fix.
    [<Fact>]
    let ``Auto-quote of monomorphic recursive data value does not leak a lazy Force - issue 20379`` () =
        Fsx """
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

#nowarn "21"
#nowarn "40"

let mutable captured: Expr option = None

type Capture =
    static member Run([<ReflectedDefinition(true)>] body: Expr<int -> int>) =
        captured <- Some body
        (fun (n: int) -> n)

let rec data : int -> int =
    Capture.Run(fun n -> data n)

if data 3 <> 3 then failwith "Unexpected result"

let rec containsForce expr =
    match expr with
    | Call(_, method, _) when method.Name = "Force" -> true
    | ExprShape.ShapeVar _ -> false
    | ExprShape.ShapeLambda(_, body) -> containsForce body
    | ExprShape.ShapeCombination(_, args) -> List.exists containsForce args

match captured with
| Some(WithValue(_, _, definition)) ->
    if containsForce definition then failwithf "Lazy Force leaked into quotation: %A" definition
| expression -> failwithf "Unexpected quotation: %A" expression
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed