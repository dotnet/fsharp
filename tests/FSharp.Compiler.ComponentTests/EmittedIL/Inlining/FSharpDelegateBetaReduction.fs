module Program

module DelegateImmediateInvoke1 =
    type Foo = delegate of unit -> unit

    let f () =
        let f1 = Foo ignore
        f1.Invoke ()

module DelegateImmediateInvoke2 =
    type Foo = delegate of unit -> unit

    let f () = Foo(ignore).Invoke()

module DelegateImmediateInvoke3 =
    type Foo<'T> = delegate of 'T -> unit

    let f () =
        let f1 = Foo<unit> ignore
        f1.Invoke(())

module DelegateImmediateInvoke4 =
    type Foo<'T> = delegate of 'T -> unit

    let f () = Foo<unit>(ignore).Invoke(())
