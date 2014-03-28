namespace Microsoft.FSharp.Collections.Tagged

    #nowarn "51"

    open System
    open System.Collections.Generic
    open Microsoft.FSharp.Collections
    type UntaggedHashMultiMap<'Key,'Value> = Microsoft.FSharp.Collections.HashMultiMap<'Key,'Value>

    type HashMultiMap<'Key,'Value,'HashTag>
         when 'HashTag :> IEqualityComparer<'Key> =
        { t : UntaggedHashMultiMap<'Key,'Value> }

        static member Create(hasheq: 'HashTag,n:int)  : HashMultiMap<'Key,'Value,'HashTag> = 
            { t = new UntaggedHashMultiMap<_,_>(n,hasheq) }

        member x.Add(y,z) = x.t.Add(y,z)
        member x.Clear() = x.t.Clear()
        member x.Copy() : HashMultiMap<'Key,'Value,'HashTag>  = { t = x.t.Copy() }
        member x.Item with get(y) = x.t.[y]
                      and  set y z = x.t.[y] <- z
        member x.FindAll(y) = x.t.FindAll(y) 
        member x.Fold f acc =  x.t.Fold f acc
        member x.Iterate(f) =  x.t.Iterate(f)
        member x.Contains(y) = x.t.ContainsKey(y)
        member x.ContainsKey(y) = x.t.ContainsKey(y)
        member x.Remove(y) = x.t.Remove(y)
        member x.Replace(y,z) = x.t.Replace(y,z)
        member x.TryFind(y) = x.t.TryFind(y)
        member x.Count = x.t.Count

    type HashMultiMap<'Key,'Value> = HashMultiMap<'Key,'Value, IEqualityComparer<'Key>>    


    [<Sealed>]
    type HashSet<'T,'HashTag when 'T : equality> 
         when 'HashTag :> IEqualityComparer<'T>(t:  HashSet<'T>) =

        static member Create(hasheq: ('HashTag :> IEqualityComparer<'T>),size:int) : HashSet<'T,'HashTag> = 
            new HashSet<'T,'HashTag>(HashSet<_>(size,hasheq))

        member x.Add(y)    = t.Add(y)
        member x.Clear() = t.Clear()
        member x.Copy() = new HashSet<'T,'HashTag>(t.Copy())
        member x.Fold f acc = t.Fold f acc
        member x.Iterate(f) =  t.Iterate(f)
        member x.Contains(y) = t.Contains(y)
        member x.Remove(y) = t.Remove(y)
        member x.Count = t.Count

        interface IEnumerable<'T> with
            member x.GetEnumerator() = (t :> seq<_>).GetEnumerator() 

        interface System.Collections.IEnumerable with 
            member x.GetEnumerator() = (t :> System.Collections.IEnumerable).GetEnumerator()  

    type HashSet<'T when 'T : equality> = HashSet<'T, IEqualityComparer<'T>>    
