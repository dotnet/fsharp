// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>This namespace contains some common collections in a style primarily designed for use from F#.</summary>

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
    /// <example-tbd></example-tbd>
    val inline Structural<'T> : IComparer<'T> when 'T : comparison 

    /// <summary>Get an implementation of comparison semantics using non-structural comparison.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.Compare"/>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val inline NonStructural< ^T > : IComparer< ^T > when ^T : (static member ( < ) : ^T * ^T    -> bool) and ^T : (static member ( > ) : ^T * ^T    -> bool) 

    /// <summary>Get an implementation of comparison semantics using the given function.</summary>
    ///
    /// <param name="comparer">A function to compare two values.</param>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IComparer`1"/> using the supplied function.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val FromFunction : comparer:('T -> 'T -> int) -> IComparer<'T>  
    
/// <summary>Common notions of value identity implementing the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> 
/// interface, for constructing <see cref="T:System.Collections.Generic.Dictionary`2"/> objects and other collections</summary>
module HashIdentity = 

    /// <summary>Get an implementation of equality semantics using structural equality and structural hashing.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.op_Equality"/> and <see cref="M:Microsoft.FSharp.Core.Operators.hash"/>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val inline Structural<'T> : IEqualityComparer<'T>  when 'T : equality
    
    /// <summary>Get an implementation of equality semantics using non-structural equality and non-structural hashing.</summary>
    ///
    /// <returns>
    ///  An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.op_Equality"/>
    ///  and <see cref="M:Microsoft.FSharp.Core.Operators.NonStructuralComparison.hash"/>.
    /// </returns>
    /// 
    /// <example-tbd></example-tbd>
    val inline NonStructural<'T> : IEqualityComparer< ^T >  when ^T : equality and ^T  : (static member ( = ) : ^T * ^T    -> bool) 
    
    /// <summary>Get an implementation of equality semantics semantics using structural equality and structural hashing.</summary>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val inline LimitedStructural<'T> : limit: int -> IEqualityComparer<'T>  when 'T : equality
    
    /// <summary>Get an implementation of equality semantics using reference equality and reference hashing.</summary>
    ///
    /// <returns>
    ///  An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using <see cref="M:Microsoft.FSharp.Core.LanguagePrimitives.PhysicalEquality"/>
    ///  and <see cref="M:Microsoft.FSharp.Core.LanguagePrimitives.PhysicalHash"/>.
    /// </returns>
    /// 
    /// <example-tbd></example-tbd>
    val Reference<'T>   : IEqualityComparer<'T>  when 'T : not struct 
    
    /// <summary>Get an implementation of equality semantics using the given functions.</summary>
    ///
    /// <param name="hasher">A function to generate a hash code from a value.</param>
    /// <param name="equality">A function to test equality of two values.</param>
    ///
    /// <returns>An object implementing <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> using the given functions.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val inline FromFunctions<'T> : hasher:('T -> int) -> equality:('T -> 'T -> bool) -> IEqualityComparer<'T> 
