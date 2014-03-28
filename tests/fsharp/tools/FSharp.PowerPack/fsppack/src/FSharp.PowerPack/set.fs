namespace Microsoft.FSharp.Compatibility

open System.Collections.Generic

module Set = 
    let cardinal (s : Set<'T>) = s.Count
    let elements (s : Set<'T>) = Set.toList s

        // Fold, left-to-right. 
        //
        // NOTE: This matches OCaml behaviour, though differs from the
        // behaviour of Map.fold which folds right-to-left.
    // let fold f m z = Set.fold_left (fun z x ->  f x z) z m
    let inter s1 s2 = Set.intersect s1 s2



    open Microsoft.FSharp.Collections

    // Functor
    type Provider<'T,'Tag> when 'Tag :> IComparer<'T> =
       interface
         //type t = Tagged.Set<'T,'Tag>
         abstract empty    : Tagged.Set<'T,'Tag>;
         abstract is_empty : Tagged.Set<'T,'Tag> -> bool;
         abstract mem      : 'T -> Tagged.Set<'T,'Tag> -> bool;
         abstract add      : 'T -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract singleton: 'T -> Tagged.Set<'T,'Tag>;
         abstract remove   : 'T -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract union    : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract inter    : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract diff     : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract iter     : ('T -> unit) -> Tagged.Set<'T,'Tag> -> unit;
         abstract elements : Tagged.Set<'T,'Tag> -> 'T list;
         abstract equal    : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> bool;
         abstract subset   : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> bool;
         abstract compare  : Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> -> int;
         abstract for_all  : ('T -> bool) -> Tagged.Set<'T,'Tag> -> bool;
         abstract exists   : ('T -> bool) -> Tagged.Set<'T,'Tag> -> bool;
         abstract filter   : ('T -> bool) -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag>;
         abstract partition: ('T -> bool) -> Tagged.Set<'T,'Tag> -> Tagged.Set<'T,'Tag> * Tagged.Set<'T,'Tag>;
         abstract fold     : ('T -> 'b -> 'b) -> Tagged.Set<'T,'Tag> -> 'b -> 'b;
         abstract cardinal : Tagged.Set<'T,'Tag> -> int;
         abstract min_elt  : Tagged.Set<'T,'Tag> -> 'T;
         abstract max_elt  : Tagged.Set<'T,'Tag> -> 'T;
         abstract choose   : Tagged.Set<'T,'Tag> -> 'T 
       end

    let gen_inter (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>)  = Tagged.Set<_,_>.Intersection(s1,s2)
    let gen_diff (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>)   = Tagged.Set<_,_>.Difference(s1,s2)
    let gen_iter f (s : Tagged.Set<_,_>)  = s.Iterate(f)
    let gen_elements (s : Tagged.Set<_,_>) = s.ToList()
    let gen_equal (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>)  =  Tagged.Set<_,_>.Equality(s1,s2)
    let gen_subset (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>)  = s1.IsSubsetOf(s2)
    let gen_compare (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>) = Tagged.Set<_,_>.Compare(s1,s2)
    let gen_for_all f (s : Tagged.Set<_,_>) = s.ForAll f
    let gen_exists f (s : Tagged.Set<_,_>) = s.Exists f
    let gen_filter f (s : Tagged.Set<_,_>) = s.Filter f
    let gen_partition f (s : Tagged.Set<_,_>) = s.Partition f 
    let gen_fold f (s : Tagged.Set<_,_>) acc = s.Fold f acc
    let gen_cardinal (s : Tagged.Set<_,_>) = s.Count
    let gen_size (s : Tagged.Set<_,_>) = s.Count
    let gen_min_elt (s : Tagged.Set<_,_>) = s.MinimumElement
    let gen_max_elt (s : Tagged.Set<_,_>) = s.MaximumElement

    let MakeTagged (cf : 'Tag) : Provider<'T,'Tag> when 'Tag :> IComparer<'T> =
       { new Provider<_,_> with 
           member p.empty = Tagged.Set<_,_>.Empty(cf);
           member p.is_empty s = s.IsEmpty;
           member p.mem x s = s.Contains(x);
           member p.add x s = s.Add(x);
           member p.singleton x = Tagged.Set<'T,'Tag>.Singleton(cf,x);
           member p.remove x s = s.Remove(x);
           member p.union (s1 : Tagged.Set<_,_>)  (s2 : Tagged.Set<_,_>) = Tagged.Set<_,_>.Union(s1,s2);
           member p.inter s1 s2 = gen_inter s1 s2 ;
           member p.diff s1 s2 = gen_diff s1 s2;
           member p.iter f s= gen_iter f s;
           member p.elements s = gen_elements s;
           member p.equal s1 s2= gen_equal s1 s2;
           member p.subset s1 s2= gen_subset s1 s2;
           member p.compare s1 s2 = gen_compare s1 s2;
           member p.for_all f s = gen_for_all f s;
           member p.fold f s z = gen_fold f s z;
           member p.exists f s = gen_exists f s;
           member p.filter f s = gen_filter f s;
           member p.partition f s = gen_partition f s ;
           member p.cardinal s = gen_cardinal s;
           member p.min_elt s = s.MinimumElement;
           member p.max_elt s = s.MaximumElement; 
           member p.choose s  = s.Choose }

    type Provider<'T> = Provider<'T, IComparer<'T>>
    let Make cf  = MakeTagged (ComparisonIdentity.FromFunction cf)
