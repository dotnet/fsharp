# F# Query Expression Bug Fixes - Comprehensive Plan

## Overview

This plan addresses **11 open bugs** in the F# query expression implementation, labeled `Area-Queries` and `type:Bug` in the dotnet/fsharp repository. The query system is implemented primarily in:

- **`src/FSharp.Core/Query.fs`** - QueryBuilder type and query translation
- **`src/FSharp.Core/Linq.fs`** - LeafExpressionConverter and quotation-to-LINQ conversion
- **`src/FSharp.Core/QueryExtensions.fs`** - Runtime helpers, grouping adapters
- **`src/Compiler/Checking/Expressions/CheckComputationExpressions.fs`** - Compiler-side query support

---

## Issue 1: LeafExpressionConverter.EvaluateQuotation fails for some cases
**GitHub:** https://github.com/dotnet/fsharp/issues/19099

### Problem Description
`FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation` throws exceptions when:
1. Quotation contains sequencing (`a; b`)
2. Quotation contains mutation (`a <- b` or `a.b <- c`)
3. Quotation returns `unit` (void method calls like `Console.WriteLine`)

The root cause is in `Linq.fs` around line 905 - the conversion doesn't handle `Sequential`, `VarSet`, `FieldSet`, `PropertySet` patterns, and LINQ's `Expression<Func<unit>>` can't handle `System.Void` return types.

### Desired State
`EvaluateQuotation` should handle sequencing, mutations, and unit-returning expressions without throwing.

### Reproducible Test Code
```fsharp
#r "nuget: FSharp.Core"
open Microsoft.FSharp.Linq.RuntimeHelpers

let eval = LeafExpressionConverter.EvaluateQuotation

// Test 1: Sequencing - should not throw
let test1 () =
    try
        eval <@ 1; 2 @> |> ignore
        printfn "PASS: Sequencing works"
    with ex ->
        printfn "FAIL: Sequencing throws %s" ex.Message

// Test 2: Variable mutation - should not throw  
let test2 () =
    try
        let mutable x = 1
        eval <@ x <- 2 @> |> ignore
        printfn "PASS: Variable mutation works"
    with ex ->
        printfn "FAIL: Variable mutation throws %s" ex.Message

// Test 3: Void method call - should not throw
let test3 () =
    try
        eval <@ System.Console.WriteLine("test") @> |> ignore
        printfn "PASS: Void method call works"
    with ex ->
        printfn "FAIL: Void method call throws %s" ex.Message

test1()
test2()
test3()
```

### TDD Test Cases
```fsharp
module LeafExpressionConverterTests =
    open Microsoft.FSharp.Linq.RuntimeHelpers
    open Xunit

    [<Fact>]
    let ``EvaluateQuotation handles sequencing`` () =
        let result = LeafExpressionConverter.EvaluateQuotation <@ 1; 2 @>
        Assert.Equal(2, result :?> int)

    [<Fact>]
    let ``EvaluateQuotation handles void method call`` () =
        // Should not throw ArgumentException about System.Void
        LeafExpressionConverter.EvaluateQuotation <@ ignore () @> |> ignore

    [<Fact>]
    let ``EvaluateQuotation handles property set`` () =
        let mutable x = { Name = "test" }
        // Should handle property mutation without NotSupportedException
        LeafExpressionConverter.EvaluateQuotation <@ x.Name <- "updated" @> |> ignore
```

---

## Issue 2: Array indexer maps to GetArray instead of get_Item
**GitHub:** https://github.com/dotnet/fsharp/issues/16918

### Problem Description
When using array indexing within LINQ expressions (e.g., `x.u[0].c`), F# generates `GetArray(d.u, 0)` instead of `d.u.get_Item(0)`. This causes LINQ providers like Azure Cosmos DB to fail with "Method 'GetArray' is not supported."

C# generates `get_Item` calls which work correctly with LINQ providers.

### Desired State
Array indexing in LINQ expressions should generate `get_Item(index)` calls instead of `GetArray(array, index)` to match C# behavior and work with standard LINQ providers.

### Reproducible Test Code
```fsharp
open System
open System.Linq

type Unfold = { c: string }
type Doc = { p: string; u: Unfold[] }

let data = [| { p = "test"; u = [| { c = "value" } |] } |]
let queryable = data.AsQueryable()

// This generates GetArray instead of get_Item
let expr = 
    queryable
        .Select(fun x -> x.u[0].c)
        .Expression

printfn "Expression: %O" expr
// Expected: Contains "get_Item(0)" 
// Actual: Contains "GetArray(x.u, 0)"

// Verify the expression tree uses get_Item
let exprStr = expr.ToString()
if exprStr.Contains("GetArray") then
    printfn "FAIL: Expression uses GetArray instead of get_Item"
elif exprStr.Contains("get_Item") then
    printfn "PASS: Expression uses get_Item"
else
    printfn "UNKNOWN: %s" exprStr
```

### TDD Test Cases
```fsharp
module ArrayIndexerTests =
    open System.Linq
    open Xunit

    type Inner = { Value: string }
    type Outer = { Items: Inner[] }

    [<Fact>]
    let ``Array indexer generates get_Item in expression tree`` () =
        let data = [| { Items = [| { Value = "test" } |] } |]
        let q = data.AsQueryable().Select(fun x -> x.Items[0].Value)
        let expr = q.Expression.ToString()
        Assert.DoesNotContain("GetArray", expr)
        // Should use get_Item or Item property access
        Assert.True(expr.Contains("get_Item") || expr.Contains("[0]"))

    [<Fact>]
    let ``Query expression array indexer generates get_Item`` () =
        let data = [| { Items = [| { Value = "test" } |] } |]
        let q = query {
            for x in data.AsQueryable() do
            select x.Items[0].Value
        }
        let expr = q.Expression.ToString()
        Assert.DoesNotContain("GetArray", expr)
```

---

## Issue 3: Inconsistent anonymous record instantiation with Delegate.Invoke
**GitHub:** https://github.com/dotnet/fsharp/issues/15648

### Problem Description
When projecting anonymous records on `IQueryable`, the generated expression tree inconsistently contains `Delegate.Invoke` calls depending on:
1. Whether field names are in alphabetical order
2. The length of field names
3. Nesting depth of member access

This causes Entity Framework and other LINQ providers to fail parsing the expression.

**Key insight from comments:** The behavior depends on whether fields are in alphabetical order - non-alphabetical order triggers the `Invoke` pattern.

**Note:** Likely duplicate of Issue #11131, solvable together.

### Desired State
Anonymous record instantiation should produce consistent expression trees regardless of field name order or length.

### Reproducible Test Code
```fsharp
open System.Linq

type Person = { Name: string; Id: int }
type Wrapper = { Person: Person }

let data = [
    { Person = { Name = "One"; Id = 1 } }
    { Person = { Name = "Two"; Id = 2 } }
]

// Non-alphabetical order (Name before Id) - generates Invoke
let queryWithInvoke = 
    data.AsQueryable()
        .Select(fun x -> {| Other = {| Name = x.Person.Name; Id = x.Person.Id |} |})

// Alphabetical order (Id before Name) - no Invoke  
let queryWithoutInvoke = 
    data.AsQueryable()
        .Select(fun x -> {| Other = {| Id = x.Person.Id; Name = x.Person.Name |} |})

printfn "With Invoke (non-alpha): %A" queryWithInvoke.Expression
printfn "Without Invoke (alpha): %A" queryWithoutInvoke.Expression

// Check for Invoke in expression
let hasInvoke = queryWithInvoke.Expression.ToString().Contains("Invoke")
if hasInvoke then
    printfn "FAIL: Non-alphabetical order generates Invoke"
else
    printfn "PASS: No Invoke generated"
```

### TDD Test Cases
```fsharp
module AnonymousRecordConsistencyTests =
    open System.Linq
    open Xunit

    type Person = { Name: string; Id: int }
    type Wrapper = { Person: Person }

    [<Fact>]
    let ``Anonymous record with non-alphabetical fields should not generate Invoke`` () =
        let data = [| { Person = { Name = "Test"; Id = 1 } } |]
        let q = data.AsQueryable()
                    .Select(fun x -> {| Name = x.Person.Name; Id = x.Person.Id |})
        let expr = q.Expression.ToString()
        Assert.DoesNotContain("Invoke", expr)

    [<Fact>]
    let ``Nested anonymous records should not generate Invoke`` () =
        let data = [| { Person = { Name = "Test"; Id = 1 } } |]
        let q = data.AsQueryable()
                    .Select(fun x -> {| Other = {| Name = x.Person.Name; Id = x.Person.Id |} |})
        let expr = q.Expression.ToString()
        Assert.DoesNotContain("Invoke", expr)

    [<Fact>]
    let ``Field order should not affect expression tree structure`` () =
        let data = [| { Person = { Name = "Test"; Id = 1 } } |]
        let q1 = data.AsQueryable().Select(fun x -> {| A = x.Person.Name; B = x.Person.Id |})
        let q2 = data.AsQueryable().Select(fun x -> {| B = x.Person.Id; A = x.Person.Name |})
        // Both should have same structure (ignoring field order)
        let hasInvoke1 = q1.Expression.ToString().Contains("Invoke")
        let hasInvoke2 = q2.Expression.ToString().Contains("Invoke")
        Assert.Equal(hasInvoke1, hasInvoke2)
```

---

## Issue 4: Query selecting multiple values cannot be reused for SQL generation
**GitHub:** https://github.com/dotnet/fsharp/issues/15133

### Problem Description
When a query selects multiple values (tuple/record projection), the query cannot be reused to generate additional SQL transformations. Works in C#, fails in F#.

The issue is that F# tuple projections wrap results in a way that breaks IQueryable composition.

### Desired State
Queries with multi-value selections should be composable and translatable to SQL when additional operations are added.

### Reproducible Test Code
```fsharp
#r "nuget: Microsoft.EntityFrameworkCore.InMemory, 8.0.0"

open System
open System.Linq
open Microsoft.EntityFrameworkCore

[<CLIMutable>]
type Product = { Id: int; Name: string; Price: decimal }

type TestContext() =
    inherit DbContext()
    [<DefaultValue>] val mutable products: DbSet<Product>
    member this.Products with get() = this.products and set v = this.products <- v
    override this.OnConfiguring(options) =
        options.UseInMemoryDatabase("TestDb") |> ignore

let ctx = new TestContext()

// Base query selecting multiple values
let baseQuery = query {
    for p in ctx.Products do
    select (p.Id, p.Name)
}

// Try to add more operations - this fails in F#
try
    let extended = baseQuery.Where(fun (id, name) -> id > 0)
    let sql = extended.ToQueryString() // or execute
    printfn "PASS: Extended query works"
with ex ->
    printfn "FAIL: Cannot extend query - %s" ex.Message
```

### TDD Test Cases
```fsharp
module QueryCompositionTests =
    open System.Linq
    open Xunit

    type Item = { Id: int; Name: string }

    [<Fact>]
    let ``Query with tuple select should be composable`` () =
        let data = [| { Id = 1; Name = "A" }; { Id = 2; Name = "B" } |]
        let baseQuery = query {
            for x in data.AsQueryable() do
            select (x.Id, x.Name)
        }
        // Should be able to add Where after tuple select
        let extended = baseQuery.Where(fun (id, _) -> id > 0)
        let results = extended.ToList()
        Assert.Equal(2, results.Count)

    [<Fact>]
    let ``Query with record select should be composable`` () =
        let data = [| { Id = 1; Name = "A" } |]
        let baseQuery = query {
            for x in data.AsQueryable() do
            select {| Id = x.Id; Name = x.Name |}
        }
        let extended = baseQuery.Where(fun r -> r.Id > 0)
        let results = extended.ToList()
        Assert.Single(results) |> ignore
```

---

## Issue 5: Different property order in LINQ select cannot be translated
**GitHub:** https://github.com/dotnet/fsharp/issues/11131

### Problem Description
F# LINQ expressions fail to translate when the order of property names in anonymous or regular records doesn't match the alphabetical order. This is a **critical bug** affecting Entity Framework Core usage.

Error: `The LINQ expression 'LastName' could not be translated`

**Root cause:** Anonymous record field ordering affects expression tree structure, and EF Core's expression visitor fails when fields aren't in expected order.

**Workaround:** Always use alphabetical field ordering.

### Desired State
Property/field order in select projections should not affect LINQ translation.

### Reproducible Test Code
```fsharp
#r "nuget: Microsoft.EntityFrameworkCore.InMemory, 8.0.0"

open System
open System.Linq
open Microsoft.EntityFrameworkCore

[<CLIMutable>]
type Person = { Id: int; LastName: string }

type TestContext() =
    inherit DbContext()
    [<DefaultValue>] val mutable persons: DbSet<Person>
    member this.Persons with get() = this.persons and set v = this.persons <- v
    override this.OnConfiguring(options) =
        options.UseInMemoryDatabase(Guid.NewGuid().ToString()) |> ignore

let ctx = new TestContext()

// Works: Alphabetical order (Id before LastName)
let successQuery = query {
    for p in ctx.Persons do
    select {| ID = p.Id; LastName = p.LastName |}
}

// Fails: Non-alphabetical order (LastName before ID)
let failQuery = query {
    for p in ctx.Persons do
    select {| LastName = p.LastName; ID = p.Id |}
}

try
    failQuery |> Seq.toList |> ignore
    printfn "PASS: Non-alphabetical order works"
with ex ->
    printfn "FAIL: %s" ex.Message
```

### TDD Test Cases
```fsharp
module PropertyOrderTests =
    open System.Linq
    open Xunit

    type Person = { Id: int; LastName: string }

    [<Fact>]
    let ``Anonymous record with non-alphabetical order should work`` () =
        let data = [| { Id = 1; LastName = "Smith" } |]
        let q = data.AsQueryable().Select(fun p -> 
            {| LastName = p.LastName; ID = p.Id |})
        // Should not throw
        let results = q.ToList()
        Assert.Single(results) |> ignore

    [<Fact>]
    let ``Record with non-alphabetical initialization should work`` () =
        let data = [| { Id = 1; LastName = "Smith" } |]
        // Regular record with non-alphabetical field init
        let q = query {
            for p in data.AsQueryable() do
            select { Id = p.Id; LastName = p.LastName }
        }
        let results = q |> Seq.toList
        Assert.Single(results) |> ignore

    [<Fact>]
    let ``Both field orders should produce equivalent expression trees`` () =
        let data = [| { Id = 1; LastName = "Smith" } |]
        let q1 = data.AsQueryable().Select(fun p -> {| A = p.Id; B = p.LastName |})
        let q2 = data.AsQueryable().Select(fun p -> {| B = p.LastName; A = p.Id |})
        // Both should be valid IQueryable (not EnumerableQuery fallback)
        Assert.IsAssignableFrom<IQueryable<_>>(q1) |> ignore
        Assert.IsAssignableFrom<IQueryable<_>>(q2) |> ignore
```

---

## Issue 6: Query expression join on tuple doesn't work
**GitHub:** https://github.com/dotnet/fsharp/issues/7885

### Problem Description
Joining on a composite key tuple within a query expression returns no matches when tuples are created inline, but works when tuples are created via a function.

```fsharp
// Returns nothing (bug)
join t2 in queryable2s on ((t1.Id1, t1.Id2) = (t2.Id1, t2.Id2))

// Works correctly
join t2 in queryable2s on (Test1.Composite t1 = Test2.Composite t2)
```

### Desired State
Inline tuple creation in join conditions should work identically to function-based tuple creation.

### Reproducible Test Code
```fsharp
open System.Linq

type Test1 = { Id1: int; Id2: int; Key: string }
type Test2 = { Id1: int; Id2: int; Value: string }

let data1 = [| { Id1 = 1; Id2 = 2; Key = "A" } |]
let data2 = [| { Id1 = 1; Id2 = 2; Value = "B" } |]

let queryable1s = data1.AsQueryable()
let queryable2s = data2.AsQueryable()

// Bug: Inline tuple join returns nothing
let inlineResult = query {
    for t1 in queryable1s do
    join t2 in queryable2s on ((t1.Id1, t1.Id2) = (t2.Id1, t2.Id2))
    select (t1.Key, t2.Value)
} |> Seq.toList

printfn "Inline tuple join results: %d" inlineResult.Length
if inlineResult.Length = 0 then
    printfn "FAIL: Inline tuple join returned no results"
else
    printfn "PASS: Inline tuple join works"

// Workaround: Function-based tuple
let composite1 t = (t.Id1, t.Id2)
let composite2 (t: Test2) = (t.Id1, t.Id2)

let funcResult = query {
    for t1 in queryable1s do
    join t2 in queryable2s on (composite1 t1 = composite2 t2)
    select (t1.Key, t2.Value)
} |> Seq.toList

printfn "Function tuple join results: %d" funcResult.Length
```

### TDD Test Cases
```fsharp
module TupleJoinTests =
    open System.Linq
    open Xunit

    type Left = { Id1: int; Id2: int; Name: string }
    type Right = { Id1: int; Id2: int; Value: int }

    [<Fact>]
    let ``Join on inline tuple should return matches`` () =
        let left = [| { Id1 = 1; Id2 = 2; Name = "A" } |]
        let right = [| { Id1 = 1; Id2 = 2; Value = 100 } |]
        
        let result = query {
            for l in left.AsQueryable() do
            join r in right.AsQueryable() on ((l.Id1, l.Id2) = (r.Id1, r.Id2))
            select (l.Name, r.Value)
        } |> Seq.toList
        
        Assert.Single(result) |> ignore
        Assert.Equal(("A", 100), result.[0])

    [<Fact>]
    let ``Join on inline tuple should match function-based tuple`` () =
        let left = [| { Id1 = 1; Id2 = 2; Name = "A" } |]
        let right = [| { Id1 = 1; Id2 = 2; Value = 100 }; { Id1 = 3; Id2 = 4; Value = 200 } |]
        
        let inlineResult = query {
            for l in left.AsQueryable() do
            join r in right.AsQueryable() on ((l.Id1, l.Id2) = (r.Id1, r.Id2))
            select r.Value
        } |> Seq.toList
        
        let funcKey (x: Left) = (x.Id1, x.Id2)
        let funcKey2 (x: Right) = (x.Id1, x.Id2)
        let funcResult = query {
            for l in left.AsQueryable() do
            join r in right.AsQueryable() on (funcKey l = funcKey2 r)
            select r.Value
        } |> Seq.toList
        
        Assert.Equal(funcResult, inlineResult)
```

---

## Issue 7: headOrDefault/exactlyOneOrDefault with struct/tuple causes NullReferenceException
**GitHub:** https://github.com/dotnet/fsharp/issues/3845

### Problem Description
When using `headOrDefault` or `exactlyOneOrDefault` with a tuple or struct type, and no results are found, the code throws `NullReferenceException` at runtime when trying to deconstruct the null tuple.

**Root cause:** `Enumerable.FirstOrDefault` returns `null` for reference types (including F# tuples which are `System.Tuple`), but F# allows direct deconstruction without null check.

**dsyme's insight:** The underlying issue is that `headOrDefault` doesn't constrain the type to admit null, allowing this unsound situation.

### Desired State
Either:
1. Add `'T : null` constraint to `headOrDefault`/`exactlyOneOrDefault` (breaking change)
2. Return `ValueOption<'T>` or `Option<'T>` instead (breaking change)
3. At minimum, produce a better error message

### Reproducible Test Code
```fsharp
open System
open System.Linq

let myQueryable = [|(1,1); (2,2)|].AsQueryable()

// This compiles but throws NullReferenceException at runtime
try
    let a, b =
        query {
            for x in myQueryable do
            where(x = (3,3))  // No match
            headOrDefault
        }
    printfn "Result: %d, %d" a b
with
| :? NullReferenceException ->
    printfn "FAIL: NullReferenceException when deconstructing null tuple"
| ex ->
    printfn "Other error: %s" ex.Message

// Workaround: Don't deconstruct immediately
let result =
    query {
        for x in myQueryable do
        where(x = (3,3))
        headOrDefault
    }
    
if isNull (box result) then
    printfn "No result found (null check works)"
else
    let a, b = result
    printfn "Result: %d, %d" a b
```

### TDD Test Cases
```fsharp
module HeadOrDefaultTests =
    open System.Linq
    open Xunit

    [<Fact>]
    let ``headOrDefault with no match should not throw NullReferenceException on tuple`` () =
        let data = [|(1,1); (2,2)|].AsQueryable()
        
        // Should either:
        // 1. Return a valid default (0,0 for value tuples)
        // 2. Throw a meaningful exception
        // 3. Be prevented at compile time
        
        let result = query {
            for x in data do
            where (x = (99,99))  // No match
            headOrDefault
        }
        
        // Current behavior: result is null, which is invalid for tuples
        // At minimum, box result shouldn't crash
        Assert.True(isNull (box result) || result = (0, 0))

    [<Fact>]
    let ``headOrDefault with struct type should return default`` () =
        let data = [|1; 2; 3|].AsQueryable()
        
        let result = query {
            for x in data do
            where (x = 99)  // No match
            headOrDefault
        }
        
        Assert.Equal(0, result)  // int default is 0

    [<Fact>]
    let ``exactlyOneOrDefault with struct type should return default`` () =
        let data = [|1; 2; 3|].AsQueryable()
        
        let result = query {
            for x in data do
            where (x = 99)
            exactlyOneOrDefault
        }
        
        Assert.Equal(0, result)
```

---

## Issue 8: Query builder with EF Core can't return F# tuples using async methods
**GitHub:** https://github.com/dotnet/fsharp/issues/3782

### Problem Description
When using `select (p.Id, p.Name)` (F# tuple) in a query with EF Core, `ToListAsync()` fails with:
> "The source IQueryable doesn't implement IAsyncEnumerable<System.Tuple`2[...]>"

The issue is that F# tuple selections produce `EnumerableQuery` type instead of `EntityQueryable`, breaking async enumeration.

**Additional bug:** Record field order matters - `select { Name = p.Name; Id = p.Id }` works, but `select { Id = p.Id; Name = p.Name }` fails.

**Related to:** Issue #11131, #15648, #47

### Desired State
F# tuple selections should produce proper `EntityQueryable` that supports async operations.

### Reproducible Test Code
```fsharp
#r "nuget: Microsoft.EntityFrameworkCore.InMemory, 8.0.0"

open System
open System.Linq
open Microsoft.EntityFrameworkCore

[<CLIMutable>]
type Product = { Id: string; Name: string }

type TestContext() =
    inherit DbContext()
    [<DefaultValue>] val mutable products: DbSet<Product>
    member this.Products with get() = this.products and set v = this.products <- v
    override this.OnConfiguring(options) =
        options.UseInMemoryDatabase("TestDb") |> ignore

let ctx = new TestContext()

// Single value works
let q1 = query {
    for p in ctx.Products do
    select p.Id
}
printfn "Single value type: %s" (q1.GetType().Name)

// Tuple fails with async
let q2 = query {
    for p in ctx.Products do
    select (p.Id, p.Name)
}
printfn "Tuple type: %s" (q2.GetType().Name)

// Check if it's EnumerableQuery (bad) vs EntityQueryable (good)
if q2.GetType().Name.Contains("EnumerableQuery") then
    printfn "FAIL: Tuple query is EnumerableQuery, not EntityQueryable"
else
    printfn "PASS: Tuple query is EntityQueryable"

// Explicit Tuple constructor works as workaround
let q3 = query {
    for p in ctx.Products do
    select (Tuple<string, string>(p.Id, p.Name))
}
printfn "Explicit Tuple type: %s" (q3.GetType().Name)
```

### TDD Test Cases
```fsharp
module AsyncTupleQueryTests =
    open System.Linq
    open Xunit

    type Item = { Id: string; Name: string }

    [<Fact>]
    let ``Tuple select should preserve IQueryable type`` () =
        let data = [| { Id = "1"; Name = "A" } |]
        let q = query {
            for p in data.AsQueryable() do
            select (p.Id, p.Name)
        }
        // Should not be EnumerableQuery
        Assert.DoesNotContain("EnumerableQuery", q.GetType().Name)

    [<Fact>]
    let ``F# tuple and explicit Tuple should behave identically`` () =
        let data = [| { Id = "1"; Name = "A" } |]
        
        let q1 = query {
            for p in data.AsQueryable() do
            select (p.Id, p.Name)
        }
        
        let q2 = query {
            for p in data.AsQueryable() do
            select (System.Tuple<string, string>(p.Id, p.Name))
        }
        
        Assert.Equal(q1.GetType().Name, q2.GetType().Name)
```

---

## Issue 9: Bug in query translation of conditionals
**GitHub:** https://github.com/dotnet/fsharp/issues/3445

### Problem Description
Using conditionals (`if-then`) in query expressions without an `else` branch fails with:
> "Type mismatch when building 'cond': types of true and false branches differ"

The query translator doesn't properly handle conditional expressions that only have a `then` branch.

### Desired State
Conditional expressions in queries should work with or without else branches.

### Reproducible Test Code
```fsharp
open System.Linq

// This fails with type mismatch error
try
    let result = query { 
        if 1 = 2 then 
            select 3 
    }
    printfn "Result: %A" (result |> Seq.toList)
with ex ->
    printfn "FAIL: %s" ex.Message

// Workaround: Add explicit else
let result2 = query {
    if 1 = 2 then
        select 3
    else
        select 0  // or yield! Seq.empty
}
printfn "With else: %A" (result2 |> Seq.toList)
```

### TDD Test Cases
```fsharp
module ConditionalQueryTests =
    open System.Linq
    open Xunit

    [<Fact>]
    let ``Query with if-then-no-else should work`` () =
        // Should not throw ArgumentException
        let result = query { 
            if 1 = 2 then 
                select 3 
        } |> Seq.toList
        Assert.Empty(result)

    [<Fact>]
    let ``Query with if-then-else should work`` () =
        let result = query {
            if 1 = 1 then
                select 3
            else
                select 0
        } |> Seq.toList
        Assert.Single(result) |> ignore
        Assert.Equal(3, result.[0])

    [<Fact>]
    let ``Query with conditional yield should work`` () =
        let data = [1; 2; 3].AsQueryable()
        let result = query {
            for x in data do
            if x > 1 then
                select x
        } |> Seq.toList
        Assert.Equal([2; 3], result)
```

---

## Issue 10: Query expressions and --warnon:1182 (unused variable warning)
**GitHub:** https://github.com/dotnet/fsharp/issues/422

### Problem Description
In query expressions, variables bound using `for` are incorrectly flagged as unused (FS1182) when they are used in `where` or `let` but not in `select`.

```fsharp
// Warning FS1182: The value 'x' is unused (but x IS used in where!)
query { for x in [1;2;3] do
        where (x > 2)
        select 1 }
```

**Root cause:** The computation expression desugaring creates duplicate copies of `x`, and the "used" tracking doesn't propagate correctly.

**Workaround:** Prefix with underscore: `for _x in ...`

### Desired State
Variables used anywhere in the query body (where, let, orderBy, etc.) should not trigger FS1182.

### Reproducible Test Code
```fsharp
// Compile with: fsc --warnon:1182 test.fs
// Or: dotnet build /p:OtherFlags="--warnon:1182"

open System.Linq

// Should NOT warn - x is used in where
let q1 = query { 
    for x in [1;2;3] do
    where (x > 2)
    select 1 
}

// Should NOT warn - x is used in let
let q2 = query { 
    for x in [1;2;3] do
    let y = x
    select y 
}

// Should NOT warn - both x and y are used
let q3 = query {
    for x in [1;2;3].AsQueryable() do
    join y in [2;3;4].AsQueryable() on (x = y)
    where (x > 1)
    select y
}

printfn "q1: %A" (q1 |> Seq.toList)
printfn "q2: %A" (q2 |> Seq.toList)
printfn "q3: %A" (q3 |> Seq.toList)
```

### TDD Test Cases
```fsharp
module UnusedVariableWarningTests =
    // These tests verify the compiler doesn't emit FS1182 warnings
    // Test by checking compilation output/diagnostics
    
    open FSharp.Compiler.Diagnostics
    open Xunit

    let compileAndCheckWarnings code =
        // Use FCS to compile and check for FS1182 warnings
        // Implementation depends on test infrastructure
        ()

    [<Fact>]
    let ``Variable used in where should not trigger FS1182`` () =
        let code = """
            query { for x in [1;2;3] do
                    where (x > 2)
                    select 1 }
        """
        // Should not contain FS1182 warning for 'x'
        // compileAndCheckWarnings code
        Assert.True(true) // Placeholder

    [<Fact>]
    let ``Variable used in let binding should not trigger FS1182`` () =
        let code = """
            query { for x in [1;2;3] do
                    let y = x
                    select y }
        """
        // Should not contain FS1182 warning for 'x'
        Assert.True(true) // Placeholder

    [<Fact>]
    let ``Join variable used elsewhere should not trigger FS1182`` () =
        let code = """
            query { for x in db1 do
                    join y in db2 on (x.Id = y.Id)
                    where (y.Value > 0)
                    select x }
        """
        // Should not contain FS1182 warning for 'y'
        Assert.True(true) // Placeholder
```

---

## Issue 11: Tuples in query expression with grouping cannot be translated to SQL
**GitHub:** https://github.com/dotnet/fsharp/issues/47

### Problem Description
Using tuples within a `groupBy` expression followed by operations on group elements fails with:
> "Member access 'Item1' of 'System.Tuple2[...]' not legal on type 'Grouping2[...]'"

The translator incorrectly attempts to access tuple properties (`Item1`) as if they were properties of the grouping itself.

**Workaround:** Use `AnonymousObject<_,_>` instead of tuples for composite keys.

### Desired State
Tuple access within groupBy expressions should correctly reference elements within the group, not the grouping itself.

### Reproducible Test Code
```fsharp
// Note: Requires actual database connection to fully reproduce
// This demonstrates the pattern that fails

open System
open System.Linq
open Microsoft.FSharp.Linq.RuntimeHelpers

type Product = { ProductID: int; ProductName: string; CategoryID: int }
type Category = { CategoryID: int; CategoryName: string }

let products = [
    { ProductID = 1; ProductName = "Apple"; CategoryID = 1 }
    { ProductID = 2; ProductName = "Banana"; CategoryID = 1 }
].AsQueryable()

let categories = [
    { CategoryID = 1; CategoryName = "Fruit" }
].AsQueryable()

// This pattern fails with real LINQ-to-SQL providers
let failingQuery = query {
    for p in products do
    join c in categories on (p.CategoryID = c.CategoryID)
    groupBy p.CategoryID into g
    select (g.Key, g.Select(fun (p, c) -> 
        if p.ProductName = c.CategoryName then 1.0 else 0.0).Average())
}

// Workaround using AnonymousObject
let workingQuery = query {
    for p in products do
    join c in categories on (p.CategoryID = c.CategoryID)
    let key = Linq.RuntimeHelpers.AnonymousObject<_,_>(p.CategoryID, c.CategoryName)
    groupValBy p.ProductName key into g
    select (g.Key.Item1, g.Count())
}

printfn "Working query result: %A" (workingQuery |> Seq.toList)
```

### TDD Test Cases
```fsharp
module GroupByTupleTests =
    open System.Linq
    open Xunit

    type Item = { Id: int; Category: int; Name: string }

    [<Fact>]
    let ``GroupBy with tuple key should work`` () =
        let data = [
            { Id = 1; Category = 1; Name = "A" }
            { Id = 2; Category = 1; Name = "B" }
            { Id = 3; Category = 2; Name = "C" }
        ].AsQueryable()
        
        let result = query {
            for x in data do
            groupBy (x.Category, x.Name.[0]) into g
            select (g.Key, g.Count())
        } |> Seq.toList
        
        Assert.Equal(3, result.Length)

    [<Fact>]
    let ``GroupBy with tuple access on group elements should work`` () =
        let items = [| { Id = 1; Category = 1; Name = "A" } |]
        let cats = [| (1, "Cat1") |]
        
        let result = query {
            for i in items.AsQueryable() do
            join c in cats.AsQueryable() on (i.Category = fst c)
            groupBy i.Category into g
            select (g.Key, g.Select(fun (item, cat) -> snd cat).First())
        } |> Seq.toList
        
        Assert.Single(result) |> ignore

    [<Fact>]
    let ``Joined tuple elements should be accessible after groupBy`` () =
        let left = [| (1, "A"); (1, "B"); (2, "C") |].AsQueryable()
        let right = [| (1, 100); (2, 200) |].AsQueryable()
        
        let result = query {
            for l in left do
            join r in right on (fst l = fst r)
            groupBy (fst l) into g
            select (g.Key, g.Select(fun (l, r) -> snd r).Sum())
        } |> Seq.toList
        
        // Should have 2 groups: key 1 (sum 200), key 2 (sum 200)
        Assert.Equal(2, result.Length)
```

---

## Implementation Priority

### High Priority (Breaking EF Core / LINQ Providers)
1. **Issue #11131** - Property order breaks translation (most impactful)
2. **Issue #15648** - Delegate.Invoke in anonymous records (duplicate of #11131)
3. **Issue #3782** - Tuple select breaks async (EF Core)
4. **Issue #16918** - Array indexer uses GetArray

### Medium Priority (Functional Bugs)
5. **Issue #7885** - Tuple join returns no results
6. **Issue #47** - GroupBy tuple translation
7. **Issue #3845** - headOrDefault null handling
8. **Issue #15133** - Query composition

### Lower Priority (Ergonomics)
9. **Issue #19099** - EvaluateQuotation edge cases
10. **Issue #3445** - Conditional translation
11. **Issue #422** - Unused variable warnings

---

## Key Files to Modify

| File | Issues Addressed |
|------|-----------------|
| `src/FSharp.Core/Query.fs` | #11131, #15648, #3782, #7885, #47, #3445 |
| `src/FSharp.Core/Linq.fs` | #19099, #16918, #11131 |
| `src/FSharp.Core/QueryExtensions.fs` | #11131, #15648 |
| `src/Compiler/Checking/Expressions/CheckComputationExpressions.fs` | #422, #3445 |

---

## Testing Strategy

All tests should:
1. Use in-memory data via `AsQueryable()` - no external database needed
2. Be added to `tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/`
3. Follow existing patterns in `tests/fsharp/core/queriesOverIQueryable/test.fsx`
4. Verify both the expression tree structure AND the execution results
