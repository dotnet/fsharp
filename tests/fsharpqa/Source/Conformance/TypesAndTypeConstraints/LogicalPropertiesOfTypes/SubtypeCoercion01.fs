type ITest =
    abstract A : unit -> unit

type ITest2 =
    inherit ITest
    abstract B : unit -> int

type ITest3 =
    inherit ITest2
    abstract C : int -> unit

type Base() =
    interface ITest3 with
        member this.A() = ()
        member this.B() = 1
        member this.C x = ()

type Derived(x) =
    inherit Base()

let testFunc (x) =
    let y = x :> ITest
    x.GetType()

let r1 = testFunc (Base())
let r2 = testFunc (Derived(1))

exit <| (if r1 <> typeof<Base> && r2 <> typeof<Derived> then 1 else 0)