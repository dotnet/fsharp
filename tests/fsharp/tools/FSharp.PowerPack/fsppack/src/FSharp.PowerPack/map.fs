namespace Microsoft.FSharp.Compatibility

    open System.Collections.Generic

    type TaggedMap<'Key,'Value,'Tag when 'Tag :> IComparer<'Key> > = Microsoft.FSharp.Collections.Tagged.Map<'Key,'Value,'Tag>
    type TaggedMap<'Key,'Value> = Microsoft.FSharp.Collections.Tagged.Map<'Key,'Value>
    module Map = 

        type Provider<'Key,'T,'Tag> when 'Tag :> IComparer<'Key> =
            interface
              abstract empty: TaggedMap<'Key,'T,'Tag>;
              abstract add: 'Key -> 'T -> TaggedMap<'Key,'T,'Tag> -> TaggedMap<'Key,'T,'Tag>;
              abstract find: 'Key -> TaggedMap<'Key,'T,'Tag> -> 'T;
              abstract first: ('Key -> 'T -> 'U option) -> TaggedMap<'Key,'T,'Tag> -> 'U option;
              abstract tryfind: 'Key -> TaggedMap<'Key,'T,'Tag> -> 'T option;
              abstract remove: 'Key -> TaggedMap<'Key,'T,'Tag> -> TaggedMap<'Key,'T,'Tag>;
              abstract mem: 'Key -> TaggedMap<'Key,'T,'Tag> -> bool;
              abstract iter: ('Key -> 'T -> unit) -> TaggedMap<'Key,'T,'Tag> -> unit;
              abstract map:  ('T -> 'U) -> TaggedMap<'Key,'T,'Tag> -> TaggedMap<'Key,'U,'Tag>;
              abstract mapi: ('Key -> 'T -> 'U) -> TaggedMap<'Key,'T,'Tag> -> TaggedMap<'Key,'U,'Tag>;
              abstract fold: ('Key -> 'T -> 'State -> 'State) -> TaggedMap<'Key,'T,'Tag> -> 'State -> 'State
            end

        let MakeTagged (cf : 'Tag) : Provider<'Key,'Value,'Tag> when 'Tag :> IComparer<'Key> =
            { new Provider<_,_,_> with 
                 member p.empty = TaggedMap<_,_,_>.Empty(cf);
                 member p.add k v m  = m.Add(k,v);
                 member p.find x m = m.[x] 
                 member p.first f m = m.First(f)
                 member p.tryfind k m = m.TryFind(k)
                 member p.remove x m = m.Remove(x)
                 member p.mem x m = m.ContainsKey(x)
                 member p.iter f m = m.Iterate(f)
                 member p.map f m = m.MapRange(f)
                 member p.mapi f m = m.Map(f)
                 member p.fold f m z = m.Fold f z }

        type Provider<'Key,'Value> = Provider<'Key,'Value,IComparer<'Key>>
        let Make cf  = MakeTagged (ComparisonIdentity.FromFunction cf)




