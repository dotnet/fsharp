// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// <summary>This namespace contains some common collections in a style primarily designed for use from F#.</summary>

namespace Microsoft.FSharp.Collections
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics
    open System
    open System.Collections.Generic

    /// <summary>Common notions of comparison identity used with sorted data structures.</summary>
    module ComparisonIdentity = 
      
        /// <summary>Structural comparison.  Compare using Operators.compare.</summary>
        val inline Structural<'T> : IComparer<'T> when 'T : comparison 

#if BUILDING_WITH_LKG
#else
        /// <summary>Non-structural comparison.  Compare using NonStructuralComparison.compare.</summary>
        val inline NonStructural< ^T > : IComparer< ^T > when ^T : (static member ( < ) : ^T * ^T    -> bool) and ^T : (static member ( > ) : ^T * ^T    -> bool) 
#endif

        /// <summary>Compare using the given comparer function.</summary>
        /// <param name="comparer">A function to compare two values.</param>
        /// <returns>An object implementing IComparer using the supplied comparer.</returns>
        val FromFunction : comparer:('T -> 'T -> int) -> IComparer<'T>  
        
    /// <summary>Common notions of value identity used with hash tables.</summary>
    module HashIdentity = 

        /// <summary>Structural hashing.  Hash using Operators.(=) and Operators.hash.</summary>
        val inline Structural<'T> : IEqualityComparer<'T>  when 'T : equality
        
        /// <summary>Non-structural hashing.  Equality using NonStructuralComparison.(=) and NonStructuralComparison.hash.</summary>
        val inline NonStructural<'T> : IEqualityComparer< ^T >  when ^T : equality and ^T  : (static member ( = ) : ^T * ^T    -> bool) 
        
        val inline LimitedStructural<'T> : limit: int -> IEqualityComparer<'T>  when 'T : equality
        
        /// <summary>Physical hashing (hash on reference identity of objects, and the contents of value types).  
        /// Hash using LanguagePrimitives.PhysicalEquality and LanguagePrimitives.PhysicalHash,
        /// That is, for value types use GetHashCode and Object.Equals (if no other optimization available),
        /// and for reference types use System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode and 
        /// reference equality.</summary>
        val Reference<'T>   : IEqualityComparer<'T>  when 'T : not struct 
        
        /// <summary>Hash using the given hashing and equality functions.</summary>
        /// <param name="hasher">A function to generate a hash code from a value.</param>
        /// <param name="equality">A function to test equality of two values.</param>
        /// <returns>An object implementing IEqualityComparer using the supplied functions.</returns>

        // inline justification: allows inlining of hash functions 
        val inline FromFunctions<'T> : hasher:('T -> int) -> equality:('T -> 'T -> bool) -> IEqualityComparer<'T> 

    
