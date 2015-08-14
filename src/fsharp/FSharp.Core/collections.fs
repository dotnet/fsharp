// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    #nowarn "51"

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics
    open System
    open System.Diagnostics
    open System.Collections
    open System.Collections.Generic

    module HashIdentity = 
                
        
        let inline Structural<'T when 'T : equality> : IEqualityComparer<'T> = 
            LanguagePrimitives.FastGenericEqualityComparer<'T>
              
        let inline LimitedStructural<'T when 'T : equality>(limit) : IEqualityComparer<'T> = 
            LanguagePrimitives.FastLimitedGenericEqualityComparer<'T>(limit)
              
        let Reference<'T when 'T : not struct > : IEqualityComparer<'T> = 
            { new IEqualityComparer<'T> with
                  member self.GetHashCode(x) = LanguagePrimitives.PhysicalHash(x) 
                  member self.Equals(x,y) = LanguagePrimitives.PhysicalEquality x y }

        let inline NonStructural< 'T when 'T : equality and 'T  : (static member ( = ) : 'T * 'T    -> bool) > = 
            { new IEqualityComparer< 'T > with
                  member self.GetHashCode(x) = NonStructuralComparison.hash x 
                  member self.Equals(x, y) = NonStructuralComparison.(=) x y  }

        let inline FromFunctions hash eq : IEqualityComparer<'T> = 
            let eq = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(eq)
            { new IEqualityComparer<'T> with 
                member self.GetHashCode(x) = hash x
                member self.Equals(x,y) = eq.Invoke(x,y)  }


    module ComparisonIdentity = 

        let inline Structural<'T when 'T : comparison > : IComparer<'T> = 
            LanguagePrimitives.FastGenericComparer<'T>

#if BUILDING_WITH_LKG
#else
        let inline NonStructural< 'T when 'T : (static member ( < ) : 'T * 'T    -> bool) and 'T : (static member ( > ) : 'T * 'T    -> bool) > : IComparer< 'T > = 
            { new IComparer<'T> with
                  member self.Compare(x,y) = NonStructuralComparison.compare x y } 
#endif

        let FromFunction comparer = 
            let comparer = OptimizedClosures.FSharpFunc<'T,'T,int>.Adapt(comparer)
            { new IComparer<'T> with
                  member self.Compare(x,y) = comparer.Invoke(x,y) } 

