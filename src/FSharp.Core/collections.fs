// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

#nowarn "51"

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open System.Collections.Generic

module HashIdentity =

    let inline Structural<'T when 'T: equality> : IEqualityComparer<'T> =
        LanguagePrimitives.FastGenericEqualityComparer<'T>

    let inline LimitedStructural<'T when 'T: equality> (limit) : IEqualityComparer<'T> =
        LanguagePrimitives.FastLimitedGenericEqualityComparer<'T>(limit)

    let Reference<'T when 'T: not struct> : IEqualityComparer<'T> =
        { new IEqualityComparer<'T> with
            member _.GetHashCode(x) =
                LanguagePrimitives.PhysicalHash(x)

            member _.Equals(x, y) =
                LanguagePrimitives.PhysicalEquality x y
        }

    let inline NonStructural<'T when 'T: equality and 'T: (static member (=): 'T * 'T -> bool)> =
        { new IEqualityComparer<'T> with
            member _.GetHashCode(x) =
                NonStructuralComparison.hash x

            member _.Equals(x, y) =
                NonStructuralComparison.(=) x y
        }

    let inline FromFunctions hasher equality : IEqualityComparer<'T> =
        let eq = OptimizedClosures.FSharpFunc<_, _, _>.Adapt (equality)

        { new IEqualityComparer<'T> with
            member _.GetHashCode(x) =
                hasher x

            member _.Equals(x, y) =
                eq.Invoke(x, y)
        }

module ComparisonIdentity =

    let inline Structural<'T when 'T: comparison> : IComparer<'T> =
        LanguagePrimitives.FastGenericComparer<'T>

    let inline NonStructural<'T
        when 'T: (static member (<): 'T * 'T -> bool) and 'T: (static member (>): 'T * 'T -> bool)> : IComparer<'T> =
        { new IComparer<'T> with
            member _.Compare(x, y) =
                NonStructuralComparison.compare x y
        }

    let FromFunction comparer =
        let comparer = OptimizedClosures.FSharpFunc<'T, 'T, int>.Adapt (comparer)

        { new IComparer<'T> with
            member _.Compare(x, y) =
                comparer.Invoke(x, y)
        }
