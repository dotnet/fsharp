// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This namespace contains some common collections in a style primarily designed for use from F#.
namespace Microsoft.FSharp.Collections

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Primitives.Basics
open System
open System.Collections.Generic

/// <summary>Common notions of value ordering implementing the <see cref="T:System.Collections.Generic.IComparer`1"/> 
/// interface, for constructing sorted data structures and performing sorting operations.</summary>
module ComparisonIdentity = 
  
    /// <summary>Get an implementation of comparison semantics using structural comparison.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.compare"/>.</returns>
    /// 
    /// <example id="structural-1">Create and use a comparer using structural comparison:
    /// <code>
    /// let compareTuples = ComparisonIdentity.Structural&lt;int * int>
    ///
    /// compareTuples.Compare((1, 4), (1, 5))
    /// </code>
    /// Evaluates to <c>-1</c>.
    /// </example>
    val inline Structural<'T> : IComparer<'T> when 'T : comparison 

    /// <summary>Get an implementation of comparison semantics using non-structural comparison.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.Compare"/>.</returns>
    /// 
    /// <example id="nonstructural-1">Create and use a comparer using structural comparison:
    /// <code>
    /// let comparer = ComparisonIdentity.NonStructural&lt;System.DateTime>
    ///
    /// comparer.Compare(System.DateTime.Now, System.DateTime.Today)
    /// </code>
    /// Evaluates to <c>1</c>.
    /// </example>
    val inline NonStructural< ^T > : IComparer< ^T > when ^T : (static member ( < ) : ^T * ^T    -> bool) and ^T : (static member ( > ) : ^T * ^T    -> bool) 

    /// <summary>Get an implementation of comparison semantics using the given function.</summary>
    ///
    /// <param name="comparer">A function to compare two values.</param>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IComparer`1"/> using the supplied function.</returns>
    /// 
    /// <example id="fromfunction-1">Create and use a comparer using the given function:
    /// <code>
    /// let comparer = ComparisonIdentity.FromFunction(fun i1 i2 -> compare (i1%5) (i2%5))
    ///
    /// comparer.Compare(7, 2)
    /// </code>
    /// Evaluates to <c>0</c>because <c>7</c> and <c>2</c> compare as equal using to the provided function.
    /// </example>
    val FromFunction : comparer:('T -> 'T -> int) -> IComparer<'T>  
    
/// <summary>Common notions of value identity implementing the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> 
/// interface, for constructing <see cref="T:System.Collections.Generic.Dictionary`2"/> objects and other collections</summary>
module HashIdentity = 

    /// <summary>Get an implementation of equality semantics using structural equality and structural hashing.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.op_Equality"/> and <see cref="M:Microsoft.FSharp.Core.Operators.hash"/>.</returns>
    /// 
    /// <example id="structural-1">Create a dictionary which uses structural equality and structural hashing on the key, allowing an array as a key:
    /// <code>
    /// open System.Collections.Generic
    ///
    /// let dict = new Dictionary&lt;int[],int&gt;(HashIdentity.Structural)
    ///
    /// let arr1 = [| 1;2;3 |]
    /// let arr2 = [| 1;2;3 |]
    ///
    /// dict.[arr1] &lt;- 6
    /// dict.[arr2] &gt;- 7
    /// </code>
    /// In this example, only one entry is added to the dictionary, as the arrays identical by structural equality.
    /// </example>
    val inline Structural<'T> : IEqualityComparer<'T>  when 'T : equality
    
    /// <summary>Get an implementation of equality semantics using non-structural equality and non-structural hashing.</summary>
    ///
    /// <returns>
    ///  An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.op_Equality"/>
    ///  and <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.hash"/>.
    /// </returns>
    /// 
    /// <example id="nonstructural-1">Create a dictionary which uses non-structural equality and hashing on the key:
    /// <code>
    /// open System.Collections.Generic
    ///
    /// let dict = new Dictionary&lt;System.DateTime,int&gt;(HashIdentity.NonStructural)
    ///
    /// dict.Add(System.DateTime.Now, 1)
    /// </code>
    /// </example>
    val inline NonStructural<'T> : IEqualityComparer< ^T >  when ^T : equality and ^T  : (static member ( = ) : ^T * ^T    -> bool) 
    
    /// <summary>Get an implementation of equality semantics semantics using limited structural equality and structural hashing.</summary>
    ///
    /// <param name="limit">The limit on the number of hashing operations used.</param>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.</returns>
    /// 
    /// <example id="limitedstructural-1">Create a dictionary which uses limited structural equality and structural hashing on the key, allowing trees as efficient keys:
    /// <code>
    /// open System.Collections.Generic
    ///
    /// type Tree = Tree of int * Tree list
    ///
    /// let dict = new Dictionary&lt;Tree,int&gt;(HashIdentity.LimitedStructural 4)
    ///
    /// let tree1 = Tree(0, [])
    /// let tree2 = Tree(0, [tree1; tree1])
    /// dict.Add(tree1, 6)
    /// dict.Add(tree2, 7)
    /// </code>
    /// </example>
    val inline LimitedStructural<'T> : limit: int -> IEqualityComparer<'T>  when 'T : equality
    
    /// <summary>Get an implementation of equality semantics using reference equality and reference hashing.</summary>
    ///
    /// <returns>
    ///  An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.LanguagePrimitives.PhysicalEquality"/>
    ///  and <see cref="M:Microsoft.FSharp.Core.LanguagePrimitives.PhysicalHash"/>.
    /// </returns>
    /// 
    /// <example id="reference-1">Create a dictionary which uses reference equality and hashing on the key, giving each key reference identity:
    /// <code>
    /// open System.Collections.Generic
    ///
    /// let dict = new Dictionary&lt;int[],int&gt;(HashIdentity.Structural)
    ///
    /// let arr1 = [| 1;2;3 |]
    /// let arr2 = [| 1;2;3 |]
    /// dict.Add(arr1, 6)
    /// dict.Add(arr2, 7)
    /// </code>
    /// In this example, two entries are added to the dictionary, as the arrays have different object reference identity.
    /// </example>
    val Reference<'T>   : IEqualityComparer<'T>  when 'T : not struct 
    
    /// <summary>Get an implementation of equality semantics using the given functions.</summary>
    ///
    /// <param name="hasher">A function to generate a hash code from a value.</param>
    /// <param name="equality">A function to test equality of two values.</param>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using the given functions.</returns>
    /// 
    /// <example id="fromfunctions-1">Create a dictionary which uses the given functions for equality and hashing:
    /// <code>
    /// open System.Collections.Generic
    ///
    /// let modIdentity = HashIdentity.FromFunctions((fun i -> i%5), (fun i1 i2 -> i1%5 = i2%5))
    /// let dict = new Dictionary&lt;int,int&gt;(HashIdentity.FromFunctions)
    ///
    /// dict.[2] &lt;- 6
    /// dict.[7] &lt;- 10
    /// </code>
    /// In this example, only one entry is added, as the keys <c>2</c> and <c>7</c> have the same hash and are equal according to the provided functions.
    /// </example>
    val inline FromFunctions<'T> : hasher:('T -> int) -> equality:('T -> 'T -> bool) -> IEqualityComparer<'T> 
