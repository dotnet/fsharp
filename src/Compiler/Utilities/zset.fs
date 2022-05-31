// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections

open Internal.Utilities.Collections.Tagged
open System.Collections.Generic

/// Sets with a specific comparison function
type internal Zset<'T> = Internal.Utilities.Collections.Tagged.Set<'T>

module internal Zset =

    let empty (ord: IComparer<'T>) =
        Internal.Utilities.Collections.Tagged.Set<_, _>.Empty (ord)

    let isEmpty (s: Zset<_>) = s.IsEmpty

    let contains x (s: Zset<_>) = s.Contains(x)

    let add x (s: Zset<_>) = s.Add(x)

    let addList xs a = List.fold (fun a x -> add x a) a xs

    let singleton ord x = add x (empty ord)

    let remove x (s: Zset<_>) = s.Remove(x)

    let fold (f: 'T -> 'b -> 'b) (s: Zset<_>) b = s.Fold f b

    let iter f (s: Zset<_>) = s.Iterate f

    let forall predicate (s: Zset<_>) = s.ForAll predicate

    let count (s: Zset<_>) = s.Count

    let exists predicate (s: Zset<_>) = s.Exists predicate

    let subset (s1: Zset<_>) (s2: Zset<_>) = s1.IsSubsetOf s2

    let equal (s1: Zset<_>) (s2: Zset<_>) =
        Internal.Utilities.Collections.Tagged.Set<_, _>.Equality (s1, s2)

    let elements (s: Zset<_>) = s.ToList()

    let filter predicate (s: Zset<_>) = s.Filter predicate

    let union (s1: Zset<_>) (s2: Zset<_>) =
        Internal.Utilities.Collections.Tagged.Set<_, _>.Union (s1, s2)

    let inter (s1: Zset<_>) (s2: Zset<_>) =
        Internal.Utilities.Collections.Tagged.Set<_, _>.Intersection (s1, s2)

    let diff (s1: Zset<_>) (s2: Zset<_>) =
        Internal.Utilities.Collections.Tagged.Set<_, _>.Difference (s1, s2)

    let memberOf m k = contains k m
