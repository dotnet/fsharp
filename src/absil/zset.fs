// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.AbstractIL.Internal

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Internal.Utilities
open Internal.Utilities.Collections.Tagged
open System.Collections.Generic

/// Sets with a specific comparison function
type internal Zset<'T> = Internal.Utilities.Collections.Tagged.Set<'T>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Zset = 

    let empty (ord : IComparer<'T>) = Internal.Utilities.Collections.Tagged.Set<_,_>.Empty(ord)

    let isEmpty (s:Zset<_>) = s.IsEmpty

    let contains x (s:Zset<_>) = s.Contains(x)
    let add x (s:Zset<_>) = s.Add(x)
    let addList xs a = List.fold (fun a x -> add x a) a xs
    let addFlatList xs a = FlatList.fold (fun a x -> add x a) a xs
        
    let singleton ord x = add x (empty ord)
    let remove x (s:Zset<_>) = s.Remove(x)

    let fold (f : 'T -> 'b -> 'b) (s:Zset<_>) b = s.Fold f b
    let iter f (s:Zset<_>) = s.Iterate f 
    let forall p (s:Zset<_>) = s.ForAll p 
    let count  (s:Zset<_>) = s.Count
    let exists  p (s:Zset<_>) = s.Exists p 
    let subset (s1:Zset<_>) (s2:Zset<_>)  = s1.IsSubsetOf s2
    let equal (s1:Zset<_>) (s2:Zset<_>)  = Internal.Utilities.Collections.Tagged.Set<_,_>.Equality(s1,s2)
    let elements (s:Zset<_>) = s.ToList()
    let filter p (s:Zset<_>) = s.Filter p

    let union (s1:Zset<_>) (s2:Zset<_>)  = Internal.Utilities.Collections.Tagged.Set<_,_>.Union(s1,s2)
    let inter (s1:Zset<_>) (s2:Zset<_>)  = Internal.Utilities.Collections.Tagged.Set<_,_>.Intersection(s1,s2)
    let diff (s1:Zset<_>) (s2:Zset<_>)  = Internal.Utilities.Collections.Tagged.Set<_,_>.Difference(s1,s2)

    let memberOf m k = contains k m
