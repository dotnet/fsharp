// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq

    open Microsoft.FSharp
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    open System
    open System.Linq
    open System.Collections
    open System.Collections.Generic

    /// <summary>
    /// A partial input or result in an F# query. This type is used to support the F# query syntax.
    /// </summary>
    ///
    /// <namespacedoc><summary>
    ///   Library functionality for F# query syntax and interoperability with .NET LINQ Expressions. See
    ///   also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/query-expressions">F# Query Expressions</a> in the F# Language Guide.
    /// </summary></namespacedoc>
    [<NoComparison; NoEquality; Sealed>]
    type QuerySource<'T, 'Q> =
        /// <summary>
        /// A method used to support the F# query syntax.  
        /// </summary>
        new: seq<'T> -> QuerySource<'T,'Q>

        /// <summary>
        /// A property used to support the F# query syntax.  
        /// </summary>
        member Source: seq<'T>

    /// The type used to support the F# query syntax. Use 'query { ... }' to use the query syntax. See
    /// also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/query-expressions">F# Query Expressions</a> in the F# Language Guide.
    [<Class>]
    type QueryBuilder =
        /// <summary>Create an instance of this builder. Use 'query { ... }' to use the query syntax.</summary>
        new: unit -> QueryBuilder
        
        /// <summary>
        /// A method used to support the F# query syntax.  Inputs to queries are implicitly wrapped by a call to one of the overloads of this method.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Source: source:IQueryable<'T> -> QuerySource<'T,'Q> 

        /// <summary>
        /// A method used to support the F# query syntax.  Inputs to queries are implicitly wrapped by a call to one of the overloads of this method.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Source: source:IEnumerable<'T> -> QuerySource<'T,IEnumerable> 

        /// <summary>
        /// A method used to support the F# query syntax.  Projects each element of a sequence to another sequence and combines the resulting sequences into one sequence.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member For: source:QuerySource<'T,'Q> * body:('T -> QuerySource<'Result,'Q2>) -> QuerySource<'Result,'Q>

        /// <summary>
        /// A method used to support the F# query syntax.  Returns an empty sequence that has the specified type argument.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Zero: unit -> QuerySource<'T,'Q> 

        /// <summary>
        /// A method used to support the F# query syntax.  Returns a sequence of length one that contains the specified value.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Yield: value:'T -> QuerySource<'T,'Q>

        /// <summary>
        /// A method used to support the F# query syntax.  Returns a sequence that contains the specified values.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member YieldFrom: computation:QuerySource<'T,'Q> -> QuerySource<'T,'Q>

        /// <summary>
        /// A method used to support the F# query syntax.  Indicates that the query should be passed as a quotation to the Run method.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Quote: Quotations.Expr<'T> -> Quotations.Expr<'T>

        /// <summary>
        /// A method used to support the F# query syntax.  Runs the given quotation as a query using LINQ IQueryable rules.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        member Run: Quotations.Expr<QuerySource<'T,IQueryable>> -> IQueryable<'T> 
        member internal RunQueryAsQueryable: Quotations.Expr<QuerySource<'T,IQueryable>> -> IQueryable<'T> 
        member internal RunQueryAsEnumerable: Quotations.Expr<QuerySource<'T,IEnumerable>> -> seq<'T> 
        member internal RunQueryAsValue: Quotations.Expr<'T> -> 'T

        /// <summary>A query operator that determines whether the selected elements contains a specified element.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("contains")>] 
        member Contains: source:QuerySource<'T,'Q> * key:'T -> bool

        /// <summary>A query operator that returns the number of selected elements.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("count")>] 
        member Count: source:QuerySource<'T,'Q> -> int

        /// <summary>A query operator that selects the last element of those selected so far.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("last")>] 
        member Last: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that selects the last element of those selected so far, or a default value if no element is found.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("lastOrDefault")>] 
        member LastOrDefault: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that selects the single, specific element selected so far
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("exactlyOne")>] 
        member ExactlyOne: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that selects the single, specific element of those selected so far, or a default value if that element is not found.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("exactlyOneOrDefault")>] 
        member ExactlyOneOrDefault: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that selects the first element of those selected so far, or a default value if the sequence contains no elements.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("headOrDefault")>] 
        member HeadOrDefault: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that projects each of the elements selected so far.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("select",AllowIntoPattern=true)>] 
        member Select: source:QuerySource<'T,'Q> * [<ProjectionParameter>] projection:('T -> 'Result) -> QuerySource<'Result,'Q>

        /// <summary>A query operator that selects those elements based on a specified predicate. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("where",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member Where: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> QuerySource<'T,'Q>

        /// <summary>A query operator that selects a value for each element selected so far and returns the minimum resulting value. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("minBy")>] 
        member MinBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] valueSelector:('T -> 'Value) -> 'Value when 'Value: equality and 'Value: comparison

        /// <summary>A query operator that selects a value for each element selected so far and returns the maximum resulting value. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("maxBy")>] 
        member MaxBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] valueSelector:('T -> 'Value) -> 'Value when 'Value: equality and 'Value: comparison

        /// <summary>A query operator that groups the elements selected so far according to a specified key selector.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("groupBy",AllowIntoPattern=true)>] 
        member GroupBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<System.Linq.IGrouping<'Key,'T>,'Q> when 'Key: equality 

        /// <summary>A query operator that sorts the elements selected so far in ascending order by the given sorting key.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sortBy",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member SortBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that sorts the elements selected so far in descending order by the given sorting key.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sortByDescending",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member SortByDescending: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that performs a subsequent ordering of the elements selected so far in ascending order by the given sorting key.
        /// This operator may only be used immediately after a 'sortBy', 'sortByDescending', 'thenBy' or 'thenByDescending', or their nullable variants.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("thenBy",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member ThenBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that performs a subsequent ordering of the elements selected so far in descending order by the given sorting key.
        /// This operator may only be used immediately after a 'sortBy', 'sortByDescending', 'thenBy' or 'thenByDescending', or their nullable variants.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("thenByDescending",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member ThenByDescending: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that selects a value for each element selected so far and groups the elements by the given key.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("groupValBy",AllowIntoPattern=true)>] 
        member GroupValBy<'T,'Key,'Value,'Q> : source:QuerySource<'T,'Q> * [<ProjectionParameter>] resultSelector:('T -> 'Value) * [<ProjectionParameter>] keySelector:('T -> 'Key) -> QuerySource<System.Linq.IGrouping<'Key,'Value>,'Q> when 'Key: equality 

        /// <summary>A query operator that correlates two sets of selected values based on matching keys. 
        /// Normal usage is 'join y in elements2 on (key1 = key2)'. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("join",IsLikeJoin=true,JoinConditionWord="on")>] 
        member Join: outerSource:QuerySource<'Outer,'Q> * innerSource:QuerySource<'Inner,'Q> * outerKeySelector:('Outer -> 'Key) * innerKeySelector:('Inner -> 'Key) * resultSelector:('Outer -> 'Inner -> 'Result) -> QuerySource<'Result,'Q>

        /// <summary>A query operator that correlates two sets of selected values based on matching keys and groups the results. 
        /// Normal usage is 'groupJoin y in elements2 on (key1 = key2) into group'. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("groupJoin",IsLikeGroupJoin=true,JoinConditionWord="on")>] 
        member GroupJoin: outerSource:QuerySource<'Outer,'Q> * innerSource:QuerySource<'Inner,'Q> * outerKeySelector:('Outer -> 'Key) * innerKeySelector:('Inner -> 'Key) * resultSelector:('Outer -> seq<'Inner> -> 'Result) -> QuerySource<'Result,'Q>

        /// <summary>A query operator that correlates two sets of selected values based on matching keys and groups the results.
        /// If any group is empty, a group with a single default value is used instead. 
        /// Normal usage is 'leftOuterJoin y in elements2 on (key1 = key2) into group'. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("leftOuterJoin",IsLikeGroupJoin=true,JoinConditionWord="on")>] 
        member LeftOuterJoin: outerSource:QuerySource<'Outer,'Q> * innerSource:QuerySource<'Inner,'Q> * outerKeySelector:('Outer -> 'Key) * innerKeySelector:('Inner -> 'Key) * resultSelector:('Outer -> seq<'Inner> -> 'Result) -> QuerySource<'Result,'Q>

#if SUPPORT_ZIP_IN_QUERIES
        /// <summary>
        /// When used in queries, this operator corresponds to the LINQ Zip operator.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("zip",IsLikeZip=true)>] 
        member Zip: firstSource:QuerySource<'T1> * secondSource:QuerySource<'T2> * resultSelector:('T1 -> 'T2 -> 'Result) -> QuerySource<'Result>
#endif

        /// <summary>A query operator that selects a nullable value for each element selected so far and returns the sum of these values. 
        /// If any nullable does not have a value, it is ignored.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sumByNullable")>] 
        member inline SumByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] valueSelector:('T -> Nullable< ^Value >) -> Nullable< ^Value > 
                                         when ^Value: (static member ( + ): ^Value * ^Value -> ^Value) 
                                          and  ^Value: (static member Zero: ^Value)
                                          and default ^Value: int

        /// <summary>A query operator that selects a nullable value for each element selected so far and returns the minimum of these values. 
        /// If any nullable does not have a value, it is ignored.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("minByNullable")>] 
        member MinByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] valueSelector:('T -> Nullable<'Value>) -> Nullable<'Value> 
                                         when 'Value: equality 
                                         and 'Value: comparison  

        /// <summary>A query operator that selects a nullable value for each element selected so far and returns the maximum of these values. 
        /// If any nullable does not have a value, it is ignored.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("maxByNullable")>] 
        member MaxByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] valueSelector:('T -> Nullable<'Value>) -> Nullable<'Value> when 'Value: equality and 'Value: comparison  

        /// <summary>A query operator that selects a nullable value for each element selected so far and returns the average of these values. 
        /// If any nullable does not have a value, it is ignored.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("averageByNullable")>] 
        member inline AverageByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] projection:('T -> Nullable< ^Value >) -> Nullable< ^Value > 
                         when ^Value: (static member ( + ): ^Value * ^Value -> ^Value)  
                         and  ^Value: (static member DivideByInt: ^Value * int -> ^Value)  
                         and  ^Value: (static member Zero: ^Value)  
                         and default ^Value: float


        /// <summary>A query operator that selects a value for each element selected so far and returns the average of these values. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("averageBy")>] 
        member inline AverageBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] projection:('T -> ^Value) -> ^Value 
                                      when ^Value: (static member ( + ): ^Value * ^Value -> ^Value) 
                                      and  ^Value: (static member DivideByInt: ^Value * int -> ^Value) 
                                      and  ^Value: (static member Zero: ^Value)
                                      and default ^Value: float


        /// <summary>A query operator that selects distinct elements from the elements selected so far. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("distinct",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member Distinct: source:QuerySource<'T,'Q> -> QuerySource<'T,'Q> when 'T: equality

        /// <summary>A query operator that determines whether any element selected so far satisfies a condition.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("exists")>] 
        member Exists: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> bool

        /// <summary>A query operator that selects the first element selected so far that satisfies a specified condition.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("find")>] 
        member Find: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> 'T


        /// <summary>A query operator that determines whether all elements selected so far satisfies a condition.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("all")>] 
        member All: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> bool

        /// <summary>A query operator that selects the first element from those selected so far.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("head")>] 
        member Head: source:QuerySource<'T,'Q> -> 'T

        /// <summary>A query operator that selects the element at a specified index amongst those selected so far.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("nth")>] 
        member Nth: source:QuerySource<'T,'Q> * index:int -> 'T

        /// <summary>A query operator that bypasses a specified number of the elements selected so far and selects the remaining elements.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("skip",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member Skip:  source:QuerySource<'T,'Q> * count:int -> QuerySource<'T,'Q>

        /// <summary>A query operator that bypasses elements in a sequence as long as a specified condition is true and then selects the remaining elements.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("skipWhile",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member SkipWhile: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> QuerySource<'T,'Q>

        /// <summary>A query operator that selects a value for each element selected so far and returns the sum of these values. 
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sumBy")>] 
        member inline SumBy: source:QuerySource<'T,'Q> * [<ProjectionParameter>] projection:('T -> ^Value) -> ^Value 
                                      when ^Value: (static member ( + ): ^Value * ^Value -> ^Value) 
                                      and  ^Value: (static member Zero: ^Value)
                                      and default ^Value: int

        /// <summary>A query operator that selects a specified number of contiguous elements from those selected so far.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("take",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member Take: source:QuerySource<'T,'Q> * count:int-> QuerySource<'T,'Q>

        /// <summary>A query operator that selects elements from a sequence as long as a specified condition is true, and then skips the remaining elements.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("takeWhile",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member TakeWhile: source:QuerySource<'T,'Q> * [<ProjectionParameter>] predicate:('T -> bool) -> QuerySource<'T,'Q>

        /// <summary>A query operator that sorts the elements selected so far in ascending order by the given nullable sorting key.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sortByNullable",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member SortByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> Nullable<'Key>) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that sorts the elements selected so far in descending order by the given nullable sorting key.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("sortByNullableDescending",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member SortByNullableDescending: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> Nullable<'Key>) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that performs a subsequent ordering of the elements selected so far in ascending order by the given nullable sorting key.
        /// This operator may only be used immediately after a 'sortBy', 'sortByDescending', 'thenBy' or 'thenByDescending', or their nullable variants.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("thenByNullable",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member ThenByNullable: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> Nullable<'Key>) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

        /// <summary>A query operator that performs a subsequent ordering of the elements selected so far in descending order by the given nullable sorting key.
        /// This operator may only be used immediately after a 'sortBy', 'sortByDescending', 'thenBy' or 'thenByDescending', or their nullable variants.
        /// </summary>
        /// 
        /// <example-tbd></example-tbd>
        [<CustomOperation("thenByNullableDescending",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
        member ThenByNullableDescending: source:QuerySource<'T,'Q> * [<ProjectionParameter>] keySelector:('T -> Nullable<'Key>) -> QuerySource<'T,'Q> when 'Key: equality and 'Key: comparison

namespace Microsoft.FSharp.Linq.QueryRunExtensions 

    open Microsoft.FSharp.Core

    /// <summary>
    /// A module used to support the F# query syntax.  
    /// </summary>
    ///
    /// <namespacedoc><summary>
    ///    Contains modules used to support the F# query syntax.  
    /// </summary></namespacedoc>
    module LowPriority = 
        type Microsoft.FSharp.Linq.QueryBuilder with
            /// <summary>
            /// A method used to support the F# query syntax.  Runs the given quotation as a query using LINQ rules.
            /// </summary>
            [<CompiledName("RunQueryAsValue")>]
            member Run: Microsoft.FSharp.Quotations.Expr<'T> -> 'T

    /// <summary>
    /// A module used to support the F# query syntax.  
    /// </summary>
    module HighPriority = 
        type Microsoft.FSharp.Linq.QueryBuilder with
            /// <summary>
            /// A method used to support the F# query syntax.  Runs the given quotation as a query using LINQ IEnumerable rules.
            /// </summary>
            [<CompiledName("RunQueryAsEnumerable")>]
            member Run: Microsoft.FSharp.Quotations.Expr<Microsoft.FSharp.Linq.QuerySource<'T,System.Collections.IEnumerable>> -> Microsoft.FSharp.Collections.seq<'T>
