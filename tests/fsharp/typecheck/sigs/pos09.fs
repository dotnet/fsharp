namespace pos09

type A() = 
    member this.Set(v : obj) = ()
    member this.Set_1(name : string, v : obj) = ()
    static member Set2(v : obj) = ()
    static member Set2_1(name : string, v : obj) = ()

module Test =

    let inline set (df:^a) (value:^b)=(^a : (member Set : ^b -> unit) (df, value))
    let inline set_1 (df:^a) (name : string) (value:^b)=(^a : (member Set_1 : string -> ^b -> unit) (df, name, value))
    let inline set2< ^t, ^b when ^t : (static member Set2 : ^b -> unit)> (value:^b)=(^t : (static member Set2 : ^b -> unit) (value))
    let inline set2_1< ^t, ^b when ^t : (static member Set2_1 : string -> ^b -> unit)> (name : string) (value:^b)=(^t : (static member Set2_1 : string -> ^b -> unit) (name, value))

    let a = A()
    set a 1.0
    set2<A, _> 2.0

    set_1 a "name" 3.0
    set2_1<A, _> "name" 5.0