// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.AbstractIL.Internal

open Internal.Utilities
open Internal.Utilities.Collections.Tagged
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open System.Collections.Generic

/// Maps with a specific comparison function
type internal Zmap<'Key,'T> = Internal.Utilities.Collections.Tagged.Map<'Key,'T> 

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Zmap = 

    let empty (ord: IComparer<'T>) = Map<_,_,_>.Empty(ord)

    let add k v (m:Zmap<_,_>) = m.Add(k,v)
    let find k (m:Zmap<_,_>) = m.[k]
    let tryFind k (m:Zmap<_,_>) = m.TryFind(k)
    let remove k (m:Zmap<_,_>) = m.Remove(k)
    let mem k (m:Zmap<_,_>) = m.ContainsKey(k)
    let iter f (m:Zmap<_,_>) = m.Iterate(f)
    let first f (m:Zmap<_,_>) = m.First(fun k v -> if f k v then Some (k,v) else None)
    let exists f (m:Zmap<_,_>) = m.Exists(f)
    let forall f (m:Zmap<_,_>) = m.ForAll(f)
    let map f (m:Zmap<_,_>) = m.MapRange(f)
    let mapi f (m:Zmap<_,_>) = m.Map(f)
    let fold f (m:Zmap<_,_>) x = m.Fold f x
    let toList (m:Zmap<_,_>) = m.ToList()
    let foldSection lo hi f (m:Zmap<_,_>) x = m.FoldSection lo hi f x

    let isEmpty (m:Zmap<_,_>) = m.IsEmpty

    let foldMap f z (m:Zmap<_,_>) =
      let m,z = m.FoldAndMap (fun k v z -> let z,v' = f z k v in v',z) z in
      z,m

    let choose f  (m:Zmap<_,_>) = m.First(f)
      
    let chooseL f  (m:Zmap<_,_>) =
      m.Fold (fun k v s -> match f k v with None -> s | Some x -> x::s) []
        
    let ofList m xs = List.fold (fun m (k,v) -> add k v m) (empty m) xs
    let ofFlatList m xs = FlatList.fold (fun m (k,v) -> add k v m) (empty m) xs

    let keys   m = chooseL (fun k _ -> Some k) m 
    let values m = chooseL (fun _ v -> Some v) m

    let memberOf m k = mem k m
