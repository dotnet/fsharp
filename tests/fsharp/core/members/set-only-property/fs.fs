namespace fsharp
type Class () =
  let mutable v = 0
  member x.Prop1 with set(value) = v <- value
                  and private get () = v

  member x.Prop2 with private set(value) = v <- value
                  and public get () = v
