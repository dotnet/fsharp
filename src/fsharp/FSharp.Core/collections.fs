// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    #nowarn "51"

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open System.Collections.Generic

    module HashIdentity = 
                
        
        let inline Structural<'T when 'T : equality> : IEqualityComparer<'T> = 
            LanguagePrimitives.FastGenericEqualityComparer<'T>
              
        let inline LimitedStructural<'T when 'T : equality>(limit) : IEqualityComparer<'T> = 
            LanguagePrimitives.FastLimitedGenericEqualityComparer<'T>(limit)
              
        let Reference<'T when 'T : not struct > : IEqualityComparer<'T> = 
            { new IEqualityComparer<'T> with
                  member __.GetHashCode(x) = LanguagePrimitives.PhysicalHash(x) 
                  member __.Equals(x,y) = LanguagePrimitives.PhysicalEquality x y }

        let inline NonStructural< 'T when 'T : equality and 'T  : (static member ( = ) : 'T * 'T    -> bool) > = 
            { new IEqualityComparer< 'T > with
                  member __.GetHashCode(x) = NonStructuralComparison.hash x 
                  member __.Equals(x, y) = NonStructuralComparison.(=) x y  }

        let inline FromFunctions hash eq : IEqualityComparer<'T> = 
            let eq = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(eq)
            { new IEqualityComparer<'T> with 
                member __.GetHashCode(x) = hash x
                member __.Equals(x,y) = eq.Invoke(x,y)  }


    module ComparisonIdentity = 

        let inline Structural<'T when 'T : comparison > : IComparer<'T> = 
            LanguagePrimitives.FastGenericComparer<'T>

#if BUILDING_WITH_LKG
#else
        let inline NonStructural< 'T when 'T : (static member ( < ) : 'T * 'T    -> bool) and 'T : (static member ( > ) : 'T * 'T    -> bool) > : IComparer< 'T > = 
            { new IComparer<'T> with
                  member __.Compare(x,y) = NonStructuralComparison.compare x y } 
#endif

        let FromFunction comparer = 
            let comparer = OptimizedClosures.FSharpFunc<'T,'T,int>.Adapt(comparer)
            { new IComparer<'T> with
                  member __.Compare(x,y) = comparer.Invoke(x,y) } 

