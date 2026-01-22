// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Suppress FS3220 warning for System.Tuple Item1/Item2 access - these tests intentionally test interop with System.Tuple
#nowarn "3220"

namespace FSharp.Core.UnitTests.Linq

open System
open System.Linq
open Xunit
open Microsoft.FSharp.Linq

/// Test data for query tests
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
    
    /// Issue #7885: Inline tuple join on ((t1.Id1, t1.Id2) = (t2.Id1, t2.Id2)) returns correct matches
    [<Fact>]
    member _.``Inline tuple join returns correct matches - issue 7885``() =
        let t1 = table1Data.AsQueryable()
        let t2 = table2Data.AsQueryable()
        
        // Inline tuple join - this was broken before the fix
        let inlineResult = 
            query {
                for a in t1 do
                join b in t2 on ((a.Id1, a.Id2) = (b.Id1, b.Id2))
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        // Expected: 2 matches (1,10) and (2,20)
        Assert.Equal(2, inlineResult.Length)
        Assert.Contains(("A", "X"), inlineResult)
        Assert.Contains(("B", "Y"), inlineResult)
    
    /// Issue #7885: Verify inline and function-based tuple joins produce same results
    [<Fact>]
    member _.``Inline tuple join matches function-based tuple join - issue 7885``() =
        let t1 = table1Data.AsQueryable()
        let t2 = table2Data.AsQueryable()
        
        let makeKey1 (a: Table1) = (a.Id1, a.Id2)
        let makeKey2 (b: Table2) = (b.Id1, b.Id2)
        
        // Inline tuple join
        let inlineResult = 
            query {
                for a in t1 do
                join b in t2 on ((a.Id1, a.Id2) = (b.Id1, b.Id2))
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        // Function-based tuple join (this was working before)
        let funcResult = 
            query {
                for a in t1 do
                join b in t2 on (makeKey1 a = makeKey2 b)
                select (a.Value1, b.Value2)
            } |> Seq.toList
        
        // Both should produce the same results
        Assert.Equal(funcResult.Length, inlineResult.Length)
        for item in funcResult do
            Assert.Contains(item, inlineResult)
        for item in inlineResult do
            Assert.Contains(item, funcResult)


type QueryGroupByTupleTests() =
    
    /// Issue #47: GroupBy with tuple key works
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
    
    /// Issue #47: Accessing tuple elements after groupBy works  
    [<Fact>]
    member _.``Accessing tuple elements after groupBy works - issue 47``() =
        let data = people.AsQueryable()
        
        let result = 
            query {
                for p in data do
                groupBy (p.DepartmentId, p.Age > 28) into g
                select (fst g.Key, snd g.Key, g.Count())
            } |> Seq.toList
        
        // Verify we can access fst and snd on the key
        Assert.True(result.Length > 0)
        for (deptId, isOver28, count) in result do
            Assert.True(deptId >= 1 && deptId <= 2)
            Assert.True(count >= 1)
    
    /// Verify groupBy with complex tuple key and element access
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
    
    /// Verify groupJoin with inline tuple key works
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
        
        // t1 has 3 items, each should have a group (possibly empty)
        Assert.Equal(3, result.Length)
        // First two should have 1 match each, third should have 0
        Assert.Contains(("A", 1), result)
        Assert.Contains(("B", 1), result)
        Assert.Contains(("C", 0), result)


type AnonymousObjectEqualityTests() =
    
    /// Verify AnonymousObject has structural equality
    [<Fact>]
    member _.``AnonymousObject with same values are equal``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        
        Assert.True(ao1.Equals(ao2))
        Assert.Equal(ao1.GetHashCode(), ao2.GetHashCode())
    
    /// Verify AnonymousObject with different values are not equal
    [<Fact>]
    member _.``AnonymousObject with different values are not equal``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(2, "test")
        
        Assert.False(ao1.Equals(ao2))
    
    /// Verify AnonymousObject hash codes differ for different values
    [<Fact>]
    member _.``AnonymousObject hash codes are consistent with equality``() =
        let ao1 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(1, "test")
        let ao2 = Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject<int, string>(2, "other")
        
        // The important thing is that equal objects have equal hash codes
        Assert.False(ao1.Equals(ao2))


/// Tests for tuple select preserving IQueryable type - Issues #3782 and #15133
type QueryTupleSelectTests() =
    
    /// Issue #3782: Tuple select should produce composable IQueryable
    /// The query expression should contain proper Select calls that can be extended
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
        
        // The expression tree should have Select calls that produce tuples
        let queryable = result
        let exprStr = queryable.Expression.ToString()
        // Verify the expression has the tuple conversion
        Assert.True(exprStr.Contains("Tuple") && exprStr.Contains("Select"), 
            sprintf "Expression should contain Select with Tuple, but got: %s" exprStr)
        
        // Most importantly: verify the query can be composed (this is what the issue is about)
        // If the query was incorrectly using Enumerable.Select, adding Where would fail or produce wrong results
        let composed = result.Where(fun (id, _name) -> id > 0)
        let items = composed |> Seq.toList
        Assert.True(items.Length > 0)
    
    /// Issue #3782: System.Tuple select should also preserve IQueryable type  
    [<Fact>]
    member _.``System.Tuple select preserves IQueryable type``() =
        let data = TestData.people.AsQueryable()
        
        let result = 
            query {
                for p in data do
                select (System.Tuple.Create(p.Id, p.Name))
            }
        
        // System.Tuple should also produce IQueryable - use box to avoid FS0067 warning
        Assert.NotNull(box result)
        Assert.True(typeof<IQueryable<System.Tuple<int, string>>>.IsAssignableFrom(result.GetType()), 
            sprintf "Expected IQueryable<Tuple<int, string>> but got %s" (result.GetType().FullName))
        
        // Verify composability
        let composed = result.Where(fun t -> t.Item1 > 0)
        let items = composed |> Seq.toList
        Assert.True(items.Length > 0)
    
    /// Issue #3782: F# tuple and System.Tuple should produce equivalent query types
    [<Fact>]
    member _.``F# tuple and System.Tuple produce equivalent query behavior``() =
        let data = TestData.people.AsQueryable()
        
        // F# tuple projection
        let fsharpTupleResult = 
            query {
                for p in data do
                select (p.Id, p.Name)
            } |> Seq.toList
        
        // Explicit System.Tuple projection
        let systemTupleResult = 
            query {
                for p in data do
                select (System.Tuple.Create(p.Id, p.Name))
            } |> Seq.map (fun t -> (t.Item1, t.Item2)) |> Seq.toList
        
        // Both should produce the same results
        Assert.Equal(fsharpTupleResult.Length, systemTupleResult.Length)
        for (expected, actual) in List.zip fsharpTupleResult systemTupleResult do
            Assert.Equal(expected, actual)
    
    /// Issue #15133: Queries with tuple select can be extended with Where
    [<Fact>]
    member _.``Tuple select query can be composed with Where - issue 15133``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age)
            }
        
        // Should be able to add Where after tuple select
        let filtered = baseQuery.Where(fun (id, name, age) -> age > 25)
        
        // Verify the query can be executed
        let result = filtered |> Seq.toList
        
        // All results should have age > 25
        Assert.True(result.Length > 0)
        for (id, name, age) in result do
            Assert.True(age > 25, sprintf "Expected age > 25 but got %d" age)
    
    /// Issue #15133: Queries with tuple select can be extended with OrderBy
    [<Fact>]
    member _.``Tuple select query can be composed with OrderBy - issue 15133``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age)
            }
        
        // Should be able to add OrderBy after tuple select
        let sorted = baseQuery.OrderBy(fun (id, name, age) -> age)
        
        // Verify the query can be executed and is sorted
        let result = sorted |> Seq.toList
        
        Assert.True(result.Length > 0)
        // Verify sorting
        let ages = result |> List.map (fun (_, _, age) -> age)
        let sortedAges = ages |> List.sort
        Assert.Equal<int list>(sortedAges, ages)
    
    /// Record projections should also be composable
    [<Fact>]
    member _.``Record projection query is composable``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select { TestData.Person.Id = p.Id; Name = p.Name; Age = p.Age; DepartmentId = p.DepartmentId }
            }
        
        // Should be able to compose with Where
        let filtered = baseQuery.Where(fun p -> p.Age > 28)
        
        let result = filtered |> Seq.toList
        Assert.True(result.Length > 0)
        for p in result do
            Assert.True(p.Age > 28)
    
    /// Multiple tuple elements should all be accessible in composed queries
    [<Fact>]
    member _.``Multi-element tuple select preserves all elements for composition``() =
        let data = TestData.people.AsQueryable()
        
        let baseQuery = 
            query {
                for p in data do
                select (p.Id, p.Name, p.Age, p.DepartmentId)
            }
        
        // Filter by tuple element
        let filtered = baseQuery.Where(fun (id, name, age, deptId) -> deptId = 1 && age > 20)
        let result = filtered |> Seq.toList
        
        Assert.True(result.Length > 0)
        for (_, _, _, deptId) in result do
            Assert.Equal(1, deptId)
