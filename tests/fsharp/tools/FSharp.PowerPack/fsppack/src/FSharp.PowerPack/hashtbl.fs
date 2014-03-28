module Microsoft.FSharp.Compatibility.OCaml.Hashtbl

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.IO

type HashTable<'Key,'Val> = HashMultiMap<'Key,'Val> 
type ('Key,'Val) t = HashMultiMap<'Key,'Val> 

let inline create (n:int) = new HashMultiMap<_,_>(n, HashIdentity.Structural)

let add (t:HashMultiMap<'Key,'Val>) x y = t.Add(x,y)
let of_list (l : _ list) = 
     let t = create (List.length l) in 
     List.iter (fun (x,y) -> add t x y) l; 
     t

let of_seq (l:seq<_>) = 
    let arr = Array.ofSeq l in 
    let t = create (Array.length arr) in Array.iter (fun (x,y) -> add t x y) arr; 
    t

let copy (t:HashMultiMap<'Key,'Val>)  = t.Copy()
let find (t:HashMultiMap<'Key,'Val>)  x = t.[x]
let tryfind (t:HashMultiMap<'Key,'Val>)  x = t.TryFind x
let find_all (t:HashMultiMap<'Key,'Val>)  x = t.FindAll x
let mem (t:HashMultiMap<'Key,'Val>)  x =  t.ContainsKey x
let remove (t:HashMultiMap<'Key,'Val>)  x = t.Remove x
let replace (t:HashMultiMap<'Key,'Val>)  x y = t.Replace(x,y)
let fold f (t:HashMultiMap<'Key,'Val>)  c = t.Fold f c
let clear (t:HashMultiMap<'Key,'Val>)  = t.Clear ()
let iter f (t:HashMultiMap<'Key,'Val>)  = t.Iterate f

let hash x = LanguagePrimitives.GenericHash x
let hashq x = LanguagePrimitives.PhysicalHash x

// Componentized Hash Tables
type Provider<'Key,'Val,'Tag> 
     when 'Tag :> IEqualityComparer<'Key> =
  interface
    abstract create  : int -> Tagged.HashMultiMap<'Key,'Val,'Tag>;
    abstract clear   : Tagged.HashMultiMap<'Key,'Val,'Tag> -> unit;
    abstract add     : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> 'Val -> unit;
    abstract copy    : Tagged.HashMultiMap<'Key,'Val,'Tag> -> Tagged.HashMultiMap<'Key,'Val,'Tag>;
    abstract find    : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> 'Val;
    abstract find_all: Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> 'Val list;
    abstract tryfind : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> 'Val option;
    abstract mem     : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> bool;
    abstract remove  : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> unit;
    abstract replace : Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'Key -> 'Val -> unit;
    abstract iter    : ('Key -> 'Val -> unit) -> Tagged.HashMultiMap<'Key,'Val,'Tag> -> unit;
    abstract fold    : ('Key -> 'Val -> 'State -> 'State) -> Tagged.HashMultiMap<'Key,'Val,'Tag> -> 'State -> 'State;
    
  end

type Provider<'Key,'Val> = Provider<'Key,'Val,IEqualityComparer<'Key>>

let MakeTagged (ops : 'Tag) : Provider<'Key,'Val,'Tag> when 'Tag :> IEqualityComparer<'Key> = 
  { new Provider<_,_,_> with 
      member p.create n = Tagged.HashMultiMap<'Key,'Val,'Tag>.Create(ops,n);
      member p.clear c = c.Clear();
      member p.add c x y = c.Add(x,y);
      member p.copy c = c.Copy();
      member p.find  c x = c.[x];
      member p.find_all c x = c.FindAll(x);
      member p.tryfind c x = c.TryFind(x);
      member p.mem c x = c.ContainsKey(x);
      member p.remove c x = c.Remove(x);
      member p.replace c x y = c.Replace(x,y);
      member p.iter f c = c.Iterate(f);
      member p.fold f c acc = c.Fold f acc; }

let Make (hash,eq) = MakeTagged (HashIdentity.FromFunctions hash eq)

