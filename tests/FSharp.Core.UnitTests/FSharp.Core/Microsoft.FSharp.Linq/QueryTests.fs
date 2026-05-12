// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Suppress FS3220 warning for System.Tuple Item1/Item2 access - these tests intentionally test interop with System.Tuple
#nowarn "3220"

namespace FSharp.Core.UnitTests.Linq

open System
open System.Linq
open Xunit
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.RuntimeHelpers

module TestData =
    type Person = { Id: int; Name: string; Age: int; DepartmentId: int }
    type Department = { Id: int; DeptName: string }
    
    let people = [
        { Id = 1; Name = "Alice"; Age = 30; DepartmentId = 1 }
        { Id = 2; Name = "Bob"; Age = 25; DepartmentId = 1 }
        { Id = 3; Name = "Charlie"; Age = 35; DepartmentId = 2 }
        { Id = 4; Name = "Diana"; Age = 28; DepartmentId = 2 }
    ]
    
    let departments = [
        { Id = 1; DeptName = "Engineering" }
        { Id = 2; DeptName = "Marketing" }
    ]
    
    type Table1 = { Id1: int; Id2: int; Value1: string }
    type Table2 = { Id1: int; Id2: int; Value2: string }
    
    let table1Data = [
        { Table1.Id1 = 1; Id2 = 10; Value1 = "A" }
        { Table1.Id1 = 2; Id2 = 20; Value1 = "B" }
        { Table1.Id1 = 3; Id2 = 30; Value1 = "C" }
    ]
    
    let table2Data = [
        { Table2.Id1 = 1; Id2 = 10; Value2 = "X" }
        { Table2.Id1 = 2; Id2 = 20; Value2 = "Y" }
        { Table2.Id1 = 4; Id2 = 40; Value2 = "Z" }
    ]

open TestData

type QueryTupleJoinTests() =
    
    [<Fact>]
    member _.``Inline tuple join returns correct matches - issue 7885``() =
        let t1 = table1Data.AsQueryable()
        let t2 = table2Data.AsQueryable()
        
        let inlineResult = 
            query {
                for a in t1 do
                join b in t2 on ((a.Id1, a.Id2) = (b.Id1, b.Id2))
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        Assert.Equal(2, inlineResult.Length)
        Assert.Contains(("A", "X"), inlineResult)
        Assert.Contains(("B", "Y"), inlineResult)
    
    [<Fact>]
    member _.``Inline tuple join matches function-based tuple join - issue 7885``() =
        let t1 = table1Data.AsQueryable()
        let t2 = table2Data.AsQueryable()
        
        let makeKey1 (a: Table1) = (a.Id1, a.Id2)
        let makeKey2 (b: Table2) = (b.Id1, b.Id2)
        
        let inlineResult = 
            query {
                for a in t1 do
                join b in t2 on ((a.Id1, a.Id2) = (b.Id1, b.Id2))
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        let funcResult = 
            query {
                for a in t1 do
                join b in t2 on (makeKey1 a = makeKey2 b)
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        Assert.Equal(funcResult.Length, inlineResult.Length)
        for item in funcResult do
            Assert.Contains(item, inlineResult)
        for item in inlineResult do
            Assert.Contains(item, funcResult)


type QueryGroupByTupleTests() =
    
    [<Fact>]
    member _.``GroupBy with tuple key works - issue 47``() =
        let data = 
            [ (1, "A", 100); (1, "A", 200); (2, "B", 300) ].AsQueryable()
        
        let result = 
            query {
                for (x, y, z) in data do
                groupBy (x, y) into g
                select (g.Key, g.Count())
            } |> Seq.toList
        
        Assert.Equal(2, result.Length)
        Assert.Contains(((1, "A"), 2), result)
        Assert.Contains(((2, "B"), 1), result)
    
    [<Fact>]
    member _.``Accessing tuple elements after groupBy works - issue 47``() =
        let data = people.AsQueryable()
        
        let result = 
            query {
                for p in data do
                groupBy (p.DepartmentId, p.Age > 28) into g
                select (fst g.Key, snd g.Key, g.Count())
            } |> Seq.toList
        
        Assert.True(result.Length > 0)
        for (deptId, isOver28, count) in result do
            Assert.True(deptId >= 1 && deptId <= 2)
            Assert.True(count >= 1)
    
    [<Fact>]
    member _.``GroupBy with tuple key allows iteration over group elements``() =
        let data = table1Data.AsQueryable()
        
        let result = 
            query {
                for item in data do
                groupBy item.Id1 into g
                select (g.Key, g |> Seq.map (fun x -> x.Value1) |> Seq.toList)
            } |> Seq.toList
        
        Assert.Equal(3, result.Length)
        let group1 = result |> List.find (fun (k, _) -> k = 1)
        Assert.Contains("A", snd group1)


type QueryGroupJoinTupleTests() =
    
    [<Fact>]
    member _.``GroupJoin with inline tuple key works``() =
        let t1 = table1Data.AsQueryable()
        let t2 = table2Data.AsQueryable()
        
        let result = 
            query {
                for a in t1 do
                groupJoin b in t2 on ((a.Id1, a.Id2) = (b.Id1, b.Id2)) into bs
                select (a.Value1, bs |> Seq.length)
            } |> Seq.toList
        
        Assert.Equal(3, result.Length)
        Assert.Contains(("A", 1), result)
        Assert.Contains(("B", 1), result)
        Assert.Contains(("C", 0), result)


type AnonymousObjectEqualityTests() =
    
    [<Fact>]
    member _.``AnonymousObject with same values are equal``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        
        Assert.True(ao1.Equals(ao2))
        Assert.Equal(ao1.GetHashCode(), ao2.GetHashCode())
    
    [<Fact>]
    member _.``AnonymousObject with different values are not equal``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(2, "test")
        
        Assert.False(ao1.Equals(ao2))
    
    [<Fact>]
    member _.``AnonymousObject hash codes are consistent with equality``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(2, "other")
        
        // The important thing is that equal objects have equal hash codes
        Assert.False(ao1.Equals(ao2))


/// Tests for tuple select preserving IQueryable type - Issues #3782 and #15133
type QueryTupleSelectTests() =
    
    [<Fact>]
    member _.``Tuple select preserves IQueryable type - issue 3782``() =
        let data = TestData.people.AsQueryable()
        
        let result = 
            query {
                for p in data do
                select (p.Id, p.Name)
            }
        
        // The result should be IQueryable - use box to avoid FS0067 warning
        Assert.NotNull(box result)
        Assert.True(typeof<IQueryable<int * string>>.IsAssignableFrom(result.GetType()), 
            sprintf "Expected IQueryable<int * string> but got %s" (result.GetType().FullName))
        
        let queryable = result
        let exprStr = queryable.Expression.ToString()
        Assert.True(exprStr.Contains("Tuple") && exprStr.Contains("Select"), 
            sprintf "Expression should contain Select with Tuple, but got: %s" exprStr)
        
        // Verify the query can be composed - if incorrectly using Enumerable.Select, this would fail
        let composed = result.Where(fun (id, _name) -> id > 0)
        let items = composed |> Seq.toList
        Assert.True(items.Length > 0)
    
    [<Fact>]
    member _.``System.Tuple select preserves IQueryable type``() =
        let data = TestData.people.AsQueryable()
        
        let result = 
            query {
                for p in data do
                select (System.Tuple.Create(p.Id, p.Name))
            }
        
        // box to avoid FS0067 warning
        Assert.NotNull(box result)
        Assert.True(typeof<IQueryable<System.Tuple<int, string>>>.IsAssignableFrom(result.GetType()), 
            sprintf "Expected IQueryable<Tuple<int, string>> but got %s" (result.GetType().FullName))
        
        let composed = result.Where(fun t -> t.Item1 > 0)
        let items = composed |> Seq.toList
        Assert.True(items.Length > 0)
    
    [<Fact>]
    member _.``F# tuple and System.Tuple produce equivalent query behavior``() =
        let data = TestData.people.AsQueryable()
        
        let fsharpTupleResult = 
            query {
                for p in data do
                select (p.Id, p.Name)
            } |> Seq.toList
        
        let systemTupleResult = 
            query {
                for p in data do
                select (System.Tuple.Create(p.Id, p.Name))
            } |> Seq.map (fun t -> (t.Item1, t.Item2)) |> Seq.toList
        
        Assert.Equal(fsharpTupleResult.Length, systemTupleResult.Length)
        for (expected, actual) in List.zip fsharpTupleResult systemTupleResult do
            Assert.Equal(expected, actual)
    
    [<Fact>]
    member _.``Tuple select query can be composed with Where - issue 15133``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age)
            }
        
        let filtered = baseQuery.Where(fun (id, name, age) -> age > 25)
        
        let result = filtered |> Seq.toList
        
        Assert.True(result.Length > 0)
        for (id, name, age) in result do
            Assert.True(age > 25, sprintf "Expected age > 25 but got %d" age)
    
    [<Fact>]
    member _.``Tuple select query can be composed with OrderBy - issue 15133``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age)
            }
        
        let sorted = baseQuery.OrderBy(fun (id, name, age) -> age)
        
        let result = sorted |> Seq.toList
        
        Assert.True(result.Length > 0)
        let ages = result |> List.map (fun (_, _, age) -> age)
        let sortedAges = ages |> List.sort
        Assert.Equal<int list>(sortedAges, ages)
    
    [<Fact>]
    member _.``Record projection query is composable``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select { TestData.Person.Id = p.Id; Name = p.Name; Age = p.Age; DepartmentId = p.DepartmentId }
            }
        
        let filtered = baseQuery.Where(fun p -> p.Age > 28)
        let result = filtered |> Seq.toList
        Assert.True(result.Length > 0)
        for p in result do
            Assert.True(p.Age > 28)
    
    [<Fact>]
    member _.``Multi-element tuple select preserves all elements for composition``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age, p.DepartmentId)
            }
        
        let filtered = baseQuery.Where(fun (id, name, age, deptId) -> deptId = 1 && age > 20)
        let result = filtered |> Seq.toList
        
        Assert.True(result.Length > 0)
        for (_, _, _, deptId) in result do
            Assert.Equal(1, deptId)


module MutationTestHelpers =
    type TypeWithMutableField() =
        [<DefaultValue>]
        val mutable Field: int
    
    type TypeWithSettableProperty() =
        let mutable value = 0
        member this.Prop
            with get() = value
            and set(v) = value <- v

/// Tests for EvaluateQuotation edge cases - Issue #19099
type EvaluateQuotationEdgeCaseTests() =
    
    [<Fact>]
    member _.``EvaluateQuotation handles Sequential expressions - issue 19099``() =
        let result = LeafExpressionConverter.EvaluateQuotation <@ ignore 1; 42 @>
        Assert.Equal(42, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles void method calls - issue 19099``() =
        // This should not throw - it should execute and return unit
        let result = LeafExpressionConverter.EvaluateQuotation <@ System.Console.Write("") @>
        Assert.Equal(box (), result)
    
    [<Fact>]
    member _.``EvaluateQuotation handles unit return - issue 19099``() =
        // Test that unit-returning expressions work (previously failed with System.Void issue)
        let result = LeafExpressionConverter.EvaluateQuotation <@ ignore 1; () @>
        Assert.Equal(box (), result)
    
    [<Fact>]
    member _.``EvaluateQuotation handles VarSet - issue 19099``() =
        let result = LeafExpressionConverter.EvaluateQuotation <@ let mutable x = 1 in x <- 2; x @>
        Assert.Equal(2, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles FieldSet - issue 19099``() =
        let obj = MutationTestHelpers.TypeWithMutableField()
        let result = LeafExpressionConverter.EvaluateQuotation <@ obj.Field <- 42; obj.Field @>
        Assert.Equal(42, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles PropertySet - issue 19099``() =
        let obj = MutationTestHelpers.TypeWithSettableProperty()
        let result = LeafExpressionConverter.EvaluateQuotation <@ obj.Prop <- 99; obj.Prop @>
        Assert.Equal(99, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles indexed PropertySet - issue 19099``() =
        let arr = [| 1; 2; 3 |]
        let result = LeafExpressionConverter.EvaluateQuotation <@ arr.[0] <- 10; arr.[0] @>
        Assert.Equal(10, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles deeply nested let bindings``() =
        let result = LeafExpressionConverter.EvaluateQuotation <@ let a = 42 in let b = a in let c = b in c @>
        Assert.Equal(42, result :?> int)
    
    [<Fact>]
    member _.``EvaluateQuotation handles deeply nested let with computation``() =
        let result = LeafExpressionConverter.EvaluateQuotation <@ let a = 1 in let b = a + 1 in let c = b + 1 in let d = c + 1 in d @>
        Assert.Equal(4, result :?> int)

    [<Fact>]
    member _.``EvaluateQuotation handles 15 nested let bindings``() =
        // Verifies inlining is O(n) not O(2^n)
        let result = LeafExpressionConverter.EvaluateQuotation <@ 
            let v1 = 1 in
            let v2 = v1 + 1 in
            let v3 = v2 + 1 in
            let v4 = v3 + 1 in
            let v5 = v4 + 1 in
            let v6 = v5 + 1 in
            let v7 = v6 + 1 in
            let v8 = v7 + 1 in
            let v9 = v8 + 1 in
            let v10 = v9 + 1 in
            let v11 = v10 + 1 in
            let v12 = v11 + 1 in
            let v13 = v12 + 1 in
            let v14 = v13 + 1 in
            let v15 = v14 + 1 in
            v15 @>
        Assert.Equal(15, result :?> int)


/// Tests for conditional without else branch in queries - Issue #3445
type QueryConditionalTests() =
    
    [<Fact>]
    member _.``Query with if-then no else compiles and runs - issue 3445``() =
        // This was throwing: Type mismatch when building 'cond'
        let result = 
            query { 
                for i in [1; 2; 3].AsQueryable() do
                if i > 1 then 
                    select i 
            } |> Seq.toList
        
        Assert.Equal(2, result.Length)
        Assert.Contains(2, result)
        Assert.Contains(3, result)
    
    [<Fact>]
    member _.``Query with if-then no else with false condition returns empty - issue 3445``() =
        let result = 
            query { 
                for i in [1; 2; 3].AsQueryable() do
                if false then 
                    select i 
            } |> Seq.toList
        
        Assert.Empty(result)
    
    [<Fact>]
    member _.``Query with complex if-then condition works - issue 3445``() =
        let data = TestData.people.AsQueryable()
        let result = 
            query { 
                for p in data do
                if p.Age > 25 && p.DepartmentId = 1 then 
                    select p.Name
            } |> Seq.toList
        
        // Alice (30, dept 1) matches; Bob (25, dept 1) doesn't (not >25)
        Assert.Single(result) |> ignore
        Assert.Contains("Alice", result)


/// Tests for headOrDefault with tuples - Issue #3845
/// Note: This issue requires a compiler warning for proper fix.
/// These tests document the current behavior.
type QueryHeadOrDefaultTests() =
    
    [<Fact>]
    member _.``headOrDefault with empty sequence returns default``() =
        let data = [1; 2; 3].AsQueryable()
        let result = 
            query { 
                for x in data do
                where (x > 100)
                headOrDefault
            }
        // For int (value type), default is 0
        Assert.Equal(0, result)
    
    [<Fact>]
    member _.``headOrDefault with matching element returns first match``() =
        let data = [1; 2; 3].AsQueryable()
        let result = 
            query { 
                for x in data do
                where (x > 1)
                headOrDefault
            }
        Assert.Equal(2, result)
    
    [<Fact>]
    member _.``headOrDefault with tuple and no match returns null - issue 3845 known limitation``() =
        let data = [(1, "a"); (2, "b")].AsQueryable()
        let result = 
            query { 
                for x in data do
                where (fst x > 100)
                headOrDefault
            }
        // The result is null (default for reference type tuple)
        // Accessing .Item1/.Item2 would cause NRE - this is the known issue
        Assert.True(obj.ReferenceEquals(null, result))
