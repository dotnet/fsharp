module Sample
open System
let inline worker<'a when 'a : not struct> (a: 'a) (b: 'a) : bool = obj.ReferenceEquals(a, b)
type Cache<'T when 'T : not struct and 'T : equality>(initial: 'T) =
    member _.Lookup(key: 'T) =
        let memoize (f: 'a -> 'b) =
            let cell = ref None
            let f' (x: 'a) =
                match cell.Value with
                | Some (k, v) when worker k x -> v
                | _ -> let v = f x in cell.Value <- Some (x, v); v
            f'
        let rec search n =
            if n <= 0 then false
            elif (memoize id) key = key then true
            else check (n - 1)
        and check n = search (n - 1)
        search 10
[<EntryPoint>]
let main _argv =
    if Cache("test").Lookup("test") then 0 else 1
